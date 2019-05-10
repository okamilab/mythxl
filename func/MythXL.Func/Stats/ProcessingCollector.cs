using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Data.Domain;
using MythXL.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MythXL.Func.Stats
{
    public static class ProcessingCollector
    {
        [FunctionName("ProcessingCollector")]
        public static async Task Run(
            [TimerTrigger("0 0 0 * * *")]TimerInfo timer,
            [Table("%Storage:ContractTable%", Connection = "Storage:Connection")] CloudTable contractTable,
            [Table("%Storage:StatTable%", Connection = "Storage:Connection")] CloudTable statTable,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

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

            TableContinuationToken token = null;
            do
            {
                var query = new TableQuery<ContractEntity> { TakeCount = 100 };
                var segment = contractTable.ExecuteQuerySegmentedAsync(query, token).Result;

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

            await statTable.ExecuteBatchAsync(batchOperation);
        }
    }
}
