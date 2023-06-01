using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Shared.Services;
using Shared.Services.AWS;
using Shared.util;

namespace Shared.Factory
{
    public class AwsServiceFactory : CloudServiceFactory
    {
        public override async Task<ICloudService> CreateCloudService(string accessKey, string secretAccessKey)
        {
            if (accessKey == null || accessKey.Trim().Length < 1 ||
                secretAccessKey == null || secretAccessKey.Trim().Length < 1) 
                return null;
             
            var client = new AmazonS3Client(accessKey, secretAccessKey, RegionEndpoint.EUSouth2);
            var cloudService = new AwsService(client);

            return await CheckConnection(cloudService) ? cloudService : null;
        }

        protected override async Task<bool> CheckConnection<T>(T cloudService)
        {
            try
            {
                await cloudService.ListBucketsAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}