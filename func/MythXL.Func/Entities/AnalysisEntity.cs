using Microsoft.WindowsAzure.Storage.Table;

namespace MythXL.Func.Entities
{
    public class AnalysisEntity : TableEntity
    {
        public string ApiVersion { get; set; }

        public string Error { get; set; }

        public string HarveyVersion { get; set; }

        public string MaestroVersion { get; set; }

        public string MaruVersion { get; set; }

        public string MythrilVersion { get; set; }

        public string QueueTime { get; set; }

        public string RunTime { get; set; }

        public string Status { get; set; }

        public string SubmittedAt { get; set; }

        public string SubmittedBy { get; set; }

        public string Issues { get; set; }
    }
}
