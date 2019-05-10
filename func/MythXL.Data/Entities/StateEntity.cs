using Microsoft.WindowsAzure.Storage.Table;

namespace MythXL.Data.Entities
{
    public class StateEntity : TableEntity
    {
        public string Value { get; set; }
    }
}
