using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Factory;
using Shared.models;
using Shared.Services;
using Shared.Services.AWS;
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
            foreach (var cloudPath in File.Clouds)
            {
                var singleCloudService = CloudServices.Single(c => c.Name.Equals(cloudPath));

                var bucket = File.Container;
                var localPath = File.LocalPath;

                await singleCloudService.UploadFileAsync(bucket, localPath);
            }

            File.UpdateLastBackup();
            ConfigFileIO.WriteSynchronizedFile(File);
            MyTimer.Interval = File.NextBackupMilliseconds();
        }
    }
}