using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Shared.models;
using Shared.Services;
using Shared.util;
using Timer = System.Timers.Timer;

namespace Service.Models
{
    public class CloudFileWrapper
    {
        private Timer MyTimer { get; }
        private IReadOnlyCollection<ICloudService> CloudServices { get; }
        private CloudFile File { get; }

        public CloudFileWrapper(CloudFile file, IReadOnlyCollection<ICloudService> cloudServices)
        {
            File = file;
            MyTimer = new Timer();
            CloudServices = cloudServices;
        }

        public void StartBackup()
        {
            MyTimer.Interval = File.NextBackupMilliseconds();
            MyTimer.Elapsed += (sender, e) => new Thread(() => DoBackup());
            MyTimer.Start();
        }

        private async void DoBackup()
        {
            var backupMadeAtLeastOnce = false;
            var localPath = File.LocalPath;
            var isDirectory = Directory.Exists(localPath);

            try
            {
                if (isDirectory) FilesIO.CompressAndWrite(ref localPath);

                foreach (var cloudPath in File.Clouds)
                {
                    var cloudService = CloudServices.Single(c => c.Name.Equals(cloudPath));

                    if (cloudService == null) continue;

                    await cloudService.UploadFileAsync(File.Container, localPath);

                    backupMadeAtLeastOnce = true;
                }
                
                if (isDirectory) FilesIO.RemoveFoldersBackup(localPath);
                
                if (!backupMadeAtLeastOnce) throw new Exception();

                File.UpdateLastBackup();
                ConfigIO.WriteSynchronizedFile(File);
                MyTimer.Interval = File.NextBackupMilliseconds();


            }
            catch (Exception ex)
            {
                if (!backupMadeAtLeastOnce) StopBackup();
            }
        }

        public void StopBackup()
        {
            MyTimer.Stop();
        }
    }
}