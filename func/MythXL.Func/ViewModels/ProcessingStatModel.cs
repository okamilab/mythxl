namespace MythXL.Func.ViewModels
{
    public class ProcessingStatModel
    {
        public long Processed { get; set; }

        public long Failed { get; set; }

        public long Finished { get; set; }

        public long NoIssues { get; set; }

        public long LowSeverity { get; set; }

        public long MediumSeverity { get; set; }

        public long HighSeverity { get; set; }
    }
}
