using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace WindowApp.Factory
{
    public abstract class CloudServiceFactory
    {
        protected static readonly IConfigurationRoot Builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName)
            .AddJsonFile("configuration.json", false, true)
            .Build();

        public abstract Task<ICloudService?> CreateCloudService();

        protected abstract string[] GetAwsSecrets();

        protected abstract Task<bool> CheckConnection<T>(T cloudService) where T : ICloudService;
    }
}