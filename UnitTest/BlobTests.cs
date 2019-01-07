using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AzureStorageAdapter.Blob;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class BlobTests
    {
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
    }
}
