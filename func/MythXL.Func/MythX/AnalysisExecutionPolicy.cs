using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MythXL.Func.Utils;
using System;
using System.Threading.Tasks;

namespace MythXL.Func.MythX
{
    public class AnalysisExecutionPolicy
    {
        public string Account { get; private set; }

        private readonly IConfigurationRoot _config;
        private readonly ILogger _log;
        private readonly string _url;
        private readonly string _selectedAccountKey;
        private readonly StateManager _stateManager;

        public AnalysisExecutionPolicy(IConfigurationRoot config, ILogger log)
        {
            _config = config;
            _log = log;

            _url = _config.GetValue<string>("MythX:BaseUrl");
            _selectedAccountKey = config.GetValue<string>("MythX:AccountManager:SelectedAccount");
            _stateManager = new StateManager(config);
        }

        public async Task<string> AnalyzeAsync(string code)
        {
            var accounts = new AccountManager(_config);
            var state = await _stateManager.GetValueAsync(_selectedAccountKey);
            var account = accounts.Get(accounts.IndexOf(state.Value));
            var attempts = 0;

            while (attempts < accounts.Count())
            {
                try
                {
                    Account = account.Address;
                    var client = new Client(_url, account.Address, account.Password);
                    return await client.AnalyzeAsync(code);
                }
                catch (AccountLimitExceedException ex)
                {
                    _log.LogDebug($"Account limit exceeded {ex.Address}");
                    account = accounts.Next(account.Address);
                    _log.LogDebug($"Switched to {account.Address}");
                    await _stateManager.SetValueAsync(_selectedAccountKey, account.Address);
                    attempts++;
                }
            }

            throw new Exception("Limit exceeded for all accounts");
        }
    }
}
