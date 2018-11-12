namespace AzureStorageAdapter.Table
{
    using Microsoft.WindowsAzure.Storage.Queue;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    interface IQueueStorageAdapter
    {
        Task CreateQueueAsync(string name);

        Task AddEntryToQueueAsync(string queueName, string messageText);

        Task<string> PeekNextMessageStringAsync(string queueName);

        Task<CloudQueueMessage> PeekNextMessageAsync(string queueName);

        Task ChangeMessageContentAsync(string queueName, string messageContent);

        Task RemoveNextMessageAsync(string queueName);

        Task<int> GetQueueLengthAsync(string queueName);

        Task DeleteQueueAsync(string queueName);
    }
}
