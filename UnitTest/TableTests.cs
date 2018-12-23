using AzureStorageAdapter.Table;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class TableTests
    {
        [TestMethod]
        public async Task CreateAndDeleteTable()
        {
            var configuration = GetConfiguration();
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);

            await tableStorageAdapter.CreateNewTable("test").ConfigureAwait(false);

            await tableStorageAdapter.DeleteTableAsync("test").ConfigureAwait(false);

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

            CloudTable cloudTable = cloudTableClient.GetTableReference("test");
            var exists = await cloudTable.ExistsAsync().ConfigureAwait(false);

            exists.Should().Equals(false);
        }

        [TestMethod]
        public async Task InsertTable()
        {
            var configuration = GetConfiguration();
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);
            var entity = new TableEntity() { PartitionKey = "partkey", RowKey = "rowkey" };
            await tableStorageAdapter.CreateNewTable("test").ConfigureAwait(false);

            await tableStorageAdapter.InsertRecordToTable("test", entity).ConfigureAwait(false);
            var result = await tableStorageAdapter.RetrieveRecord<TableEntity>("test", entity).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(TableEntity));
            result.Should().Equals(entity);

            await tableStorageAdapter.DeleteTableAsync("test").ConfigureAwait(false);
        }

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false).Build();
        }
    }
}
