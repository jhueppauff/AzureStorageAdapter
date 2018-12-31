//-----------------------------------------------------------------------
// <copyright file="BlobTests.cs" company="https://github.com/jhueppauff/AzureStorageAdapter">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/AzureStorageAdapter/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace UnitTest
{
    using AzureStorageAdapter.Blob;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    [TestClass]
    public class BlobTests
    {
        private IConfiguration configuration;
        private BlobStorageAdapter blobStorageAdapter;

        [TestInitialize]
        public void Initialize()
        {
            configuration = GetConfiguration();
            blobStorageAdapter = new BlobStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);
        }

        [TestMethod]
        public async Task TestBlobUpload()
        {
            string sample = System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName + @"\Resources\sample.txt";

            var configuration = Configuration.GetConfiguration(); 
            BlobStorageAdapter blobStorageAdapter = new BlobStorageAdapter(configuration.GetSection("AzureBlogStorage:BlobConnectionString").Value);

            string uri = await blobStorageAdapter.UploadToBlob(await File.ReadAllBytesAsync(sample), Guid.NewGuid() + ".txt", "text/plain", configuration.GetSection("AzureBlogStorage:BlobContainer").Value, true);

            Assert.IsTrue(!String.IsNullOrWhiteSpace(uri));

            string path = System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName + @"\download.txt";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var client = new WebClient())
            {
                client.DownloadFile(uri, path);
            }

            Assert.IsTrue(File.Exists(path));
            var fileInfo = new FileInfo(path);

            Assert.IsTrue(fileInfo.Length != 0);
        }

        [TestCleanup]
        public void Dispose()
        {
            blobStorageAdapter = null;
            configuration = null;
        }

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false).Build();
        }
    }
}