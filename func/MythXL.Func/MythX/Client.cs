using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MythXL.Func.MythX
{
    public class Client
    {
        internal readonly HttpClient _client;
        private string _address;

        public Client(string baseUrl, string address, string pwd)
        {
            _address = address;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseUrl);
            var token = GetToken(baseUrl, address, pwd).Result;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string> AnalyzeAsync(string bytecode)
        {
            var serializedData = JsonConvert.SerializeObject(
                new { data = new { bytecode }, clientToolName = "MythXL" },
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var content = new StringContent(serializedData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/v1/analyses", content);
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                throw new AccountLimitExceedException(_address);
            }
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var analysesResponse = JsonConvert.DeserializeObject<AnalysesResponse>(result);

            var analyses = "";
            while (true)
            {
                analyses = await GetAnalyzeAsync(analysesResponse.UUID);
                try
                {
                    var res = JsonConvert.DeserializeObject<AnalysesResponse>(analyses);
                    if (res.Status == "Error" || res.Status == "Finished")
                    {
                        break;
                    }
                }
                catch (Exception) { }

                Thread.Sleep(2000);
            }

            return analyses;
        }

        public async Task<string> GetIssuesAsync(string uuid)
        {
            var response = await _client.GetAsync($"/v1/analyses/{uuid}/issues");
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetAnalyzeAsync(string uuid)
        {
            var response = await _client.GetAsync($"/v1/analyses/{uuid}");
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetToken(string baseUrl, string address, string pwd)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var serializedData = JsonConvert.SerializeObject(
                    new { ethAddress = address, password = pwd },
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var content = new StringContent(serializedData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/v1/auth/login", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(result);
                return authResponse.Access;
            }
        }
    }

    class AuthResponse
    {
        public string Access { get; set; }
    }

    class AnalysesResponse
    {
        public string UUID { get; set; }

        public string Status { get; set; }
    }
}
