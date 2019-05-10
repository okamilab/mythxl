using CommandLine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Data.Entities;
using System;

namespace MythXL.Transform.Commands
{
    [Verb("migrate-contract-v1", HelpText = "Migrates contract entities to version 1.")]
    public class MigrateContractV1Options
    {
        [Option('c', "connection-string", Required = true, HelpText = "Connection string to Azure Storage.")]
        public string Connection { get; set; }

        [Option('t', "table", Required = true, HelpText = "Source table.")]
        public string TableName { get; set; }

        [Option('l', "blob", Required = true, HelpText = "Blob container for code.")]
        public string BlobContainer { get; set; }

        [Option('b', "batchSize", Required = false, HelpText = "Batch size.", Default = 10)]
        public int BatchSize { get; set; }
    }

    public static class MigrateContractV1
    {
        public static int RunAddAndReturnExitCode(MigrateContractV1Options options)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(options.Connection);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(options.TableName);

            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(options.BlobContainer);
            container.CreateIfNotExistsAsync().Wait();

            TableContinuationToken token = null;
            do
            {
                var query = new TableQuery<ContractEntity> { TakeCount = options.BatchSize };
                var segment = table.ExecuteQuerySegmentedAsync(query, token).Result;

                foreach (var entry in segment.Results)
                {
                    if (entry.Code == null)
                    {
                        continue;
                    }

                    var blockBlob = container.GetBlockBlobReference(entry.PartitionKey);
                    blockBlob.UploadTextAsync(entry.Code).Wait();

                    entry.Code = null;
                    entry.Version = 1;
                    TableOperation insertOperation = TableOperation.InsertOrReplace(entry);
                    table.ExecuteAsync(insertOperation).Wait();

                    Console.WriteLine($"Migrated {entry.PartitionKey} to version 1");
                }

                token = segment.ContinuationToken;
            } while (token != null);

            return 0;
        }
    }
}
