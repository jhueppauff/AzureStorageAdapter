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

        [TestMethod]
        public async Task QueueExists()
        {
            var result = await queueStorageAdapter.QueueExistsAsync("dontexists");

            result.Should().BeFalse();

            await queueStorageAdapter.CreateQueueAsync("exists");
            var result2 = await queueStorageAdapter.QueueExistsAsync("exists");

            result2.Should().BeTrue();

            await queueStorageAdapter.DeleteQueueAsync("exists");
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await queueStorageAdapter.DeleteQueueAsync(queueName).ConfigureAwait(false);
        }
    }
}
