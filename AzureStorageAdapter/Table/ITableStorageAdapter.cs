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

    public interface ITableStorageAdapter
    {
        /// <summary>
        /// Inserts the record to table.
        /// </summary>
        /// <typeparam name="TTableEntity">The type of the table entity.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="throwErrorOnExistingRecord">if set to <c>true</c> [throw error on existing record].</param>
        /// <returns></returns>
        Task InsertRecordToTable<TTableEntity>(string tableName, TTableEntity entity, bool throwErrorOnExistingRecord = false);

        /// <summary>
        /// Creates a new table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        Task CreateNewTable(string tableName);

        /// <summary>
        /// Retrieves the record.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task<TResponse> RetrieveRecord<TResponse>(string tableName, TableEntity entity);

        /// <summary>
        /// Deletes a Table
        /// </summary>
        /// <param name="tableName">The Name of the Table to delete</param>
        /// <returns>Returns <see cref="Task{void}"/></returns>
        Task DeleteTableAsync(string tableName);
    }
}