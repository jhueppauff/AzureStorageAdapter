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

    /// <summary>
    /// Table not found Exception
    /// </summary>
    public class TableDoesNotExistsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableDoesNotExistsException"/> class.
        /// </summary>
        public TableDoesNotExistsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDoesNotExistsException"/> class.
        /// </summary>
        /// <param name="message">The Message Text to Include in the Exception</param>
        public TableDoesNotExistsException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDoesNotExistsException"/> class.
        /// </summary>
        /// <param name="message">The Message Text to Include in the Exception</param>
        /// <param name="innerException">The Inner Exception to Include in the Exception</param>
        public TableDoesNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDoesNotExistsException"/> class.
        /// </summary>
        /// <param name="info">Serialization Info to include in the Exception</param>
        /// <param name="context">Streaming Context to include in the Exception</param>
        protected TableDoesNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}