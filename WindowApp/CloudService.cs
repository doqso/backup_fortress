using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WindowApp
{
    public interface ICloudService
    {
        Task<HttpStatusCode> UploadFileAsync(string bucketName, string objectName, string filePath);
        Task<HttpStatusCode> DownloadFileAsync(string bucketName, string objectName, string filePath);
        Task<HttpStatusCode> DeleteFileAsync(string bucketName, string objectName);
        Task<List<S3Object>> ListFilesAsync(string bucketName);
        Task<List<S3Bucket>> ListBucketsAsync();
    }
}