using Microsoft.WindowsAzure.Storage.Table;

namespace MythXL.Func.Entities
{
    public class ContractEntity : TableEntity
    {
        public string TxHash { get; set; }

        public string Code { get; set; }

        public string AnalyzeUUID { get; set; }

        public string AnalyzeStatus { get; set; }

        public string Severity { get; set; }
    }
}
