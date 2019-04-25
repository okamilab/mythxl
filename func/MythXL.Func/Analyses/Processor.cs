using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Func.Entities;
using MythXL.Func.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MythXL.Func.Analyses
{
    public static class Processor
    {
        [FunctionName("AnalysesProcessor")]
        public static async Task Run(
            [QueueTrigger("%Storage:AnalysesQueue%", Connection = "Storage:Connection")] AnalysesMessage message,
            [Table("%Storage:ContractTable%", Connection = "Storage:Connection")] CloudTable contractTable,
            [Table("%Storage:AnalysesTable%", Connection = "Storage:Connection")] CloudTable analysesTable,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var client = new MythXClient(
                config.GetValue<string>("MythX:BaseUrl"),
                config.GetValue<string>("MythX:Address"),
                config.GetValue<string>("MythX:Password"));

            string issues = null;
            string severity = null;
            var result = JsonConvert.DeserializeObject<AnalysesResult>(message.Result);
            if (result.Status == "Finished")
            {
                issues = await client.GetIssuesAsync(result.UUID);
                var list = JsonConvert.DeserializeObject<List<AnalysesIssueResult>>(issues);
                if (list.Count > 0 && list[0] != null && list[0].Issues != null && list[0].Issues.Count > 0)
                {
                    severity = list[0].Issues[0].Severity;
                }
            }

            await InsertAnalyses(analysesTable, message.Address, result, issues);
            await InsertContract(contractTable, message, result, severity);
        }

        private static async Task InsertAnalyses(CloudTable table, string address, AnalysesResult analyses, string issues)
        {
            var entry = new AnalysesEntity()
            {
                PartitionKey = address,
                RowKey = analyses.UUID,
                ApiVersion = analyses.ApiVersion,
                Error = analyses.Error,
                HarveyVersion = analyses.HarveyVersion,
                MaestroVersion = analyses.MaestroVersion,
                MaruVersion = analyses.MythrilVersion,
                QueueTime = analyses.QueueTime,
                RunTime = analyses.RunTime,
                Status = analyses.Status,
                SubmittedAt = analyses.SubmittedAt,
                SubmittedBy = analyses.SubmittedBy,
                Issues = issues
            };
            TableOperation insertOperation = TableOperation.Insert(entry);
            await table.ExecuteAsync(insertOperation);
        }

        private static async Task InsertContract(CloudTable table, AnalysesMessage message, AnalysesResult analyses, string severity)
        {
            var entry = new ContractEntity()
            {
                PartitionKey = message.Address,
                RowKey = "",
                TxHash = message.TxHash,
                Code = message.Bytecode,
                AnalyzeUUID = analyses.UUID,
                AnalyzeStatus = analyses.Status,
                Severity = severity
            };
            TableOperation insertOperation = TableOperation.Insert(entry);
            await table.ExecuteAsync(insertOperation);
        }
    }
}
