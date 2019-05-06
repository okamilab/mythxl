using System;

namespace MythXL.Func.Models
{
    public class AnalysisMessage
    {
        public string Address { get; set; }

        public string TxHash { get; set; }

        public string AnalysisId { get; set; }

        public string Account { get; set; }

        public int Version { get; set; }
    }
}
