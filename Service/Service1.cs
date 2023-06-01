using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Service.Models;
using Shared.Factory;
using Shared.Services;
using Shared.util;
using System.IO;
using System.Threading;

namespace Service
{
    public sealed partial class Service1 : ServiceBase
    {
        private List<ICloudService> CloudServices { get; set; } = new List<ICloudService>();
        private List<Thread> BackupThreads { get; set; } = new List<Thread>();

        public Service1()
        {
            InitializeComponent();

            EventLog.Source = "Backup Fortress";
            EventLog.WriteEntry("config file: " + ConfigIO.ConfigFilePath, EventLogEntryType.Information);
        }

        protected override async void OnStart(string[] args)
        {
            await InitCloudServices();

            var synchronizedFiles = ConfigIO.ReadSynchronizedFiles()
                .Select(f => new CloudFileWrapper(f, CloudServices, EventLog));

            List<CloudFileWrapper> WrappedFiles = new List<CloudFileWrapper>();
            WrappedFiles.AddRange(synchronizedFiles);

            EventLog.WriteEntry("Files to backup: " + WrappedFiles.Count, EventLogEntryType.Information);
            EventLog.WriteEntry("Cloud Services: " + CloudServices.Count, EventLogEntryType.Information);

            foreach (var wrappedFile in WrappedFiles)
            {
                var t = new Thread(() => wrappedFile.StartBackup());
                BackupThreads.Add(t);
                t.Start();
            }
        }

        protected override void OnContinue()
        {
            OnStart(null);
            EventLog.WriteEntry("OnContinue", EventLogEntryType.Information);
        }

        protected override void OnPause()
        {
            OnStop();
            EventLog.WriteEntry("OnPause", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            foreach (var threads in BackupThreads)
            {
                threads.Abort();
            }

            BackupThreads.Clear();
            CloudServices.Clear();

            EventLog.WriteEntry("OnStop", EventLogEntryType.Information);
        }

        private async Task InitCloudServices()
        {
            var credentials = ConfigIO.ReadAccountCredentials("Aws");

            if (credentials == null) return;

            var awsService = await new AwsServiceFactory()
                .CreateCloudService(credentials.AccessKey, credentials.SecretAccessKey);

            if (awsService != null) CloudServices.Add(awsService);

            //... Add other cloud services here
        }
    }
}