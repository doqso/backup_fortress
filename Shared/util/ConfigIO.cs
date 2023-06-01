using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.models;

namespace Shared.util
{
    public class ConfigIO
    {
        public static string ConfigFilePath { get; }

        static ConfigIO()
        {
#if DEBUG
            var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
            ConfigFilePath = Path.Combine(projectRoot, "Shared", "config.development.json");
#else
            var projectRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); 
            ConfigFilePath = Path.Combine(projectRoot, "Backup Fortress", "config.json");
#endif
        }

        public static CloudAccount ReadAccountCredentials(string cloudName)
        {
            var configJson = ReadConfigurationJson();
            var credentialsToken = configJson.SelectToken("accounts")?.SelectToken(cloudName);

            if (credentialsToken == null) return null;

            var cloudAccount = new CloudAccount
            {
                AccessKey = credentialsToken.SelectToken("access_key").ToString(),
                SecretAccessKey = credentialsToken.SelectToken("secret_access_key").ToString()
            };

            return cloudAccount;
        }

        public static bool WriteAccountCredentials(CloudAccount account, string cloudName)
        {
            var configJson = ReadConfigurationJson();

            configJson.SelectToken("accounts")?
                .SelectToken(cloudName)?
                .Replace(JToken.FromObject(account));

            File.WriteAllText(ConfigFilePath, configJson.ToString());

            return true;
        }

        public static List<CloudFile> ReadSynchronizedFiles()
        {
            var configJson = ReadConfigurationJson();

            var mainToken = (JArray)configJson.SelectToken("synchronized_files")
                            ?? JArray.Parse("[]");

            return JsonConvert.DeserializeObject<List<CloudFile>>(mainToken.ToString());
        }

        public static bool WriteSynchronizedFile(CloudFile cloudFiles)
        {
            var configJson = ReadConfigurationJson();

            configJson.SelectToken("synchronized_files")?
                .SingleOrDefault(c => c.Value<string>("local_path").Equals(cloudFiles.LocalPath))?
                .Replace(JToken.FromObject(cloudFiles));

            File.WriteAllText(ConfigFilePath, configJson.ToString());

            return true;
        }

        public static bool WriteSynchronizedFiles(List<CloudFile> cloudFiles)
        {
            var configJson = ReadConfigurationJson();

            var synchronizedToken = new JArray();
            synchronizedToken.Clear();

            foreach (var cloudFile in cloudFiles)
            {
                synchronizedToken.Add(JToken.FromObject(cloudFile));
            }

            configJson.SelectToken("synchronized_files")?.Replace(synchronizedToken);

            File.WriteAllText(ConfigFilePath, configJson.ToString());

            return true;
        }

        private static JObject ReadConfigurationJson()
        {
            var file = File.ReadAllText(ConfigFilePath);
            return JsonConvert.DeserializeObject<JObject>(file);
        }
    }
}