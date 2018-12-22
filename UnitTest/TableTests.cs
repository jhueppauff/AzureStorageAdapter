using AzureStorageAdapter.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class TableTests
    {
        [TestMethod]
        public async Task CreateTable()
        {
            var configuration = GetConfiguration();
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);

            await tableStorageAdapter.CreateNewTable("test").ConfigureAwait(false);

            await tableStorageAdapter.DeleteTableAsync("test").ConfigureAwait(false);
        }

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false).Build();
        }
    }
}
