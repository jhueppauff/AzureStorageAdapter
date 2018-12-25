//-----------------------------------------------------------------------
// <copyright file="TableDoesNotExistsException.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace AzureStorageAdapter.Table.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class TableDoesNotExistsException : Exception
    {
        public TableDoesNotExistsException()
        {
        }

        public TableDoesNotExistsException(string message) : base(message)
        {
        }

        public TableDoesNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TableDoesNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}