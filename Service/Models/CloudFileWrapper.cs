using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
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
        private EventLog EventLog { get; set; }

        public CloudFileWrapper(CloudFile file, IReadOnlyCollection<ICloudService> cloudServices, EventLog eventLog)
        {
            File = file;
            MyTimer = new Timer();
            CloudServices = cloudServices;
            EventLog = eventLog;
        }

        public void StartBackup()
        {
            MyTimer.Interval = File.NextBackupMilliseconds();
            MyTimer.Elapsed += MakeBackup;
            MyTimer.Start();
        }

        private async void MakeBackup(object sender, ElapsedEventArgs e)
        {
            MyTimer.Stop();

            var localPath = File.LocalPath;
            var isDirectory = Directory.Exists(localPath);

            try
            {
                var cloudMatches = CloudServices.Where(d => File.Clouds.Contains(d.Name));
                if (!cloudMatches.Any()) throw new Exception();

                EventLog.WriteEntry("Start comppressing: " + localPath, EventLogEntryType.Information);

                if (isDirectory) FilesIO.CompressAndWrite(ref localPath);

                EventLog.WriteEntry("File uploading: " + localPath, EventLogEntryType.Information);

                foreach (var cloudService in cloudMatches)
                {
                    await cloudService.UploadFileAsync(File.Container, localPath);
                }

                if (isDirectory) FilesIO.DeleteFile(localPath);

                EventLog.WriteEntry("Backup made: " + localPath, EventLogEntryType.Information);

                File.UpdateLastBackup();
                ConfigIO.WriteSynchronizedFile(File);
                MyTimer.Interval = File.NextBackupMilliseconds();

                MyTimer.Start();
            }
            catch (Exception ex)
            {
                StopBackup();
            }
        }

        public void StopBackup()
        {
            MyTimer.Stop();
            EventLog.WriteEntry("Stop backup: " + File.LocalPath, EventLogEntryType.Information);
        }
    }
}