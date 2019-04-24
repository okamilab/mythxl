using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Queue;
using MythXL.Func.Models;

namespace MythXL.Func
{
    public static class BlockEnqueuer
    {
        [FunctionName("BlockEnqueuer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Queue("%Storage:BlockQueue%", Connection = "Storage:Connection")] CloudQueue blockQueue,
            ILogger log)
        {
            var numberParam = req.Query["number"];
            var number = int.Parse(numberParam);
            if (number <= 0)
            {
                return new BadRequestObjectResult("Number should be grater then zero");
            }

            for (var i = 0; i < 100000; i++)
            {
                string msg = JsonConvert.SerializeObject(new BlockMessage { Id = number + i });
                await blockQueue.AddMessageAsync(new CloudQueueMessage(msg));
            }

            return new OkObjectResult("");
        }
    }
}
