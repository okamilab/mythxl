using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Data.Entities;
using System.Threading.Tasks;

namespace MythXL.Func.Stats
{
    public static class ProcessingCollector
    {
        [FunctionName("ProcessingCollector")]
        public static async Task Run(
            [TimerTrigger("0 0 0 * * *")]TimerInfo timer,
            [Table("%Storage:ContractTable%", Connection = "Storage:Connection")] CloudTable contractTable,
            [Table("%Storage:ProcessingStatTable%", Connection = "Storage:Connection")] CloudTable statTable,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

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
                var query = new TableQuery<ContractEntity> { TakeCount = 100 };
                var segment = contractTable.ExecuteQuerySegmentedAsync(query, token).Result;

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

                token = segment.ContinuationToken;
            } while (token != null);

            TableOperation insertOperation = TableOperation.InsertOrReplace(stat);
            await statTable.ExecuteAsync(insertOperation);
        }
    }
}
