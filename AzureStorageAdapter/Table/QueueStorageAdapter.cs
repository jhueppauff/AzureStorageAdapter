//-----------------------------------------------------------------------
// <copyright file="QueueStorageAdapter.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace AzureStorageAdapter.Table
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// Azure Storage Queue Adapter handling the most important operations
    /// </summary>
    public class QueueStorageAdapter
    {
        private readonly CloudQueueClient queueClient;

        public QueueStorageAdapter(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            queueClient = storageAccount.CreateCloudQueueClient();
        }

        public async Task CreateQueueAsync(string name)
        {
            CloudQueue queue = queueClient.GetQueueReference(name);
            await queue.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        public async Task AddEntryToQueueAsync(string queueName, string messageText)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            CloudQueueMessage message = new CloudQueueMessage(messageText);
            await queue.AddMessageAsync(message).ConfigureAwait(false);
        }

        public async Task<string> PeekNextMessageStringAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            CloudQueueMessage peekedMessage = await queue.PeekMessageAsync().ConfigureAwait(false);

            return peekedMessage.AsString;
        }

        public async Task<CloudQueueMessage> PeekNextMessageAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            CloudQueueMessage peekedMessage = await queue.PeekMessageAsync().ConfigureAwait(false);

            return peekedMessage;
        }

        public async Task ChangeMessageContentAsync(string queueName, string messageContent)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            CloudQueueMessage message = await queue.GetMessageAsync().ConfigureAwait(false);

            message.SetMessageContent(messageContent);
            await queue.UpdateMessageAsync(message, TimeSpan.FromSeconds(20.0), MessageUpdateFields.Content | MessageUpdateFields.Visibility);
        }

        public async Task RemoveNextMessageAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            CloudQueueMessage retrievedMessage = await queue.GetMessageAsync().ConfigureAwait(false);

            await queue.DeleteMessageAsync(retrievedMessage).ConfigureAwait(false);
        }

        public async Task<int> GetQueueLengthAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            // Fetch the queue attributes.
            await queue.FetchAttributesAsync().ConfigureAwait(false);

            // Retrieve the cached approximate message count.
            return queue.ApproximateMessageCount.Value;
        }

        public async Task DeleteQueueAsync(string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            // Delete the queue.
            await queue.DeleteIfExistsAsync().ConfigureAwait(false);
        }
    }
}
