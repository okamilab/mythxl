using System.Collections.Generic;

namespace MythXL.Func.Models
{
    public class AnalysesIssueResult
    {
        public List<Issue> Issues { get; set; }
    }

    public class Issue
    {
        public string SwcID { get; set; }

        public string SwcTitle { get; set; }

        public IssueDescription Description { get; set; }

        public string Severity { get; set; }
    }

    public class IssueDescription
    {
        public string Head { get; set; }
        public string Tail { get; set; }
    }
}
