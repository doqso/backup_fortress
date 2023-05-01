using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.models;
using Shared.util;

namespace WindowApp.models
{
    public class CloudFileWrapper : CloudFile
    {
        [JsonIgnore] public bool IsAws { get; set; }

        [JsonIgnore] public bool IsOvh { get; set; }

        [JsonIgnore] public long BackupInHours { get; set; }

        public CloudFileWrapper()
        {
        }

        public CloudFileWrapper(CloudFile cloudFile)
        {
            LocalPath = cloudFile.LocalPath;
            Clouds = cloudFile.Clouds;
            BackupInHours = FromMillisecondsToHours(cloudFile.BackupFrequency);
            LastBackup = cloudFile.LastBackup;
            Container = cloudFile.Container;

            IsAws = Clouds.Any(cloudPath => cloudPath.Equals("Aws"));
            IsOvh = Clouds.Any(cloudPath => cloudPath.Equals("Ovh"));
        }

        private long FromMillisecondsToHours(long milliseconds)
        {
            return milliseconds / 60 / 60 / 1000;
        }

        private long FromHoursToMilliseconds(long hours)
        {
            return hours * 60 * 60 * 1000;
        }

        // This function replaces IsAws and IsOvh variables
        // for corresponding data in CloudPath
        public CloudFile GetCloudFile()
        {
            BackupFrequency = FromHoursToMilliseconds(BackupInHours);

            if (IsAws) this.Clouds.Add("Aws");
            else this.Clouds.Remove("Aws");

            if (IsOvh) this.Clouds.Add("Ovh");
            else this.Clouds.Remove("Ovh");

            return this;
        }
    }
}