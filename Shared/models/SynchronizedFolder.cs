using System;
using Newtonsoft.Json;

namespace Shared.models
{
    public class SynchronizedFolder
    {
        [JsonProperty("local_path")] public string LocalPath { get; set; } = "";

        [JsonProperty("remote_path")] public List<RemotePath> RemotePath { get; set; } = new();

        [JsonProperty("backup_frequency")] public int BackupFrequency { get; set; }
    }

    public class RemotePath
    {
        [JsonProperty("cloud_name")] public string CloudName { get; set; } = "";

        [JsonProperty("bucket")] public string Bucket { get; set; } = "";

        [JsonProperty("path")] public string Path { get; set; } = "";
    }
}