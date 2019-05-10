using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Data.Entities;
using MythXL.Data.Domain;
using MythXL.Func.ViewModels;

namespace MythXL.Func.Contract
{
    public static class GetProcessing
    {
        [FunctionName("GetProcessingStat")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/stats/processing")] HttpRequest req,
            [Table("%Storage:StatTable%", Connection = "Storage:Connection")] CloudTable table,
            ILogger log)
        {
            var partKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "ProcessingStat");
            var query = new TableQuery<StatEntity> { FilterString = partKey };
            var segment = await table.ExecuteQuerySegmentedAsync(query, null);

            var model = new ProcessingStatModel();
            foreach (var entry in segment.Results)
            {
                switch (entry.RowKey)
                {
                    case nameof(ProcessingStatFields.Processed):
                        model.Processed = entry.Count;
                        break;
                    case nameof(ProcessingStatFields.Failed):
                        model.Failed = entry.Count;
                        break;
                    case nameof(ProcessingStatFields.Finished):
                        model.Finished = entry.Count;
                        break;
                    case nameof(ProcessingStatFields.HighSeverity):
                        model.HighSeverity = entry.Count;
                        break;
                    case nameof(ProcessingStatFields.LowSeverity):
                        model.LowSeverity = entry.Count;
                        break;
                    case nameof(ProcessingStatFields.MediumSeverity):
                        model.MediumSeverity = entry.Count;
                        break;
                    case nameof(ProcessingStatFields.NoIssues):
                        model.NoIssues = entry.Count;
                        break;
                    default:
                        break;
                }
            }

            return new OkObjectResult(model);
        }
    }
}
