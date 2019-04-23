using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using MythXL.Func.Models;
using Nethereum.Web3;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MythXL.Func
{
    public static class ContractProcessor
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

            var client = new MythXClient(
                config.GetValue<string>("MythX:BaseUrl"),
                config.GetValue<string>("MythX:Address"),
                config.GetValue<string>("MythX:Password"));

            if (string.IsNullOrEmpty(code) || code == "0x")
            {
                return;
            }

            var analyses = await client.AnalyzeAsync(code);

            string msg = JsonConvert.SerializeObject(new AnalysesMessage
            {
                Address = message.Address,
                TxHash = message.TxHash,
                Bytecode = code,
                Result = analyses
            });
            await analysesQueue.AddMessageAsync(new CloudQueueMessage(msg));
        }
    }
}
