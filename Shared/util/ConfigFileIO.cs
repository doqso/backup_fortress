using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.models;

namespace Shared.util
{
    public class ConfigFileIO
    {
        private static readonly string ConfigFilePath;

        static ConfigFileIO()
        {
            var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
            ConfigFilePath = Path.Combine(projectRoot!, "Shared", "configuration.json");
        }

        public static CloudAccount? ReadAccountCredentials(string cloudName)
        {
            var configJson = GetConfigurationJson();
            var mainToken = configJson.SelectToken("accounts").SelectToken(cloudName);

            if (mainToken == null) return null;

            var cloudAccount = new CloudAccount
            {
                AccessKey = mainToken.SelectToken("access_key").ToString(),
                SecretAccessKey = mainToken.SelectToken("secret_access_key").ToString()
            };

            return cloudAccount;
        }

        public static bool WriteAccountCredentials(CloudAccount account, string cloudName)
        {
            var configJson = GetConfigurationJson();

            configJson.SelectToken("accounts")?
                .SelectToken(cloudName)
                .Replace(JToken.FromObject(account));

            var str = configJson.ToString();

            File.WriteAllText(ConfigFilePath, str);

            return true;
        }

        private static JObject? GetConfigurationJson()
        {
            var file = File.ReadAllText(ConfigFilePath);
            return JsonConvert.DeserializeObject<JObject>(file);
        }
    }
}