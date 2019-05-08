using Microsoft.WindowsAzure.Storage;
using System;
using System.Threading.Tasks;

namespace MythXL.Func.Utils
{
    internal static class Blob
    {
        public static async Task WriteAsync(string connection, string containerName, string blobName, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(
                    nameof(content),
                    $"Connection: {connection}, Container: {containerName}, BlobName: {blobName}");
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            var blockBlob = container.GetBlockBlobReference(blobName);
            await blockBlob.UploadTextAsync(content);
        }

        public static async Task<string> ReadAsync(string connection, string containerName, string blobName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            var blockBlob = container.GetBlockBlobReference(blobName);
            return await blockBlob.DownloadTextAsync();
        }
    }
}
