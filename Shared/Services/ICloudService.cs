﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace SharedLibrary.Services
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