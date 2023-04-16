using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using WindowApp.Services.AWS;

namespace WindowApp.Factory
{
    public class AwsServiceFactory : CloudServiceFactory
    {
        public override async Task<ICloudService?> CreateCloudService()
        {
            var mySecrets = GetAwsSecrets();
            var client = new AmazonS3Client(mySecrets[0], mySecrets[1], RegionEndpoint.EUSouth2);
            var cloudService = new AwsService(client);

            if (await CheckConnection(cloudService)) return cloudService;

            return null;
        }

        protected override string[] GetAwsSecrets()
        {
            var mySecrets = new string[2];
            var awsSection = Builder.GetSection("accounts").GetSection("AWS");

            mySecrets[0] = awsSection.GetSection("access_key").Value;
            mySecrets[1] = awsSection.GetSection("secret_access_key").Value;

            return mySecrets;
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