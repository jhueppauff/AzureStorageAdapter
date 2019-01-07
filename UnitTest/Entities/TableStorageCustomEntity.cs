using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Entities
{
    public class TableStorageCustomEntity : TableEntity
    {
        public TableStorageCustomEntity()
        {
        }

        public TableStorageCustomEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public string CustomString { get; set; }

        public int CustomInt { get; set; }

        public DateTime CustomDateTime { get; set; }

        public string NullValue { get; set; }
    }
}
