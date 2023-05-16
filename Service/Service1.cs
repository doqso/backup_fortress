using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Service.Models;
using Shared.Factory;
using Shared.Services;
using Shared.util;

namespace Service
{
    public sealed partial class Service1 : ServiceBase
    {
        private List<ICloudService> CloudServices { get; set; }
        private List<CloudFileWrapper> WrappedFiles { get; set; }

        public Service1()
        {
            InitializeComponent();

            EventLog.Source = "Backup Fortress";
            EventLog.WriteEntry("config file: " + ConfigIO.ConfigFilePath, EventLogEntryType.Information);
        }

        protected override async void OnStart(string[] args)
        {
            CloudServices = new List<ICloudService>();
            await AddCloudServices();

            WrappedFiles = ConfigIO.ReadSynchronizedFiles()
                .Select(f => new CloudFileWrapper(f, CloudServices)).ToList();

            EventLog.WriteEntry("Files to backup: " + WrappedFiles.Count, EventLogEntryType.Information);

            foreach (var wrappedFile in WrappedFiles)
            {
                wrappedFile.StartBackup();
            }
        }

        protected override void OnContinue()
        {
            OnStart(null);
        }

        protected override void OnPause()
        {
            OnStop();
        }

        protected override void OnStop()
        {
            foreach (var wrappedFile in WrappedFiles)
            {
                wrappedFile.StopBackup();
            }
        }

        private async Task AddCloudServices()
        {
            var credentials = ConfigIO.ReadAccountCredentials("Aws");
            
            var awsService = await new AwsServiceFactory()
                .CreateCloudService(credentials.AccessKey, credentials.SecretAccessKey);
            if (awsService != null) CloudServices.ToList().Add(awsService);

            //... Add other cloud services here
        }
    }
}