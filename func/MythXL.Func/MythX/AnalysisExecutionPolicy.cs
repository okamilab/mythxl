using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Func.Entities;
using System;
using System.Threading.Tasks;

namespace MythXL.Func.MythX
{
    public class AnalysisExecutionPolicy
    {
        private CloudTable _table;
        private string _stateKey;

        public string Account { get; private set; }

        private readonly string _url;
        private readonly IConfigurationRoot _config;
        private readonly ILogger _log;

        public AnalysisExecutionPolicy(IConfigurationRoot config, ILogger log)
        {
            _config = config;
            _log = log;

            _url = _config.GetValue<string>("MythX:BaseUrl");

            _table = CreateTable(
                config.GetValue<string>("Storage:Connection"),
                config.GetValue<string>("Storage:AppStateTable"));
            _stateKey = config.GetValue<string>("MythX:AccountManager:SelectedAccount");
        }

        public async Task<string> AnalyzeAsync(string code)
        {
            var accounts = new AccountManager(_config);
            var state = await GetAccount();
            var account = accounts.Get(accounts.IndexOf(state.Value));
            var attempts = 0;

            while (attempts < accounts.Count())
            {
                try
                {
                    Account = account.Address;
                    var client = new Client(_url, account.Address, account.Password);
                    return await client.AnalyzeAsync(code);
                }
                catch (AccountLimitExceedException ex)
                {
                    _log.LogDebug($"Account limit exceeded {ex.Address}");
                    account = accounts.Next(account.Address);
                    _log.LogDebug($"Switched to {account.Address}");
                    await SetAccount(account.Address);
                    attempts++;
                }
            }

            throw new Exception("Limit exceeded for all accounts");
        }

        // TODO: move to state manager
        private async Task<StateEntity> GetAccount()
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _stateKey);
            var query = new TableQuery<StateEntity> { TakeCount = 1, FilterString = filter };
            var queryResult = await _table.ExecuteQuerySegmentedAsync(query, null);
            return queryResult.Results[0];
        }

        private async Task SetAccount(string address)
        {
            var entry = new StateEntity()
            {
                PartitionKey = _stateKey,
                RowKey = "",
                Value = address
            };
            TableOperation insertOperation = TableOperation.InsertOrReplace(entry);
            await _table.ExecuteAsync(insertOperation);
        }

        private CloudTable CreateTable(string connection, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
