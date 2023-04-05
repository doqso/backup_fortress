using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowApp
{
    public interface CloudService
    {
        public Task<bool> UploadFileAsync(string bucketName, string objectName, string filePath);
        void DownloadFile(string filePath);
        void DeleteFile(string filePath);
        void ListFiles();
    }
}