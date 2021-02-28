using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dapr.Client;
using Shared;

namespace SentenceApp.Services
{
    public class LowercaseServiceClient
    {
        private readonly HttpClient _client;

        public LowercaseServiceClient(HttpClient client)
        {
            _client = client;
        }

        private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task<ConvertedResult> Convert(string sentence)
        {

            var responseMessage = await _client.GetAsync($"/lowercase?sentence={sentence}");
            var stream = await responseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<ConvertedResult>(stream, _options);
        }
    }
}