using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NuciAPI.Client;
using NuciAPI.Requests;
using PersonalLogManagerClient.Models;

namespace PersonalLogManagerClient.Services
{
    public class PersonalLogService(INuciApiClient client, ApiKeyService apiKeyService)
    {
        private readonly INuciApiClient client = client;
        private readonly ApiKeyService apiKeyService = apiKeyService;

        // Matches: leading log ID (L + digits + space) and date (yyyy-MM-dd: )
        private static readonly Regex trimPattern =
            new(@"^L\d+\s+\d{4}-\d{2}-\d{2}:\s*", RegexOptions.Compiled);

        public async Task<List<string>> GetLogsForDateAsync(string date, int count = 1000, string localisation = "ro")
        {
            string apiKey = await apiKeyService.GetApiKeyAsync();

            GetLogsRequest request = new()
            {
                Date = date,
                Count = count,
                Localisation = localisation
            };

            NuciApiRequestAuthorisationInfo auth = new()
            {
                BearerToken = apiKey
            };

            NuciAPI.Responses.NuciApiResponse response = await client.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                HttpMethod.Get,
                request,
                auth,
                "/PersonalLog");

            if (response is GetLogsResponse logsResponse)
            {
                return [.. (logsResponse.Logs ?? []).Select(entry => trimPattern.Replace(entry, "")).Reverse()];
            }

            return [];
        }
    }
}
