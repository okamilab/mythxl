using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Data.Entities;
using MythXL.Func.Utils;

namespace MythXL.Func.Contract
{
    public static class Get
    {
        [FunctionName("GetContracts")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/contracts")] HttpRequest req,
            [Table("%Storage:ContractTable%", Connection = "Storage:Connection")] CloudTable table,
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

            var filter = "";
            var queryParam = (string)req.Query["q"];
            if (!string.IsNullOrEmpty(queryParam))
            {
                filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, queryParam);
            }

            var severityParam = (string)req.Query["s"];
            if (!string.IsNullOrEmpty(severityParam))
            {
                var severityFilter = TableQuery.GenerateFilterCondition("Severity", QueryComparisons.Equal, severityParam);
                filter = string.IsNullOrEmpty(filter) ? severityFilter : TableQuery.CombineFilters(filter, TableOperators.And, severityFilter);
            }

            var analysisParam = (string)req.Query["a"];
            if (!string.IsNullOrEmpty(analysisParam))
            {
                var analysisFilter = TableQuery.GenerateFilterCondition("AnalysisStatus", QueryComparisons.Equal, analysisParam);
                filter = string.IsNullOrEmpty(filter) ? analysisFilter : TableQuery.CombineFilters(filter, TableOperators.And, analysisFilter);
            }

            var query = new TableQuery<ContractEntity> { TakeCount = 10, FilterString = filter };
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);

            // TODO use view model instead of entity from storage
            return new OkObjectResult(new
            {
                data = queryResult.Results,
                next = ContinuationToken.Zip(queryResult.ContinuationToken)
            });
        }
    }
}
