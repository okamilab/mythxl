using CommandLine;
using MythXL.Jobs.Commands;

namespace MythXL.Jobs
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                CollectProcessingStatOptions,
                CollectIssuesStatOptions>(args)
                .MapResult(
                    (CollectProcessingStatOptions opts) => CollectProcessingStat.RunAddAndReturnExitCode(opts),
                    (CollectIssuesStatOptions opts) => CollectIssuesStat.RunAddAndReturnExitCode(opts),
                    errs => 1);
        }
    }
}
