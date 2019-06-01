namespace UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AzureStorageAdapter.Table;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage.Table;
    using UnitTest.Entities;

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

        [TestMethod]
        public async Task Insert_CustomEntity_Into_Table()
        {
            string tableName = "insertcustomtest" + DateTime.Now.Second;
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);

            TableStorageCustomEntity customEntity = new TableStorageCustomEntity("partkey", "rowkey")
            {
                CustomInt = 1,
                CustomString = "string",
                CustomDateTime = DateTime.Now
            };

            await tableStorageAdapter.CreateNewTable(tableName).ConfigureAwait(false);

            await tableStorageAdapter.InsertRecordToTable(tableName, customEntity).ConfigureAwait(false);
            var result = await tableStorageAdapter.RetrieveRecord<TableStorageCustomEntity>(tableName, customEntity).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(TableStorageCustomEntity));
            result.Should().Equals(customEntity);

            await tableStorageAdapter.DeleteTableAsync(tableName).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Insert_Batch_CustomEntity_Into_Table()
        {
            string tableName = "insertbatchcustomtest" + DateTime.Now.Second;
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);

            TableStorageCustomEntity customEntity = new TableStorageCustomEntity("partkey", "rowkey")
            {
                CustomInt = 1,
                CustomString = "string",
                CustomDateTime = DateTime.Now
            };

            TableStorageCustomEntity customEntity2 = new TableStorageCustomEntity("partkey", "rowkey1")
            {
                CustomInt = 2,
                CustomString = "string",
                CustomDateTime = DateTime.Now
            };

            TableStorageCustomEntity[] entities = new TableStorageCustomEntity[2];
            entities[0] = customEntity;
            entities[1] = customEntity2;

            await tableStorageAdapter.CreateNewTable(tableName).ConfigureAwait(false);

            await tableStorageAdapter.ExcuteBatchOperationToTable(tableName, entities).ConfigureAwait(false);

            var result = await tableStorageAdapter.RetrieveRecord<TableStorageCustomEntity>(tableName, customEntity).ConfigureAwait(false);
            var result2 = await tableStorageAdapter.RetrieveRecord<TableStorageCustomEntity>(tableName, customEntity).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(TableStorageCustomEntity));
            result.Should().Equals(customEntity);

            result2.Should().NotBeNull();
            result2.Should().BeOfType(typeof(TableStorageCustomEntity));
            result2.Should().Equals(customEntity);

            await tableStorageAdapter.DeleteTableAsync(tableName).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Update_Record()
        {
            string tableName = "updatetest" + DateTime.Now.Second;
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);

            TableStorageCustomEntity customEntity = new TableStorageCustomEntity("partkey", "rowkey")
            {
                CustomInt = 1,
                CustomString = "string",
                CustomDateTime = DateTime.Now
            };

            await tableStorageAdapter.CreateNewTable(tableName).ConfigureAwait(false);

            await tableStorageAdapter.InsertRecordToTable(tableName, customEntity).ConfigureAwait(false);

            customEntity.CustomString = "newstring";

            await tableStorageAdapter.InsertRecordToTable(tableName, customEntity).ConfigureAwait(false);

            var result = await tableStorageAdapter.RetrieveRecord<TableStorageCustomEntity>(tableName, customEntity).ConfigureAwait(false);
            result.CustomString.Should().Equals("newstring");

            await tableStorageAdapter.DeleteTableAsync(tableName).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAll()
        {
            string tableName = "all" + DateTime.Now.Second;

            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);

            TableStorageCustomEntity[] entities = new TableStorageCustomEntity[10];

            for (int i = 0; i < 10; i++)
            {
                TableStorageCustomEntity customEntity = new TableStorageCustomEntity("partkey", Guid.NewGuid().ToString())
                {
                    CustomInt = 1,
                    CustomString = "string",
                    CustomDateTime = DateTime.Now
                };

                entities[i] = customEntity;
            }

            await tableStorageAdapter.CreateNewTable(tableName).ConfigureAwait(false);

            await tableStorageAdapter.ExcuteBatchOperationToTable<TableStorageCustomEntity>(tableName, entities).ConfigureAwait(false);

            var retrievedEntities = await tableStorageAdapter.GetAll<TableStorageCustomEntity>(tableName);

            retrievedEntities.Count.Should().Be(10);
        }
    }
}