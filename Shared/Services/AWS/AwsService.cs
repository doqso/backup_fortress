using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Shared.util;

namespace Shared.Services.AWS
{
    public class AwsService : ICloudService
    {
        public string Name { get; }
        private readonly IAmazonS3 _client;

        public AwsService(IAmazonS3 client)
        {
            Name = "Aws";
            _client = client;
        }

        // Need to create thread to wrap this method
        public async Task<HttpStatusCode> UploadFileAsync(string bucket, string filePath)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = Path.GetFileName(filePath),
                FilePath = filePath,
            };

            return (await _client.PutObjectAsync(request)).HttpStatusCode;
        }


        public async Task<HttpStatusCode> DownloadFileAsync(string bucketName, string objectName, string savePath)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectName
            };

            var response = await _client.GetObjectAsync(request);
            var responseStream = response.ResponseStream;

            FilesIO.Write(responseStream, savePath);

            response.Dispose();
            responseStream.Dispose();

            return response.HttpStatusCode;
        }

        public async Task<HttpStatusCode> DeleteFileAsync(string bucketName, string objectName)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = objectName
            };
            return (await _client.DeleteObjectAsync(request)).HttpStatusCode;
        }

        public async Task<List<S3Object>> ListFilesAsync(string bucketName)
        {
            var resp = await _client.ListVersionsAsync(bucketName);

            foreach(var vers in resp.Versions)
            {
                Console.WriteLine(vers.LastModified + " | " + vers.VersionId);
            }

            return (await _client.ListObjectsAsync(bucketName))
                .S3Objects;
        }

        public async Task<List<S3Bucket>> ListBucketsAsync()
        {
            return (await _client.ListBucketsAsync())
                .Buckets;
        }

        public async Task<HttpStatusCode> CreateBucketAsync(string bucketName)
        {
            var response = await _client.PutBucketAsync(bucketName);

            var request = new PutBucketVersioningRequest
            {
                BucketName = bucketName,
                VersioningConfig = new S3BucketVersioningConfig
                {
                    Status = VersionStatus.Enabled
                }
            };

            await _client.PutBucketVersioningAsync(request);

            return response.HttpStatusCode;
        }

        public async Task<HttpStatusCode> DeleteBucketAsync(string bucketName)
        {
            try
            {
                await _client.DeleteBucketAsync(bucketName);
                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return HttpStatusCode.BadRequest;
            }

        }
    }
}