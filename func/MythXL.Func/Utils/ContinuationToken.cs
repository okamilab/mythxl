using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace MythXL.Func.Utils
{
    public class ContinuationToken
    {
        public static string Zip(TableContinuationToken token)
        {
            if (token == null)
            {
                return null;
            }

            var location = token.TargetLocation.HasValue ? (int)token.TargetLocation.Value : -1;
            return $"{token.NextPartitionKey}|{token.NextRowKey}|{token.NextTableName}|{location}";
        }

        public static TableContinuationToken Unzip(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var args = value.Split('|');
            if (args.Length != 4)
            {
                throw new ArgumentException("Wrong token format");
            }

            var location = int.Parse(args[3]);
            return new TableContinuationToken
            {
                NextPartitionKey = args[0],
                NextRowKey = args[1],
                NextTableName = args[2],
                TargetLocation = location < 0 ? (StorageLocation?)null : (StorageLocation)location
            };
        }
    }
}
