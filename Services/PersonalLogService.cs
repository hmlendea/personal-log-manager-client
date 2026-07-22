using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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

        // Matches: log ID (L + digits), date (yyyy-MM-dd), and text separately.
        private static readonly Regex parsePattern =
            new(@"^(L\d+)\s+(\d{4}-\d{2}-\d{2}):\s*(.*)", RegexOptions.Compiled | RegexOptions.Singleline);

        public async Task<List<LogEntry>> GetLogsForDateAsync(string date, int count = 1000, bool ascending = false)
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
                IEnumerable<LogEntry> logs = (logsResponse.Logs ?? []).Select(CreateLogEntry);
                return ascending ? [.. logs] : [.. logs.Reverse()];
            }

            return [];
        }

        public async Task DeleteLogAsync(string id)
        {
            if (rateLimitService.IsLocked)
            {
                throw new InvalidOperationException(localeService.Strings.LockedOut(rateLimitService.LockedUntil!.Value.ToLocalTime()));
            }

            string apiKey = await apiKeyService.GetApiKeyAsync();

            DeleteLogRequest request = new()
            {
                Id = id
            };

            NuciApiRequestAuthorisationInfo auth = new()
            {
                BearerToken = apiKey,
                ClientId = $"PersonalLogManagerClient_{Environment.MachineName}"
            };

            NuciAPI.Responses.NuciApiResponse response = await client.SendRequestAsync<DeleteLogRequest, NuciAPI.Responses.NuciApiSuccessResponse>(
                HttpMethod.Delete,
                request,
                auth,
                $"/PersonalLog/{id}");

            if (response is NuciAPI.Responses.NuciApiErrorResponse errorResponse &&
                (errorResponse.Code == "AUTHENTICATION_FAILURE" || errorResponse.Code == "UNAUTHORISED"))
            {
                rateLimitService.RecordFailure();
                string message = rateLimitService.IsLocked
                    ? localeService.Strings.LockedOut(rateLimitService.LockedUntil!.Value.ToLocalTime())
                    : localeService.Strings.InvalidApiKey;

                throw new InvalidOperationException(message);
            }
        }

        public async Task UpdateLogAsync(
            string id,
            string date,
            string time,
            string timeZone,
            Dictionary<string, JsonElement> data)
        {
            if (rateLimitService.IsLocked)
            {
                throw new InvalidOperationException(localeService.Strings.LockedOut(rateLimitService.LockedUntil!.Value.ToLocalTime()));
            }

            string apiKey = await apiKeyService.GetApiKeyAsync();

            UpdateLogRequest request = new()
            {
                Date = date,
                Time = time,
                TimeZone = timeZone,
                Data = data
            };

            NuciApiRequestAuthorisationInfo auth = new()
            {
                BearerToken = apiKey,
                ClientId = $"PersonalLogManagerClient_{Environment.MachineName}"
            };

            NuciAPI.Responses.NuciApiResponse response = await client.SendRequestAsync<UpdateLogRequest, NuciAPI.Responses.NuciApiSuccessResponse>(
                HttpMethod.Put,
                request,
                auth,
                $"/PersonalLog/{id}");

            if (response is NuciAPI.Responses.NuciApiErrorResponse errorResponse &&
                (errorResponse.Code == "AUTHENTICATION_FAILURE" || errorResponse.Code == "UNAUTHORISED"))
            {
                rateLimitService.RecordFailure();

                string message = localeService.Strings.InvalidApiKey;

                if (rateLimitService.IsLocked)
                {
                    message = localeService.Strings.LockedOut(rateLimitService.LockedUntil!.Value.ToLocalTime());
                }

                throw new InvalidOperationException(message);
            }
        }

        private LogEntry CreateLogEntry(string rawText)
        {
            Match match = parsePattern.Match(rawText ?? "");

            if (match.Success)
            {
                return new LogEntry
                {
                    Id = match.Groups[1].Value,
                    Date = match.Groups[2].Value,
                    Text = match.Groups[3].Value,
                    RawText = rawText
                };
            }

            return new LogEntry
            {
                Id = "",
                Date = "",
                Text = rawText ?? "",
                RawText = rawText ?? ""
            };
        }
    }
}
