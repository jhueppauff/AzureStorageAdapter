using AzureStorageAdapter.Table;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public async Task CreateTable()
        {
            var configuration = GetConfiguration();
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);

            await tableStorageAdapter.CreateNewTable("test").ConfigureAwait(false);

            await tableStorageAdapter.DeleteTableAsync("test").ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertTable()
        {
            var configuration = GetConfiguration();
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);
            var entity = new TableEntity() { PartitionKey = "partkey", RowKey = "rowkey" };
            await tableStorageAdapter.CreateNewTable("test").ConfigureAwait(false);

            await tableStorageAdapter.InsertRecordToTable("test", entity);
            var result = await tableStorageAdapter.RetrieveRecord<TableEntity>("test", entity);

            result.Should().NotBeNull();
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
