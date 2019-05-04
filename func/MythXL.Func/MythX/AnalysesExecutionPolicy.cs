using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MythXL.Func.MythX
{
    public class AnalysesExecutionPolicy
    {
        public string Account { get; private set; }

        private readonly string _url;
        private readonly IConfigurationRoot _config;

        public AnalysesExecutionPolicy(IConfigurationRoot config)
        {
            _config = config;
            _url = _config.GetValue<string>("MythX:BaseUrl");
        }

        public async Task<string> AnalyzeAsync(string code)
        {
            var creds = new AccountManager(_config);
            var acc = creds.Get();
            var attempts = 0;

            while (attempts < creds.Count())
            {
                try
                {
                    Account = acc.Address;
                    var client = new Client(_url, acc.Address, acc.Password);
                    return await client.AnalyzeAsync(code);
                }
                catch (AccountLimitExceedException)
                {
                    acc = creds.Next(acc.Address);
                    attempts++;
                }
            }

            throw new Exception("Limit exceeded for all accounts");
        }
    }
}
