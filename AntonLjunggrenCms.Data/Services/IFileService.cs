using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonLjunggrenCms.Data.Services
{
    public interface IFileService
    {
        public Task<bool> FileExists(string filePath);

        public Task<Stream> GetFileContentStream(string filePath, CancellationToken cancellation = default);

        public Task<string> SaveFile(Stream stream, string contentType, CancellationToken cancellation = default);
    }
}
