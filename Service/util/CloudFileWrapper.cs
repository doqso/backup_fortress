using System;
using System.Collections.Generic;
using System.Linq;
using Shared.models;
using Shared.Services;
using Shared.util;
using Timer = System.Timers.Timer;

namespace Service.util
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
            MyTimer.Elapsed += DoBackup;
            MyTimer.Start();
        }

        public void StopBackup()
        {
            MyTimer.Stop();
        }

        private async void DoBackup(object sender, EventArgs e)
        {
            var backupMadeAtLeastOnce = false;

            try
            {
                foreach (var cloudPath in File.Clouds)
                {
                    var cloudService = CloudServices.Single(c => c.Name.Equals(cloudPath));

                    var bucket = File.Container;
                    var localPath = File.LocalPath;

                    if (cloudService == null) continue;

                    await cloudService.UploadFileAsync(bucket, localPath);

                    backupMadeAtLeastOnce = true;
                }

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
    }
}