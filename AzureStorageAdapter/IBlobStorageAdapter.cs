//-----------------------------------------------------------------------
// <copyright file="IBlobStorageAdapter.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace AzureStorageAdapter
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.Threading.Tasks;
    
    public interface IBlobStorageAdapter
    {
        /// <summary>
        /// Uploads to BLOB.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="name">The name.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="containerName">Container Name</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        Task<string> UploadToBlob(byte[] data, string name, string contentType, string containerName, bool overwrite = false);

        /// <summary>
        /// Uploads to BLOB.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="name">The name.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="containerName">Container Name</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        Task<string> UploadToBlob(string data, string name, string contentType, string containerName, bool overwrite = false);

        /// <summary>
        /// Destroys the BLOB.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="deleteSnapshotsOption">The delete snapshots option.</param>
        /// <param name="accessCondition">The access condition.</param>
        /// <param name="blobRequestOptions">The BLOB request options.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="containerName">Container Name</param>
        /// <returns></returns>
        Task DestroyBlob(string fileName, DeleteSnapshotsOption deleteSnapshotsOption, AccessCondition accessCondition, BlobRequestOptions blobRequestOptions, OperationContext operationContext, string containerName);

        /// <summary>
        /// Destroys the BLOB.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">Container Name</param>
        /// <returns></returns>
        Task DestroyBlob(string fileName, string containerName);

        /// <summary>
        /// Deletes the BLOB container.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns></returns>
        Task DeleteBlobContainerAsync(string containerName);
    }
}
