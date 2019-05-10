using CommandLine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Data.Entities;
using System;

namespace MythXL.Jobs.Commands
{
    [Verb("collect-processing-stat", HelpText = "Collects processing statistics.")]
    public class CollectProcessingStatOptions
    {
        [Option('c', "connection-string", Required = true, HelpText = "Connection string to Azure Storage.")]
        public string Connection { get; set; }

        [Option('t', "contract-table-name", Required = true, HelpText = "Contract table.")]
        public string TableName { get; set; }

        [Option('s', "stat-table-name", Required = true, HelpText = "Stat table.")]
        public string StatTableName { get; set; }

        [Option('b', "batchSize", Required = false, HelpText = "Batch size.", Default = 100)]
        public int BatchSize { get; set; }
    }

    public class CollectProcessingStat
    {
        public static int RunAddAndReturnExitCode(CollectProcessingStatOptions options)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(options.Connection);
            var tableClient = storageAccount.CreateCloudTableClient();
            var sourceTable = tableClient.GetTableReference(options.TableName);

            var stat = new ProcessingStatEntity()
            {
                PartitionKey = "stat",
                RowKey = "",
                Count = 0,
                Finished = 0,
                Errors = 0,
                NoSeverity = 0,
                LowSeverity = 0,
                MediumSeverity = 0,
                HighSeverity = 0,
            };

            TableContinuationToken token = null;
            do
            {
                var query = new TableQuery<ContractEntity> { TakeCount = options.BatchSize };
                var segment = sourceTable.ExecuteQuerySegmentedAsync(query, token).Result;

                foreach (var entry in segment.Results)
                {
                    stat.Count += 1;
                    if (entry.AnalysisStatus == "Error")
                    {
                        stat.Errors += 1;
                    }
                    else
                    {
                        stat.Finished += 1;
                    }

                    switch (entry.Severity)
                    {
                        case "Low":
                            stat.LowSeverity += 1;
                            break;
                        case "Medium":
                            stat.MediumSeverity += 1;
                            break;
                        case "High":
                            stat.HighSeverity += 1;
                            break;
                        default:
                            stat.NoSeverity += 1;
                            break;
                    }
                }

                Console.WriteLine($"Handled {segment.Results.Count} records");

                token = segment.ContinuationToken;
            } while (token != null);


            var statTable = tableClient.GetTableReference(options.StatTableName);
            statTable.CreateIfNotExistsAsync().Wait();

            TableOperation insertOperation = TableOperation.InsertOrReplace(stat);
            statTable.ExecuteAsync(insertOperation).Wait();

            return 0;
        }
    }
}
