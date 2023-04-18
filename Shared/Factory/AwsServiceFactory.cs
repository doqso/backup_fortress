﻿using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using SharedLibrary.Services;
using SharedLibrary.Services.AWS;
using SharedLibrary.util;

namespace SharedLibrary.Factory
{
    public class AwsServiceFactory : CloudServiceFactory
    {
        public override async Task<ICloudService> CreateCloudService()
        {
            var mySecrets = ConfigFileIO.ReadAccountCredentials("AWS");

            if (mySecrets == null) return null;

            var client = new AmazonS3Client(mySecrets.AccessKey, mySecrets.SecretAccessKey, RegionEndpoint.EUSouth2);
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