namespace UnitTest
{
    using System.Threading.Tasks;
//-----------------------------------------------------------------------
// <copyright file="QueueTests.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace UnitTest
{
    using System;
    using System.Threading.Tasks;
    using AzureStorageAdapter.Queue;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueueTests
    {
        private const string queueName = "sdktest";
        private QueueStorageAdapter queueStorageAdapter;
        private IConfiguration configuration;

        [TestInitialize]
        public async Task Initialize()
        {
            this.configuration = Configuration.GetConfiguration();
            this.queueStorageAdapter = new QueueStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);
            await this.queueStorageAdapter.CreateQueueAsync(queueName);
        }

        [TestMethod]
        public async Task AddEntry()
        {
            await queueStorageAdapter.AddEntryToQueueAsync(queueName, "test").ConfigureAwait(false);

            var message = await queueStorageAdapter.PeekNextMessageAsync(queueName).ConfigureAwait(false);
            this.configuration = this.GetConfiguration();
            this.queue = new QueueStorageAdapter(this.configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);
            await this.queue.CreateQueueAsync(QueueName);
        }

        [TestMethod]
        public async Task QueueMessage()
        {
            await this.queue.AddEntryToQueueAsync(QueueName, "test");

            var message = await this.queue.PeekNextMessageAsync(QueueName);

            message.AsString.Should().Equals("test");
        }

        [TestMethod]
        public async Task GetQueueCount()
        {
            for (int i = 0; i < 10; i++)
            {
                await queueStorageAdapter.AddEntryToQueueAsync(queueName, i.ToString()).ConfigureAwait(false);
            }
            
            var queueLength = await queueStorageAdapter.GetQueueLengthAsync(queueName).ConfigureAwait(false);

            queueLength.Should().Equals(10);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await queueStorageAdapter.DeleteQueueAsync(queueName).ConfigureAwait(false);
        }
    }
}
