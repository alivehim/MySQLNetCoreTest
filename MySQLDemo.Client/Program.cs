using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MySQLDemo.Client
{
    class Program
    {
        public static void Main()
        {
            var requestWithoutPolicyResponse = Task.Run(Send).Result;

            Console.WriteLine(requestWithoutPolicyResponse);
            //Console.ReadKey();
        }


        private async static Task<string> Send()
        {
            async Task<string> GetAccessToken()
            {
                var client = new HttpClient();
                var discoveryResponse = await client.GetDiscoveryDocumentAsync("http://localhost:5000");

                if (discoveryResponse.IsError)
                {
                    Console.WriteLine(discoveryResponse.Error);
                    return "error";
                }

                // request token
                //var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "ro.client1", "123654");
                //var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "ro.client", "secret");
                var accessToken = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = discoveryResponse.TokenEndpoint,
                    ClientId = "ro.client",
                    ClientSecret = "secret",
                    UserName = "hello",
                    Password = "1111",
                    Scope = "api1 offline_access",
                });

                Console.WriteLine((int)accessToken.HttpStatusCode);

                if (accessToken.IsError)
                {
                    Console.WriteLine(accessToken.Error);
                    return accessToken.Error;
                }
               
                Console.WriteLine(accessToken.Json);

                return accessToken.AccessToken;
            }

            using (var client = new HttpClient())
            {
                var accessToken = await GetAccessToken();

                //Console.WriteLine(accessToken);

                return accessToken;
            }
        }

        private static async Task MainX()
        {
            // discover endpoints from metadata
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",

                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            //// call api
            //var apiClient = new HttpClient();
            //apiClient.SetBearerToken(tokenResponse.AccessToken);

            //var response = await apiClient.GetAsync("http://localhost:5001/identity");
            //if (!response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(response.StatusCode);
            //}
            //else
            //{
            //    var content = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(JArray.Parse(content));
            //}
        }
    }
}
