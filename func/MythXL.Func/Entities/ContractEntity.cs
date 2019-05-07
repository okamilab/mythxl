using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace MythXL.Func.Entities
{
    public class ContractEntity : TableEntity
    {
        public string TxHash { get; set; }

        [Obsolete("Deprecated in entity version 1")]
        public string Code { get; set; }

        public string AnalysisId { get; set; }

        public string AnalysisStatus { get; set; }

        public string Severity { get; set; }

        public int Version { get; set; }
    }
}
