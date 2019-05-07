using CommandLine;

namespace MythXL.Transform.Commands
{
    [Verb("test", HelpText = "Does nothing.")]
    public class TestOptions
    {
    }

    public static class Test
    {
        public static int RunAddAndReturnExitCode(TestOptions options)
        {
            return 0;
        }
    }
}
