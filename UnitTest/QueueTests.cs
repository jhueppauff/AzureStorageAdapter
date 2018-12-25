//-----------------------------------------------------------------------
// <copyright file="QueueTests.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace UnitTest
{
    using AzureStorageAdapter.Queue;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Threading.Tasks;

    [TestClass]
    public class QueueTests
    {
        private IConfiguration configuration;
        private const string QueueName = "test";
        private QueueStorageAdapter queue;

        [TestInitialize]
        public async Task Initialize()
        {
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

        [TestCleanup]
        public async Task Dispose()
        {
            await this.queue.DeleteQueueAsync(QueueName);
            this.queue = null;
            this.configuration = null;
        }

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false).Build();
        }
    }
}