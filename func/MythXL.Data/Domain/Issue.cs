namespace MythXL.Data.Domain
{
    public class Issue
    {
        public string SwcID { get; set; }

        public string SwcTitle { get; set; }

        public string Severity { get; set; }

        public IssueDescription Description { get; set; }
    }
}
