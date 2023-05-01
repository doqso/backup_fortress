using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Shared.models
{
    public class CloudFile
    {
        [JsonProperty("local_path")] public string LocalPath { get; set; } = "";

        [JsonProperty("clouds")] public HashSet<string> Clouds { get; set; } = new HashSet<string>();

        [JsonProperty("backup_frequency")] public long BackupFrequency { get; set; }

        [JsonProperty("last_backup")] public long LastBackup { get; set; }

        [JsonProperty("container")] public string Container { get; set; } = "";

        public long NextBackupMilliseconds()
        {
            var difference = DateTimeOffset
                                 .FromUnixTimeMilliseconds(LastBackup)
                                 .AddMilliseconds(BackupFrequency).ToUnixTimeMilliseconds() -
                             DateTimeOffset.Now.ToUnixTimeMilliseconds();

            return Math.Max(100, difference);
        }

        public void UpdateLastBackup()
        {
            LastBackup = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}