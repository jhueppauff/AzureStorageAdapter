//-----------------------------------------------------------------------
// <copyright file="TableStorageAdapter.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
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
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Azure Table Storage Adapter handling the most important operations
    /// </summary>
    /// <seealso cref="AzureStorageAdapter.Table.ITableStorageAdapter" />
    public class TableStorageAdapter : ITableStorageAdapter
    {
        private readonly CloudTableClient cloudTableClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorageAdapter"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public TableStorageAdapter(string connectionString)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Creates a new table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public async Task CreateNewTable(string tableName)
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);
            await cloudTable.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a Table
        /// </summary>
        /// <param name="tableName">The Name of the Table to delete</param>
        /// <returns>Returns <see cref="Task{void}"/></returns>
        public async Task DeleteTableAsync(string tableName)
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);

            await cloudTable.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Inserts the record to table.
        /// </summary>
        /// <typeparam name="TTableEntity">The type of the table entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="throwErrorOnExistingRecord">if set to <c>true</c> [throw error on existing record].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// Provided entity does already exist
        /// or
        /// Provided entity is not of the type TableEntity
        /// </exception>
        public async Task InsertRecordToTable<TTableEntity>(string tableName, TTableEntity entity, bool throwErrorOnExistingRecord = false)
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);

            if (entity is TableEntity)
            {
                var retrievedEntity = RetrieveRecord<TTableEntity>(tableName, entity as TableEntity);

                if (retrievedEntity == null)
                {
                    TableOperation tableOperation = TableOperation.Insert(entity as TableEntity);
                    await cloudTable.ExecuteAsync(tableOperation).ConfigureAwait(false);
                }
                else if (throwErrorOnExistingRecord)
                {
                    throw new ArgumentException("Provided entity does already exist");
                }
            }
            else
            {
                throw new ArgumentException("Provided entity is not of the type TableEntity");
            }
        }

        /// <summary>
        /// Retrieves the record.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task<TResponse> RetrieveRecord<TResponse>(string tableName, TableEntity entity)
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);

            TableOperation tableOperation = TableOperation.Retrieve(entity.PartitionKey, entity.RowKey);
            TableResult tableResult = await cloudTable.ExecuteAsync(tableOperation).ConfigureAwait(false);

            return (TResponse)tableResult.Result;
        }
    }
}
