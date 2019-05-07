using CommandLine;
using MythXL.Transform.Commands;

namespace MythXL.Transform
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AzureTableEntryMoveOptions, TestOptions>(args)
                .MapResult(
                    (AzureTableEntryMoveOptions opts) => AzureTableEntryMove.RunAddAndReturnExitCode(opts),
                    (TestOptions opts) => Test.RunAddAndReturnExitCode(opts),
                    errs => 1);
        }
    }
}
