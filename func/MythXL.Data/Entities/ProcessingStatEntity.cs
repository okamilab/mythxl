using Microsoft.WindowsAzure.Storage.Table;

namespace MythXL.Data.Entities
{
    public class ProcessingStatEntity : TableEntity
    {
        public int Count { get; set; }

        public int Errors { get; set; }

        public int Finished { get; set; }

        public int NoSeverity { get; set; }

        public int LowSeverity { get; set; }

        public int MediumSeverity { get; set; }

        public int HighSeverity { get; set; }
    }
}
