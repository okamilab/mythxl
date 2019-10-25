using CommandLine;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Linq;

namespace MythXL.Transform.Commands
{
    [Verb("reset-contract", HelpText = "Reset contract.")]
    public class ResetContractOptions
    {
        [Option('c', "connection-string", Required = true, HelpText = "Connection string to Azure Storage.")]
        public string Connection { get; set; }

        [Option('q', "queue", Required = true, HelpText = "Source queue.")]
        public string QueueName { get; set; }

        [Option('b', "batchSize", Required = false, HelpText = "Batch size.", Default = 32)]
        public int BatchSize { get; set; }
    }

    public static class ResetContract
    {
        public static int RunAddAndReturnExitCode(ResetContractOptions options)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(options.Connection);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(options.QueueName);

            while (true)
            {
                var messages = queue.GetMessagesAsync(options.BatchSize).Result;

                if (messages.Count() == 0)
                {
                    break;
                }

                foreach (var msg in messages)
                {
                    var visibilityDelay = TimeSpan.FromHours(1);
                    queue.DeleteMessageAsync(msg).Wait();
                    queue.AddMessageAsync(msg, null, visibilityDelay, null, null).Wait();
                }
            }

            return 0;
        }
    }
}
