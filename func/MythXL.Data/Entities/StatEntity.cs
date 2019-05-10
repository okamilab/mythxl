using Microsoft.WindowsAzure.Storage.Table;

namespace MythXL.Data.Entities
{
    public class StatEntity : TableEntity
    {
        public long Count { get; set; }
    }
}
