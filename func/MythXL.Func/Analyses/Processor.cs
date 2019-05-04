using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Func.Entities;
using MythXL.Func.Models;
using MythXL.Func.MythX;
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

            string issues = null;
            string severity = null;
            var result = GetResult(config, message).Result;
            if (result.Status == "Finished")
            {
                var client = GetClient(config, message);
                issues = await client.GetIssuesAsync(result.UUID);

                var list = JsonConvert.DeserializeObject<List<AnalysesIssueResult>>(issues);
                if (list.Count > 0 && list[0] != null && list[0].Issues != null && list[0].Issues.Count > 0)
                {
                    severity = list[0].Issues[0].Severity;
                }
            }

            await InsertAnalyses(analysesTable, message.Address, result, issues);
            await WriteContractCode(config, message.Address, message.Bytecode);
            await InsertContract(contractTable, message, result, severity);
        }

        private static Client GetClient(IConfigurationRoot config, AnalysesMessage message)
        {
            if (message.Version <= 1)
            {
                return new Client(
                    config.GetValue<string>("MythX:BaseUrl"),
                    config.GetValue<string>("MythX:Address"),
                    config.GetValue<string>("MythX:Password"));
            }

            var creds = new AccountManager(config);
            var password = creds.GetPassword(message.Account);
            return new Client(config.GetValue<string>("MythX:BaseUrl"), message.Account, password);
        }

        private static async Task<AnalysesResult> GetResult(IConfigurationRoot config, AnalysesMessage message)
        {
            // TODO: remove fetching result
            if (message.Version == 0)
            {
                return JsonConvert.DeserializeObject<AnalysesResult>(message.Result);
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.GetValue<string>("Storage:Connection"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(config.GetValue<string>("Storage:AnalysesContainer"));
            await blobContainer.CreateIfNotExistsAsync();

            var cloudBlockBlob = blobContainer.GetBlockBlobReference(message.Address);
            var content = await cloudBlockBlob.DownloadTextAsync();

            return JsonConvert.DeserializeObject<AnalysesResult>(content);
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
            TableOperation insertOperation = TableOperation.InsertOrReplace(entry);
            await table.ExecuteAsync(insertOperation);
        }

        // TODO: remove code writing
        private static async Task WriteContractCode(IConfigurationRoot config, string blobName, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.GetValue<string>("Storage:Connection"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(config.GetValue<string>("Storage:ContractContainer"));
            await blobContainer.CreateIfNotExistsAsync();

            var cloudBlockBlob = blobContainer.GetBlockBlobReference(blobName);
            await cloudBlockBlob.UploadTextAsync(content);
        }

        // TODO: transform all contracts with zero version
        private static async Task InsertContract(CloudTable table, AnalysesMessage message, AnalysesResult analyses, string severity)
        {
            var entry = new ContractEntity()
            {
                PartitionKey = message.Address,
                RowKey = "",
                TxHash = message.TxHash,
                AnalyzeUUID = analyses.UUID,
                AnalyzeStatus = analyses.Status,
                Severity = severity,
                Version = 1
            };
            TableOperation insertOperation = TableOperation.InsertOrReplace(entry);
            await table.ExecuteAsync(insertOperation);
        }
    }
}
