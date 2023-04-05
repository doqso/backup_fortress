using Amazon.S3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Amazon;
using Microsoft.Extensions.Configuration;
using WindowApp.AWS;

namespace WindowApp.Factory
{
    public class AwsServiceFactory : CloudServiceFactory
    {
        public override CloudService CreateCloudService()
        {
            var mySecrets = GetAwsSecrets();
            var client = new AmazonS3Client(mySecrets[0], mySecrets[1], RegionEndpoint.EUSouth2);
            return new AwsService(client);
        }


        private string[] GetAwsSecrets()
        {
            var mySecrets = new string[2];
            var awsSection = builder.GetSection("AWS");

            mySecrets[0] = awsSection.GetSection("AccessKey").Value;
            mySecrets[1] = awsSection.GetSection("Secret_access_key").Value;

            return mySecrets;
        }
    }
}