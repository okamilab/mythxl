using CommandLine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Data.Domain;
using MythXL.Data.Entities;
using System;
using System.Collections.Generic;

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

            var stat = new Dictionary<ProcessingStatFields, long>
            {
                { ProcessingStatFields.Processed, 0},
                { ProcessingStatFields.Failed, 0},
                { ProcessingStatFields.Finished, 0},
                { ProcessingStatFields.LowSeverity, 0},
                { ProcessingStatFields.MediumSeverity, 0},
                { ProcessingStatFields.HighSeverity, 0},
                { ProcessingStatFields.NoIssues, 0},
            };

            var counter = 0;

            TableContinuationToken token = null;
            do
            {
                var query = new TableQuery<ContractEntity> { TakeCount = options.BatchSize };
                var segment = sourceTable.ExecuteQuerySegmentedAsync(query, token).Result;

                foreach (var entry in segment.Results)
                {
                    stat[ProcessingStatFields.Processed] += 1;
                    if (entry.AnalysisStatus == "Error")
                    {
                        stat[ProcessingStatFields.Failed] += 1;
                    }
                    else
                    {
                        stat[ProcessingStatFields.Finished] += 1;
                    }

                    switch (entry.Severity)
                    {
                        case "Low":
                            stat[ProcessingStatFields.LowSeverity] += 1;
                            break;
                        case "Medium":
                            stat[ProcessingStatFields.MediumSeverity] += 1;
                            break;
                        case "High":
                            stat[ProcessingStatFields.HighSeverity] += 1;
                            break;
                        default:
                            stat[ProcessingStatFields.NoIssues] += 1;
                            break;
                    }
                }

                counter += segment.Results.Count;
                Console.WriteLine($"Handled {counter} records");

                token = segment.ContinuationToken;
            } while (token != null);

            var batchOperation = new TableBatchOperation();
            foreach (var key in stat.Keys)
            {
                var entry = new StatEntity
                {
                    PartitionKey = "ProcessingStat",
                    RowKey = key.ToString(),
                    Count = stat[key]
                };
                batchOperation.InsertOrReplace(entry);
            }

            var statTable = tableClient.GetTableReference(options.StatTableName);
            statTable.CreateIfNotExistsAsync().Wait();
            statTable.ExecuteBatchAsync(batchOperation).Wait();

            return 0;
        }
    }
}
