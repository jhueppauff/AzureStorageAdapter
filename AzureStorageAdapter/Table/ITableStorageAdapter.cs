//-----------------------------------------------------------------------
// <copyright file="ITableStorageAdapter.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace AzureStorageAdapter.Table
{
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Interface of the <see cref="TableStorageAdapter"/> Class.
    /// </summary>
    public interface ITableStorageAdapter
    {
        /// <summary>
        /// Inserts the record to table.
        /// </summary>
        /// <typeparam name="TTableEntity">The type of the table entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="autoCreateTable">if set to <c>true</c> [a table will be created, when it does not exists]</param>
        /// <param name="throwErrorOnExistingRecord">if set to <c>true</c> [throw error on existing record].</param>
        /// <returns>Returns <see cref="Task{void}"/></returns>
        Task InsertRecordToTable<TTableEntity>(string tableName, TTableEntity entity, bool autoCreateTable, bool throwErrorOnExistingRecord = false) where TTableEntity : TableEntity, new();

        /// <summary>
        /// Creates a new table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Returns <see cref="Task{void}"/></returns>
        Task CreateNewTable(string tableName);

        /// <summary>
        /// Retrieves the record.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns <see cref="Task{TResponse}"/></returns>
        Task<TResponse> RetrieveRecord<TResponse>(string tableName, TableEntity entity) where TResponse : TableEntity, new();

        /// <summary>
        /// Deletes a Table
        /// </summary>
        /// <param name="tableName">The Name of the Table to delete</param>
        /// <returns>Returns <see cref="Task{void}"/></returns>
        Task DeleteTableAsync(string tableName);

        /// <summary>
        /// Checks if a Table Exists
        /// </summary>
        /// <param name="tableName">The Name of the Table to check</param>
        /// <returns>Returns <see cref="Task{bool}"/></returns>
        Task<bool> TableExists(string tableName);
    }
}