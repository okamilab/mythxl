using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Func.Entities;
using MythXL.Func.Models;
using MythXL.Func.MythX;
using MythXL.Func.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MythXL.Func.Analysis
{
    public static class Processor
    {
        [FunctionName("AnalysisProcessor")]
        public static async Task Run(
            [QueueTrigger("%Storage:AnalysisQueue%", Connection = "Storage:Connection")] AnalysisMessage message,
            [Queue("%Storage:AnalysisQueue%", Connection = "Storage:Connection")] CloudQueue analysisQueue,
            [Table("%Storage:ContractTable%", Connection = "Storage:Connection")] CloudTable contractTable,
            [Table("%Storage:AnalysisTable%", Connection = "Storage:Connection")] CloudTable analysisTable,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var client = GetClient(config, message);
            var analysis = await client.GetAnalysisAsync(message.AnalysisId);
            var result = JsonConvert.DeserializeObject<AnalysisResult>(analysis);
            if (result.Status != "Error" && result.Status != "Finished")
            {
                string msg = JsonConvert.SerializeObject(message);
                var visibilityDelay = TimeSpan.FromMinutes(4);
                await analysisQueue.AddMessageAsync(new CloudQueueMessage(msg), null, visibilityDelay, null, null);
                return;
            }

            string issues = null;
            string severity = null;
            if (result.Status == "Finished")
            {
                issues = await client.GetIssuesAsync(result.UUID);

                var list = JsonConvert.DeserializeObject<List<AnalysisIssueResult>>(issues);
                if (list.Count > 0 && list[0] != null && list[0].Issues != null && list[0].Issues.Count > 0)
                {
                    severity = list[0].Issues[0].Severity;
                }
            }

            await Blob.Write(
                config.GetValue<string>("Storage:Connection"),
                config.GetValue<string>("Storage:AnalysisContainer"),
                message.Address,
                analysis);
            await InsertAnalysis(analysisTable, message.Address, result, issues);
            await InsertContract(contractTable, message, result, severity);
        }

        private static Client GetClient(IConfigurationRoot config, AnalysisMessage message)
        {
            if (message.Version <= 1)
            {
                return new Client(
                    config.GetValue<string>("MythX:BaseUrl"),
                    config.GetValue<string>("MythX:Address"),
                    config.GetValue<string>("MythX:Password"));
            }

            var accounts = new AccountManager(config);
            var password = accounts.GetPassword(message.Account);
            return new Client(config.GetValue<string>("MythX:BaseUrl"), message.Account, password);
        }

        private static async Task InsertAnalysis(CloudTable table, string address, AnalysisResult analysis, string issues)
        {
            var entry = new AnalysisEntity()
            {
                PartitionKey = address,
                RowKey = analysis.UUID,
                ApiVersion = analysis.ApiVersion,
                Error = analysis.Error,
                HarveyVersion = analysis.HarveyVersion,
                MaestroVersion = analysis.MaestroVersion,
                MaruVersion = analysis.MythrilVersion,
                QueueTime = analysis.QueueTime,
                RunTime = analysis.RunTime,
                Status = analysis.Status,
                SubmittedAt = analysis.SubmittedAt,
                SubmittedBy = analysis.SubmittedBy,
                Issues = issues
            };
            TableOperation insertOperation = TableOperation.InsertOrReplace(entry);
            await table.ExecuteAsync(insertOperation);
        }

        // TODO: transform all contracts with zero version
        private static async Task InsertContract(CloudTable table, AnalysisMessage message, AnalysisResult analysis, string severity)
        {
            var entry = new ContractEntity()
            {
                PartitionKey = message.Address,
                RowKey = "",
                TxHash = message.TxHash,
                AnalyzeUUID = analysis.UUID,
                AnalyzeStatus = analysis.Status,
                Severity = severity,
                Version = 1
            };
            TableOperation insertOperation = TableOperation.InsertOrReplace(entry);
            await table.ExecuteAsync(insertOperation);
        }
    }
}
