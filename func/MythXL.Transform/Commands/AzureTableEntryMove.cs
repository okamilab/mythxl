using CommandLine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace MythXL.Transform.Commands
{
    [Verb("az-te-move", HelpText = "Move storage table entry to another table.")]
    public class AzureTableEntryMoveOptions
    {
        [Option('c', "connection-string", Required = true, HelpText = "Connection string to Azure Storage.")]
        public string Connection { get; set; }

        [Option('s', "source", Required = true, HelpText = "Source table.")]
        public string Source { get; set; }

        [Option('t', "target", Required = true, HelpText = "Target table.")]
        public string Target { get; set; }

        [Option('b', "batchSize", Required = false, HelpText = "Batch size.", Default = 10)]
        public int BatchSize { get; set; }
    }

    public static class AzureTableEntryMove
    {
        public static int RunAddAndReturnExitCode(AzureTableEntryMoveOptions options)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(options.Connection);
            var client = storageAccount.CreateCloudTableClient();
            var fromTable = client.GetTableReference(options.Source);
            var toTable = client.GetTableReference(options.Target);

            while (true)
            {
                var query = new TableQuery<TableEntity> { TakeCount = options.BatchSize };
                var segment = fromTable.ExecuteQuerySegmentedAsync(query, null).Result;
                if (segment.Results.Count == 0)
                {
                    break;
                }
                foreach (var entry in segment.Results)
                {
                    TableOperation insertOperation = TableOperation.InsertOrReplace(entry);
                    toTable.ExecuteAsync(insertOperation).Wait();

                    entry.ETag = "*";
                    TableOperation deleteOperation = TableOperation.Delete(entry);
                    fromTable.ExecuteAsync(deleteOperation).Wait();

                    Console.WriteLine($"Moved {entry.PartitionKey}");
                }
            }

            return 0;
        }
    }
}
