namespace UnitTest
{
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

            message.AsString.Should().Equals("test");
        }

    }
}
