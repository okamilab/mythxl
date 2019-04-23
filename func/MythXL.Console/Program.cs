using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MythXL.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            using (var client = new HttpClient())
            {
                var token = await GetToken();
                client.BaseAddress = new Uri("https://api.mythx.io");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                //var serializedData = JsonConvert.SerializeObject(data,
                //    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                //var content = new StringContent(serializedData, Encoding.UTF8, "application/json");

                //var response = await client.PostAsync(requestUri, content);
                //response.EnsureSuccessStatusCode();
            }

        }

        static async Task<string> GetToken()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.mythx.io");
                var serializedData = JsonConvert.SerializeObject(new
                {
                    ethAddress = "0x0000000000000000000000000000000000000000",
                    password = "trial"
                },
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var content = new StringContent(serializedData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/v1/auth/login", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(result);
                return authResponse.Access;
            }
        }

        class AuthResponse
        {
            public string Access { get; set; }
        }
    }
}
