using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PersonalLogManagerClient.Models;

namespace PersonalLogManagerClient.Services
{
    public class PersonalLogService(HttpClient http, ApiKeyService apiKeyService)
    {
        private readonly HttpClient _http = http;
        private readonly ApiKeyService _apiKeyService = apiKeyService;

        // Matches: leading log ID (L + digits + space) and date (yyyy-MM-dd: )
        private static readonly Regex _trimPattern =
            new(@"^L\d+\s+\d{4}-\d{2}-\d{2}:\s*", RegexOptions.Compiled);

        public async Task<List<string>> GetLogsForDateAsync(string date, int count = 1000, string localisation = "ro")
        {
            string apiKey = await _apiKeyService.GetApiKeyAsync();

            string url = $"/PersonalLog?date={Uri.EscapeDataString(date)}&count={count}&localisation={Uri.EscapeDataString(localisation)}";
            HttpRequestMessage request = new(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            HttpResponseMessage response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            GetLogsResponse result = await response.Content.ReadFromJsonAsync<GetLogsResponse>();
            return [.. (result?.Logs ?? []).Select(entry => _trimPattern.Replace(entry, "")).Reverse()];
        }
    }
}
