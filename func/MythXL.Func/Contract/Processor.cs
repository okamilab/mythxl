using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MythXL.Func.Models;
using MythXL.Func.MythX;
using Nethereum.Web3;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MythXL.Func.Contract
{
    public static class Processor
    {
        [FunctionName("ContractProcessor")]
        public static async Task Run(
            [QueueTrigger("%Storage:ContractQueue%", Connection = "Storage:Connection")] ContractMessage message,
            [Queue("%Storage:AnalysesQueue%", Connection = "Storage:Connection")] CloudQueue analysesQueue,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var web3 = new Web3(config.GetValue<string>("Blockchain:Endpoint"));
            var code = await web3.Eth.GetCode.SendRequestAsync(message.Address);
            if (string.IsNullOrEmpty(code) || code == "0x")
            {
                return;
            }

            var policy = new AnalysesExecutionPolicy(config);
            var analyses = await policy.AnalyzeAsync(code);

            await WriteBlob(
                config.GetValue<string>("Storage:Connection"),
                config.GetValue<string>("Storage:ContractContainer"),
                message.Address,
                code);
            await WriteBlob(
                config.GetValue<string>("Storage:Connection"),
                config.GetValue<string>("Storage:AnalysesContainer"),
                message.Address,
                analyses);

            string msg = JsonConvert.SerializeObject(new AnalysesMessage
            {
                Address = message.Address,
                TxHash = message.TxHash,
                Account = policy.Account,
                Version = 2
            });
            await analysesQueue.AddMessageAsync(new CloudQueueMessage(msg));
        }

        private static async Task WriteBlob(string connection, string container, string blobName, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content), $"Connection: {connection}, Container: {container}, BlobName: {blobName}");
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(container);
            await blobContainer.CreateIfNotExistsAsync();

            var cloudBlockBlob = blobContainer.GetBlockBlobReference(blobName);
            await cloudBlockBlob.UploadTextAsync(content);
        }
    }
}
