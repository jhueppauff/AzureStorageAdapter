﻿namespace AzureStorageAdapter
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Blob Storage Adpater for Executing File Operations on the Azure Blob Storage
    /// </summary>
    public class BlobStorageAdapter
    {
        /// <summary>
        /// The default shared access expiry time
        /// </summary>
        private const int defaultSharedAccessExpiryTime = 30;

        /// <summary>
        /// The library container
        /// </summary>
        private readonly CloudBlobContainer libraryContainer;

        /// <summary>
        /// Default SharedAccess Expiry Time for calls, overridable by subclasses
        /// </summary>
        protected virtual int DefaultSharedAccessExpiryTime => defaultSharedAccessExpiryTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageAdapter"/> class.
        /// </summary>
        /// <param name="blobConnectionString">The BLOB connection string.</param>
        /// <param name="containerName">Name of the container.</param>
        public BlobStorageAdapter(string blobConnectionString, string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            libraryContainer = blobClient.GetContainerReference(containerName);
        }

        /// <summary>
        /// Uploads to BLOB Storage.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="name">The name.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        public Task<string> UploadToBlob(byte[] data, string name, string contentType, bool overwrite = false)
        {
            return UploadToBlob(new MemoryStream(data), name, contentType);
        }

        /// <summary>
        /// Uploads to BLOB Storage.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="name">The name.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        public Task<string> UploadToBlob(string data, string name, string contentType, bool overwrite = false)
        {
            return UploadToBlob(new MemoryStream(Convert.FromBase64String(data)), name, contentType);
        }

        /// <summary>
        /// Uploads to BLOB Storage.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="name">The name.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        private async Task<string> UploadToBlob(Stream stream, string name, string contentType)
        {
            CloudBlockBlob blockBlob = libraryContainer.GetBlockBlobReference(name);
            if (!await blockBlob.ExistsAsync())
            {
                await blockBlob.UploadFromStreamAsync(stream);
                blockBlob.Properties.ContentType = contentType;
                await blockBlob.SetPropertiesAsync();

            }

            string blobUri = blockBlob.Uri.ToString();
            string blobSAS = blockBlob.GetSharedAccessSignature(GetSAS());

            return $"{blobUri}{blobSAS}";
        }

        /// <summary>
        /// Gets the SAS Token.
        /// </summary>
        /// <returns>Returns the <see cref="SharedAccessBlobPolicy"/></returns>
        private SharedAccessBlobPolicy GetSAS()
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(DefaultSharedAccessExpiryTime),
                Permissions = SharedAccessBlobPermissions.Read
            };

            return sasConstraints;
        }
    }
}
