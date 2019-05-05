using Microsoft.WindowsAzure.Storage;
using System;
using System.Threading.Tasks;

namespace MythXL.Func.Utils
{
    internal static class Blob
    {
        public static async Task Write(string connection, string container, string blobName, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content), $"Connection: {connection}, Container: {container}, BlobName: {blobName}");
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(container);
            await blobContainer.CreateIfNotExistsAsync();

            var cloudBlockBlob = blobContainer.GetBlockBlobReference(blobName);
            await cloudBlockBlob.UploadTextAsync(content);
        }
    }
}
