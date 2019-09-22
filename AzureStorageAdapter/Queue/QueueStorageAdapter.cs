//-----------------------------------------------------------------------
// <copyright file="QueueStorageAdapter.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace AzureStorageAdapter.Queue
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    /// <summary>
    /// Azure Storage Queue Adapter handling the most important operations
    /// </summary>
    public class QueueStorageAdapter
    {
        /// <summary>
        /// The Azure Queue Storage Client
        /// </summary>
        private readonly CloudQueueClient queueClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueStorageAdapter"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public QueueStorageAdapter(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            queueClient = storageAccount.CreateCloudQueueClient();

            queueClient.DefaultRequestOptions = new QueueRequestOptions
            {
                RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(3), 4),
                LocationMode = LocationMode.PrimaryThenSecondary,
                MaximumExecutionTime = TimeSpan.FromSeconds(20)
            };
        }

        /// <summary>
        /// Creates a new queue asynchronous.
        /// </summary>
        /// <param name="name">The name of the Queue.</param>
        /// <returns></returns>
        public async Task CreateQueueAsync(string name)
        {
            CloudQueue queue = queueClient.GetQueueReference(name);
            await queue.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Adds an new entry to the queue asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="messageText">The queue entry message text.</param>
        /// <returns></returns>
        public async Task AddEntryToQueueAsync(string queueName, string messageText)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            CloudQueueMessage message = new CloudQueueMessage(messageText);
            await queue.AddMessageAsync(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Peeks the next queue message asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Returns the Message Text of the next Queue Message Item as <see cref="string"/></returns>
        public async Task<string> PeekNextMessageStringAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            CloudQueueMessage peekedMessage = await queue.PeekMessageAsync().ConfigureAwait(false);

            return peekedMessage.AsString;
        }

        /// <summary>
        /// Peeks the next queue message asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Returns the next Queue Message Item</returns>
        public async Task<CloudQueueMessage> PeekNextMessageAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            CloudQueueMessage peekedMessage = await queue.PeekMessageAsync().ConfigureAwait(false);

            return peekedMessage;
        }

        /// <summary>
        /// Changes the message content asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="messageContent">Content of the message.</param>
        /// <returns></returns>
        public async Task ChangeMessageContentAsync(string queueName, string messageContent)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            CloudQueueMessage message = await queue.GetMessageAsync().ConfigureAwait(false);

            message.SetMessageContent(messageContent);
            await queue.UpdateMessageAsync(message, TimeSpan.FromSeconds(20.0), MessageUpdateFields.Content | MessageUpdateFields.Visibility);
        }

        /// <summary>
        /// Removes the next message asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public async Task RemoveNextMessageAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            CloudQueueMessage retrievedMessage = await queue.GetMessageAsync().ConfigureAwait(false);

            await queue.DeleteMessageAsync(retrievedMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the queue length asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Returns the length <see cref="int"/> of the queue.</returns>
        public async Task<int> GetQueueLengthAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            // Fetch the queue attributes.
            await queue.FetchAttributesAsync().ConfigureAwait(false);

            // Retrieve the cached approximate message count.
            return queue.ApproximateMessageCount.Value;
        }

        /// <summary>
        /// Deletes the queue asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        public async Task DeleteQueueAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            // Delete the queue.
            await queue.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Returns true if the queue exists.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Returns true if the Queue exists.</returns>
        public async Task<bool> QueueExistsAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            return await queue.ExistsAsync().ConfigureAwait(false);
        }
    }
}
