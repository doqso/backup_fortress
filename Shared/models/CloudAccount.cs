using System;
using Newtonsoft.Json;

namespace Shared.models
{
    public class CloudAccount
    {
        [JsonProperty("access_key")] public string AccessKey { get; set; } = "";

        [JsonProperty("secret_access_key")] public string SecretAccessKey { get; set; } = "";
    }
}