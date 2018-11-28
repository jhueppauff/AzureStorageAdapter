//-----------------------------------------------------------------------
// <copyright file="IQueueStorageAdapter.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace AzureStorageAdapter.Queue
{
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// Azure Storage Queue Adapter handling the most important operations
    /// </summary>
    public interface IQueueStorageAdapter
    {
        /// <summary>
        /// Creates a new queue asynchronous.
        /// </summary>
        /// <param name="name">The name of the Queue.</param>
        /// <returns></returns>
        Task CreateQueueAsync(string name);

        /// <summary>
        /// Adds an new entry to the queue asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="messageText">The queue entry message text.</param>
        /// <returns></returns>
        Task AddEntryToQueueAsync(string queueName, string messageText);

        /// <summary>
        /// Peeks the next queue message asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Returns the Message Text of the next Queue Message Item as <see cref="string"/></returns>
        Task<string> PeekNextMessageStringAsync(string queueName);

        /// <summary>
        /// Peeks the next queue message asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Returns the next Queue Message Item</returns>
        Task<CloudQueueMessage> PeekNextMessageAsync(string queueName);

        /// <summary>
        /// Changes the message content asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="messageContent">Content of the message.</param>
        /// <returns></returns>
        Task ChangeMessageContentAsync(string queueName, string messageContent);

        /// <summary>
        /// Removes the next message asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        Task RemoveNextMessageAsync(string queueName);

        /// <summary>
        /// Gets the queue length asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Returns the length <see cref="int"/> of the queue.</returns>
        Task<int> GetQueueLengthAsync(string queueName);

        /// <summary>
        /// Deletes the queue asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        Task DeleteQueueAsync(string queueName);
    }
}
