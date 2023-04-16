using Newtonsoft.Json;
using Shared.Services;

namespace Shared.Factory
{
    public abstract class CloudServiceFactory
    {
        public abstract Task<ICloudService?> CreateCloudService();

        protected abstract Task<bool> CheckConnection<T>(T cloudService) where T : ICloudService;
    }
}