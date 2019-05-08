using CommandLine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Func.Entities;
using System;

namespace MythXL.Transform.Commands
{
    [Verb("analysis-version-fix", HelpText = "Fixes version in analysis.")]
    public class AnalysisVersionsFixOptions
    {
        [Option('c', "connection-string", Required = true, HelpText = "Connection string to Azure Storage.")]
        public string Connection { get; set; }

        [Option('t', "table", Required = true, HelpText = "Source table.")]
        public string TableName { get; set; }

        [Option('b', "batchSize", Required = false, HelpText = "Batch size.", Default = 10)]
        public int BatchSize { get; set; }
    }

    public static class AnalysisVersionsFix
    {
        public static int RunAddAndReturnExitCode(AnalysisVersionsFixOptions options)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(options.Connection);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(options.TableName);

            TableContinuationToken token = null;
            do
            {
                var query = new TableQuery<AnalysisEntity> { TakeCount = options.BatchSize };
                var segment = table.ExecuteQuerySegmentedAsync(query, token).Result;

                foreach (var entry in segment.Results)
                {
                    entry.MaruVersion = "0.4.4";
                    entry.MythrilVersion = "0.20.4";
                    TableOperation insertOperation = TableOperation.InsertOrReplace(entry);
                    table.ExecuteAsync(insertOperation).Wait();

                    Console.WriteLine($"Fixed versions in {entry.PartitionKey}");
                }

                token = segment.ContinuationToken;
            } while (token != null);

            return 0;
        }
    }
}
