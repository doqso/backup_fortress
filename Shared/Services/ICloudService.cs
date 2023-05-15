using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace Shared.Services
{
    public interface ICloudService
    {
        string Name { get; }
        Task<HttpStatusCode> UploadFileAsync(string bucket, string filePath);
        Task<HttpStatusCode> DownloadFileAsync(string bucketName, string objectName, string filePath);
        Task<HttpStatusCode> DeleteFileAsync(string bucketName, string objectName);
        Task<HttpStatusCode> CreateBucketAsync(string bucketName);
        Task<HttpStatusCode> DeleteBucketAsync(string bucketName);
        Task<List<S3Object>> ListFilesAsync(string bucketName);
        Task<List<S3Bucket>> ListBucketsAsync();
    }
}