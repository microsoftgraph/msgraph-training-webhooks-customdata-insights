using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Batch
{
    class BatchDemo
    {
        public async Task RunAsync(string clientId)
        {
            PublicClientApplication pca = new PublicClientApplication(clientId);
            string[] scopes = { "User.ReadWrite Calendars.Read Mail.Read Contacts.Read" };
            var authResult = await pca.AcquireTokenAsync(scopes);
            var accessToken = authResult.AccessToken;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                await RetrieveUserOutlookData(client);
            }
        }

        async Task RetrieveUserOutlookData(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "$batch");
            request.Content = new StringContent(@"{
                  'requests': [
                    {
                      'id': '1',
                      'method': 'GET',
                      'url': '/me/messages?$top=1'
                    },
                    {
                      'id': '2',
                      'dependsOn': [ '1' ],
                      'method': 'GET',
                      'url': '/me/calendar/events?$top=1'
                    },
                    {
                      'id': '3',
                      'dependsOn': [ '2' ],
                      'method': 'GET',
                      'url': 'me/contacts?$top=1'
                    }
                  ]
                }", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            response.WriteCodeAndReasonToConsole();
            Console.WriteLine(JValue.Parse(await response.Content.ReadAsStringAsync()).ToString(Newtonsoft.Json.Formatting.Indented));
            Console.WriteLine();
        }
    }
}
