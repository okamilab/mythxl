using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Data;
using MythXL.Data.Entities;
using MythXL.Func.ViewModels;
using MythXL.Func.Utils;

namespace MythXL.Func.Analysis
{
    public static class Get
    {
        [FunctionName("GetAnalyses")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/analyses")] HttpRequest req,
            [Table("%Storage:AnalysisTable%", Connection = "Storage:Connection")] CloudTable table,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            try
            {
                var segment = await GetAsync(req, table);
                var list = new List<AnalysisModel>(10);
                foreach (var entry in segment.Results)
                {
                    list.Add(new AnalysisModel
                    {
                        Id = $"{entry.PartitionKey}|{entry.RowKey}",
                        ApiVersion = entry.ApiVersion,
                        Error = entry.Error,
                        HarveyVersion = entry.HarveyVersion,
                        MaestroVersion = entry.MaestroVersion,
                        MaruVersion = entry.MaruVersion,
                        MythrilVersion = entry.MythrilVersion,
                        Status = entry.Status,
                        SubmittedAt = entry.SubmittedAt,
                        Version = entry.Version,
                        Issues = await Blob.ReadAsync(
                            config.GetValue<string>("Storage:Connection"),
                            config.GetValue<string>("Storage:AnalysisIssuesContainer"),
                            entry.RowKey)
                    });
                }

                return new OkObjectResult(new
                {
                    data = list,
                    next = ContinuationToken.Zip(segment.ContinuationToken)
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        private static async Task<TableQuerySegment<AnalysisEntity>> GetAsync(HttpRequest req, CloudTable table)
        {
            TableContinuationToken token = null;
            var tokenParam = (string)req.Query["t"];
            if (!string.IsNullOrEmpty(tokenParam))
            {
                token = ContinuationToken.Unzip(tokenParam);
            }

            var idParam = (string)req.Query["id"];
            if (string.IsNullOrEmpty(idParam))
            {
                throw new ArgumentException("id is empty");
            }

            var args = idParam.Split('|');
            var partKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, args[0]);
            var rowKey = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, args[1]);

            var query = new TableQuery<AnalysisEntity>
            {
                TakeCount = 10,
                FilterString = TableQuery.CombineFilters(partKey, TableOperators.And, rowKey)
            };
            return await table.ExecuteQuerySegmentedAsync(query, token);
        }
    }
}
