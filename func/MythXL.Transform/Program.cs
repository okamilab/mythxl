﻿using CommandLine;
using MythXL.Transform.Commands;

namespace MythXL.Transform
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AzureTableEntryMoveOptions, MigrateContractV1Options>(args)
                .MapResult(
                    (AzureTableEntryMoveOptions opts) => AzureTableEntryMove.RunAddAndReturnExitCode(opts),
                    (MigrateContractV1Options opts) => MigrateContractV1.RunAddAndReturnExitCode(opts),
                    errs => 1);
        }
    }
}