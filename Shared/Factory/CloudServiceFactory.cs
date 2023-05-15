using System.Threading.Tasks;
using Shared.Services;

namespace Shared.Factory
{
    public abstract class CloudServiceFactory
    {
        public abstract Task<ICloudService> CreateCloudService(string accessKey, string secretAccessKey);

        protected abstract Task<bool> CheckConnection<T>(T cloudService) where T : ICloudService;
    }
}