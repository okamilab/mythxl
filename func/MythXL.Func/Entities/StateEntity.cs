using Microsoft.WindowsAzure.Storage.Table;

namespace MythXL.Func.Entities
{
    public class StateEntity : TableEntity
    {
        public string Value { get; set; }
    }
}
