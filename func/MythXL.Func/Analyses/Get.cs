using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Func.Entities;
using MythXL.Func.Utils;

namespace MythXL.Func.Analyses
{
    public static class Get
    {
        [FunctionName("GetAnalyses")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/analyses")] HttpRequest req,
            [Table("%Storage:AnalysesTable%", Connection = "Storage:Connection")] CloudTable table,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            TableContinuationToken token = null;
            var tokenParam = (string)req.Query["t"];
            if (!string.IsNullOrEmpty(tokenParam))
            {
                token = ContinuationToken.Unzip(tokenParam);
            }

            var idParam = (string)req.Query["id"];
            if (string.IsNullOrEmpty(idParam))
            {
                return new BadRequestObjectResult("id is empty");
            }

            var args = idParam.Split('|');
            var partKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, args[0]);
            var rowKey = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, args[1]);

            var query = new TableQuery<AnalysesEntity>
            {
                TakeCount = 10,
                FilterString = TableQuery.CombineFilters(partKey, TableOperators.And, rowKey)
            };
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);

            return new OkObjectResult(new
            {
                data = queryResult.Results,
                next = ContinuationToken.Zip(queryResult.ContinuationToken)
            });
        }
    }
}
