//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace AzureStorageAdapter.Table.Exceptions
{
    /// <summary>
    /// Table Constants 
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Message Text used in the Table Storage Client
        /// </summary>
        public const string TableDoesNotExistsExceptionMessage = "Provided Table \"{0}\" not found! Please provide a valid Table Name or validate the Storage ConnectionString";
    }
}