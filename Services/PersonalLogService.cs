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
    public class PersonalLogService(INuciApiClient client, ApiKeyService apiKeyService, LocaleService localeService, ApiKeyRateLimitService rateLimitService)
    {
        private readonly INuciApiClient client = client;
        private readonly ApiKeyService apiKeyService = apiKeyService;
        private readonly LocaleService localeService = localeService;
        private readonly ApiKeyRateLimitService rateLimitService = rateLimitService;

        // Matches: leading log ID (L + digits + space) and date (yyyy-MM-dd: )
        private static readonly Regex trimPattern =
            new(@"^L\d+\s+\d{4}-\d{2}-\d{2}:\s*", RegexOptions.Compiled);

        public async Task<List<string>> GetLogsForDateAsync(string date, int count = 1000, bool ascending = false)
        {
            if (rateLimitService.IsLocked)
            {
                throw new InvalidOperationException(localeService.Strings.LockedOut(rateLimitService.LockedUntil!.Value.ToLocalTime()));
            }

            string localisation = localeService.Current == LocaleService.Romanian ? "ro" : "en";
            string apiKey = await apiKeyService.GetApiKeyAsync();

            GetLogsRequest request = new()
            {
                Date = date,
                Count = count,
                Localisation = localisation
            };

            NuciApiRequestAuthorisationInfo auth = new()
            {
                BearerToken = apiKey,
                ClientId = $"PersonalLogManagerClient_{Environment.MachineName}"
            };

            NuciAPI.Responses.NuciApiResponse response = await client.SendRequestAsync<GetLogsRequest, GetLogsResponse>(
                HttpMethod.Get,
                request,
                auth,
                "/PersonalLog");

            if (response is NuciAPI.Responses.NuciApiErrorResponse errorResponse &&
                (errorResponse.Code == "AUTHENTICATION_FAILURE" || errorResponse.Code == "UNAUTHORISED"))
            {
                rateLimitService.RecordFailure();
                string message = rateLimitService.IsLocked
                    ? localeService.Strings.LockedOut(rateLimitService.LockedUntil!.Value.ToLocalTime())
                    : localeService.Strings.InvalidApiKey;

                throw new InvalidOperationException(message);
            }

            if (response is GetLogsResponse logsResponse)
            {
                IEnumerable<string> logs = (logsResponse.Logs ?? []).Select(entry => trimPattern.Replace(entry, ""));
                return ascending ? [.. logs] : [.. logs.Reverse()];
            }

            return [];
        }
    }
}
