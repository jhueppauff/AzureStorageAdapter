﻿namespace UnitTest
{
    using System;
    using System.Threading.Tasks;
    using AzureStorageAdapter.Table;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage.Table;

    [TestClass]
    public class TableTests
    {
        private IConfiguration configuration;

        [TestInitialize]
        public void Initialize()
        {
            this.configuration = Configuration.GetConfiguration();
        }

        [TestMethod]
        public async Task CreateAndDeleteTable()
        {
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);
            string tableName = "createtest" + DateTime.Now.Second;

            await tableStorageAdapter.CreateNewTable(tableName).ConfigureAwait(false);

            var exists = await tableStorageAdapter.TableExits(tableName);

            exists.Should().Equals(true);

            await tableStorageAdapter.DeleteTableAsync(tableName).ConfigureAwait(false);

            exists = await tableStorageAdapter.TableExits(tableName);

            exists.Should().Equals(false);
        }

        [TestMethod]
        public async Task TableExits_Should_Return_True_If_Table_Exits()
        {
            string tableName = "existstest" + DateTime.Now.Second;
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);

            await tableStorageAdapter.CreateNewTable(tableName).ConfigureAwait(false);

            var exists = await tableStorageAdapter.TableExits(tableName);
            exists.Should().Equals(true);

            await tableStorageAdapter.DeleteTableAsync(tableName).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task TableExits_Should_Return_False_If_Table_Does_Not_Exits()
        {
            string tableName = "notexiststest" + DateTime.Now.Second;
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);

            var exists = await tableStorageAdapter.TableExits(tableName);
            exists.Should().Equals(false);
        }

        [TestMethod]
        public async Task InsertTable()
        {
            string tableName = "inserttest" + DateTime.Now.Second;
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);
            var entity = new TableEntity() { PartitionKey = "partkey", RowKey = "rowkey" };
            await tableStorageAdapter.CreateNewTable(tableName).ConfigureAwait(false);

            await tableStorageAdapter.InsertRecordToTable(tableName, entity).ConfigureAwait(false);
            var result = await tableStorageAdapter.RetrieveRecord<TableEntity>(tableName, entity).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(TableEntity));
            result.Should().Equals(entity);

            await tableStorageAdapter.DeleteTableAsync(tableName).ConfigureAwait(false);
        }
    }
}
