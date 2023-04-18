using System.Threading.Tasks;
using SharedLibrary.Services;

namespace SharedLibrary.Factory
{
    public abstract class CloudServiceFactory
    {
        public abstract Task<ICloudService> CreateCloudService();

        protected abstract Task<bool> CheckConnection<T>(T cloudService) where T : ICloudService;
    }
}