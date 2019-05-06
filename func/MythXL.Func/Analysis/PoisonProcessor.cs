using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using MythXL.Func.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MythXL.Func.Analysis
{
    public static class PoisonProcessor
    {
        [FunctionName("AnalysisPoisonProcessor")]
        public static async Task Run(
            [QueueTrigger("%Storage:AnalysisPoisonQueue%", Connection = "Storage:Connection")] AnalysisMessage message,
            [Queue("%Storage:AnalysisQueue%", Connection = "Storage:Connection")] CloudQueue contractQueue,
            ILogger log)
        {
            string msg = JsonConvert.SerializeObject(message);
            var visibilityDelay = TimeSpan.FromHours(1);
            await contractQueue.AddMessageAsync(new CloudQueueMessage(msg), null, visibilityDelay, null, null);
        }
    }
}
