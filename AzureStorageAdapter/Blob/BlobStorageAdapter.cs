//-----------------------------------------------------------------------
// <copyright file="BlobStorageAdapter.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace AzureStorageAdapter.Blob
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Blob Storage Adapter for Executing File Operations on the Azure Blob Storage
    /// </summary>
    public class BlobStorageAdapter : IBlobStorageAdapter
    {
        /// <summary>
        /// The default shared access expiry time
        /// </summary>
        private const int ConstDefaultSharedAccessExpiryTime = 30;

        /// <summary>
        /// The BLOB client
        /// </summary>
        private readonly CloudBlobClient blobClient;

        /// <summary>
        /// The prevent automatic creation of blob container
        /// </summary>
        private readonly bool preventAutoCreation = false;
 
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageAdapter"/> class.
        /// </summary>
        /// <param name="blobConnectionString">The BLOB connection string.</param>
        public BlobStorageAdapter(string blobConnectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobConnectionString);
            this.blobClient = storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageAdapter"/> class.
        /// </summary>
        /// <param name="blobConnectionString">The BLOB connection string.</param>
        /// <param name="preventAutoCreation">Disables the Autocreation of blobs</param>
        public BlobStorageAdapter(string blobConnectionString, bool preventAutoCreation)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobConnectionString);
            this.blobClient = storageAccount.CreateCloudBlobClient();
            this.preventAutoCreation = preventAutoCreation;
        }

        /// <summary>
        /// Default SharedAccess Expiry Time for calls, overridable by subclasses
        /// </summary>
        protected virtual int DefaultSharedAccessExpiryTime => ConstDefaultSharedAccessExpiryTime;

        /// <summary>
        /// Uploads to BLOB Storage.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="name">The name.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="containerName">Container name</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns>Returns <see cref="Task{String}"/></returns>
        public Task<string> UploadToBlob(byte[] data, string name, string contentType, string containerName, bool overwrite = false)
        {
            return this.UploadToBlob(new MemoryStream(data), name, contentType, containerName);
        }

        /// <summary>
        /// Uploads to BLOB Storage.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="name">The name.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="containerName">Name of the Azure Blob Container</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns>Returns <see cref="Task{String}"/>the URL with SAS Token.</returns>
        public Task<string> UploadToBlob(string data, string name, string contentType, string containerName, bool overwrite = false)
        {
            return this.UploadToBlob(new MemoryStream(Convert.FromBase64String(data)), name, contentType, containerName);
        }

        /// <summary>
        /// Destroys the BLOB.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="deleteSnapshotsOption">The delete snapshots option.</param>
        /// <param name="accessCondition">The access condition.</param>
        /// <param name="blobRequestOptions">The BLOB request options.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="containerName">Name of the Azure Blob Storage Container</param>
        /// <returns>Returns <see cref="Task"/></returns>
        public async Task DestroyBlob(string fileName, DeleteSnapshotsOption deleteSnapshotsOption, AccessCondition accessCondition, BlobRequestOptions blobRequestOptions, OperationContext operationContext, string containerName)
        {
            CloudBlobContainer blobContainer = this.blobClient.GetContainerReference(containerName);

            var blob = blobContainer.GetBlockBlobReference(fileName);
            await blob.DeleteIfExistsAsync(deleteSnapshotsOption, accessCondition, blobRequestOptions, operationContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Destroys the BLOB.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">Name of the Azure Blob Storage Container</param>
        /// <returns>Returns <see cref="Task"/></returns>
        public async Task DestroyBlob(string fileName, string containerName)
        {
            CloudBlobContainer blobContainer = this.blobClient.GetContainerReference(containerName);

            var blob = blobContainer.GetBlockBlobReference(fileName);
            await blob.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the BLOB container.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>Returns <see cref="Task"/></returns>
        public async Task DeleteBlobContainerAsync(string containerName)
        {
            CloudBlobContainer container = this.blobClient.GetContainerReference(containerName);

            await container.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Uploads to BLOB Storage.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="name">The name.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="containerName">Name of the Azure Blob Storage Container</param>
        /// <param name="createSAS">if set <c>true</c> [a SAS Token will be created]</param>
        /// <param name="overwriteSASTime">Overwrite the default SAS Token Lifetime.</param>
        /// <returns>Returns the URI with optional SAS Token as <see cref="Task{String}"/></returns>
        private async Task<string> UploadToBlob(Stream stream, string name, string contentType, string containerName, bool createSAS = true, int overwriteSASTime = 0)
        {
            CloudBlobContainer blobContainer = this.blobClient.GetContainerReference(containerName);

            if (!this.preventAutoCreation)
            {
                await blobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);
            }

            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(name);
            if (!await blockBlob.ExistsAsync().ConfigureAwait(false))
            {
                await blockBlob.UploadFromStreamAsync(stream).ConfigureAwait(false);
                blockBlob.Properties.ContentType = contentType;
                await blockBlob.SetPropertiesAsync().ConfigureAwait(false);
            }

            string blobUri = blockBlob.Uri.ToString();

            if (!createSAS)
            {
                return $"{blobUri}";
            }

            string blobSAS = blockBlob.GetSharedAccessSignature(this.GetSAS(overwriteSASTime));

            return $"{blobUri}{blobSAS}";
        }

        /// <summary>
        /// Gets the SAS Token.
        /// </summary>
        /// <param name="overwriteSASTime">If set, this will overwrite the Default SAS Token Lifetime (Value in minutes)</param>
        /// <returns>Returns the <see cref="SharedAccessBlobPolicy"/></returns>
        private SharedAccessBlobPolicy GetSAS(int overwriteSASTime = 0)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read
            };

            if (overwriteSASTime == 0)
            {
                sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(this.DefaultSharedAccessExpiryTime);
            }
            else
            {
                sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(overwriteSASTime);
            }

            return sasConstraints;
        }
    }
}
