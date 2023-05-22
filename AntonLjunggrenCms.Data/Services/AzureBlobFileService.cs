using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonLjunggrenCms.Data.Services
{
    public sealed class AzureBlobFileService : IFileService
    {
        private readonly string _blobConnectionString;
        private readonly string _blobContainerName;

        public AzureBlobFileService(string blobConnectionString, string blobContainerName)
        {
            _blobConnectionString = blobConnectionString;
            _blobContainerName = blobContainerName;
        }

        public async Task<bool> FileExists(string filePath)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_blobConnectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(filePath);

            var exist = await blobClient.ExistsAsync();

            return exist;
        }

        public async Task<Stream> GetFileContentStream(string filePath, CancellationToken cancellation = default)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_blobConnectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(filePath);

            var exist = await blobClient.ExistsAsync();

            if (!exist)
            {
                throw new FileNotFoundException($"File [{filePath}] not found");
            }

            var content = await blobClient.OpenReadAsync().ConfigureAwait(false);

            return content;
        }

        public async Task<string> SaveFile(Stream stream, string contentType, CancellationToken cancellation = default)
        {
            stream.Position = 0;

            BlobServiceClient blobServiceClient = new BlobServiceClient(_blobConnectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);

            try
            {
                string trustedFileNameForFileStorage = Path.ChangeExtension(Path.GetRandomFileName(), "file");
                BlobClient blobClient = blobContainerClient.GetBlobClient(trustedFileNameForFileStorage);

                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });

                return trustedFileNameForFileStorage;
            }
            catch (Exception e)
            {
                throw new Exception("Error uploading blob", e.InnerException);
            }
            finally
            {
                stream.Close();
            }
        }
    }
}
