using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Data.Entities;
using System.Linq;
using MythXL.Func.Models;

namespace MythXL.Func.Contract
{
    public static class GetProcessing
    {
        [FunctionName("GetProcessingStat")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/stats/processing")] HttpRequest req,
            [Table("%Storage:ProcessingStatTable%", Connection = "Storage:Connection")] CloudTable table,
            ILogger log)
        {
            var query = new TableQuery<ProcessingStatEntity> { TakeCount = 1 };
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            var stat = queryResult.Results.FirstOrDefault();
            if (stat == null)
            {
                return new BadRequestResult();
            }

            return new OkObjectResult(new ProcessingStatModel
            {
                Count = stat.Count,
                Errors = stat.Errors,
                Finished = stat.Finished,
                HighSeverity = stat.HighSeverity,
                LowSeverity = stat.LowSeverity,
                MediumSeverity = stat.MediumSeverity,
                NoSeverity = stat.NoSeverity
            });
        }
    }
}
