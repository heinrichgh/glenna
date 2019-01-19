using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Core.Entities.GuildWars;
using Core.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Infrastructure
{
    public class GuildWarsApi : IGuildWarsApi
    {
        public async Task<Account> Fetch(string apiKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                var url = new Uri("https://api.guildwars2.com/v2/account");
                
                var response = await client.GetAsync(url);
                string json;
                using (var content = response.Content)
                {
                    json = await content.ReadAsStringAsync();
                }
                return JsonConvert.DeserializeObject<Account>(json, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                });
            }
        }
    }
}