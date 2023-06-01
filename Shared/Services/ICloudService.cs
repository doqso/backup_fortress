using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace Shared.Services
{
    public interface ICloudService
    {
        string Name { get; }
        Task<HttpStatusCode> UploadFileAsync(string bucket, string filePath);
        Task<HttpStatusCode> DownloadFileAsync(string bucketName, string objectName, string versionId, string filePath);
        Task<HttpStatusCode> DeleteFileAsync(string bucketName, string objectName, string versionId);
        Task<HttpStatusCode> CreateBucketAsync(string bucketName);
        Task<HttpStatusCode> DeleteBucketAsync(string bucketName);
        Task<Dictionary<S3ObjectVersion, List<S3ObjectVersion>>> ListFilesAsync(string bucketName);
        Task<List<S3Bucket>> ListBucketsAsync();
    }
}