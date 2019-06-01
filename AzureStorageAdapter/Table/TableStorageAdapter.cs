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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
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

            cloudTableClient.DefaultRequestOptions = new TableRequestOptions
            {
                RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(3), 4),
                LocationMode = LocationMode.PrimaryThenSecondary,
                MaximumExecutionTime = TimeSpan.FromSeconds(20)
            };
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
        /// Executes a Insert Batch Operation.
        /// </summary>
        /// <typeparam name="TTableEntity">Type of the Table Entity</typeparam>
        /// <param name="tableName">Name of the Table</param>
        /// <param name="entities">Array of the Entities to add to the Insert Operation</param>
        /// <param name="merge">If set true, Entities will be merged</param>
        /// <returns>Returns <see cref="Task{void}"/></returns>
        /// <returns></returns>
        public async Task ExcuteBatchOperationToTable<TTableEntity>(string tableName, TTableEntity[] entities, bool merge = false) where TTableEntity : TableEntity, new()
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);

            if (entities is TableEntity[])
            {
                // Create the batch operation.
                TableBatchOperation batchOperation = new TableBatchOperation();

                foreach (var entity in entities)
                {
                    if (merge)
                    {
                        batchOperation.InsertOrMerge(entity);
                    }
                    else
                    {
                        batchOperation.InsertOrReplace(entity);
                    }
                }

                await cloudTable.ExecuteBatchAsync(batchOperation).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets all Rows
        /// </summary>
        /// <typeparam name="TResponse">The type of the table entity.</typeparam>
        /// <param name="tableName">Name of the Table</param>
        /// <returns></returns>
        public async Task<List<TResponse>> GetAll<TResponse>(string tableName) where TResponse : TableEntity, new()
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);
            TableContinuationToken token = null;
            var entities = new List<TResponse>();

            do
            {
                var queryResult = await cloudTable.ExecuteQuerySegmentedAsync(new TableQuery<TResponse>(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
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
        public async Task InsertRecordToTable<TTableEntity>(string tableName, TTableEntity entity, bool throwErrorOnExistingRecord = false) where TTableEntity : TableEntity, new()
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);

            if (entity is TableEntity)
            {
                var retrievedEntity = await RetrieveRecord<TTableEntity>(tableName, entity as TableEntity).ConfigureAwait(false);

                if (retrievedEntity == null)
                {
                    TableOperation tableOperation = TableOperation.InsertOrReplace(entity as TableEntity);
                    await cloudTable.ExecuteAsync(tableOperation).ConfigureAwait(false);
                }
                else if (throwErrorOnExistingRecord)
                {
                    throw new ArgumentException("Provided entity does already exist");
                }
                else
                {
                    TableOperation tableOperation = TableOperation.Replace(entity as TableEntity);
                    await cloudTable.ExecuteAsync(tableOperation).ConfigureAwait(false);
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
        public async Task<TResponse> RetrieveRecord<TResponse>(string tableName, TableEntity entity) where TResponse : TableEntity, new()
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);

            TableOperation tableOperation = TableOperation.Retrieve<TResponse>(entity.PartitionKey, entity.RowKey, null);
            TableResult tableResult = await cloudTable.ExecuteAsync(tableOperation).ConfigureAwait(false);

            // Issue is here 
            if (tableResult.HttpStatusCode == 404)
            {
                return default(TResponse);
            }

            return (TResponse)tableResult.Result;
        }

        /// <summary>
        /// Checks if a Table Exists
        /// </summary>
        /// <param name="tableName">The Name of the Table to check</param>
        /// <returns>Returns <see cref="Task{bool}"/></returns>
        public async Task<bool> TableExits(string tableName)
        {
            CloudTable cloudTable = cloudTableClient.GetTableReference("test");
            return await cloudTable.ExistsAsync().ConfigureAwait(false);
        }
    }
}