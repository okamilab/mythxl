using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MythXL.Data.Entities;
using System.Threading.Tasks;

namespace MythXL.Func.Utils
{
    public class StateManager
    {
        private CloudTable _table;

        public StateManager(IConfigurationRoot config)
        {
            _table = CreateTable(
                config.GetValue<string>("Storage:Connection"),
                config.GetValue<string>("Storage:AppStateTable"));
        }

        public async Task<StateEntity> GetValueAsync(string key)
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, key);
            var query = new TableQuery<StateEntity> { TakeCount = 1, FilterString = filter };
            var queryResult = await _table.ExecuteQuerySegmentedAsync(query, null);
            return queryResult.Results[0];
        }

        public async Task SetValueAsync(string key, string value)
        {
            var entry = new StateEntity()
            {
                PartitionKey = key,
                RowKey = "",
                Value = value
            };
            TableOperation insertOperation = TableOperation.InsertOrReplace(entry);
            await _table.ExecuteAsync(insertOperation);
        }

        private CloudTable CreateTable(string connection, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);
            var client = storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(tableName);
            table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
