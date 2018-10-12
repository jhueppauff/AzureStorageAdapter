using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageAdapter
{
    public class BlobStorageAdapter
    {
        private readonly CloudBlobContainer libraryContainer;

        public BlobStorageAdapter(string blobConnectionString, string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            libraryContainer = blobClient.GetContainerReference(containerName);
        }

        public Task<string> UploadToBlob(byte[] data, string name, string contentType, bool overwrite = false)
        {
            return UploadToBlob(new MemoryStream(data), name, contentType, overwrite);
        }

        public Task<string> UploadToBlob(string data, string name, string contentType, bool overwrite = false)
        {
            return UploadToBlob(new MemoryStream(Convert.FromBase64String(data)), name, contentType, overwrite);
        }

        private async Task<string> UploadToBlob(Stream stream, string name, string contentType, bool overwrite = false)
        {
            CloudBlockBlob blockBlob = libraryContainer.GetBlockBlobReference(name);
            if (!await blockBlob.ExistsAsync())
            {
                await blockBlob.UploadFromStreamAsync(stream);

                blockBlob.Properties.ContentType = contentType;
                await blockBlob.SetPropertiesAsync();
            }

            return blockBlob.Uri.ToString();
        }
    }
}
