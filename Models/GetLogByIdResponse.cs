using System.Collections.Generic;
using System.Text.Json.Serialization;

using NuciAPI.Responses;

namespace PersonalLogManagerClient.Models
{
    public sealed class GetLogByIdResponse : NuciApiSuccessResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("template")]
        public string Template { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, string> Data { get; set; }

        [JsonPropertyName("createdDateTime")]
        public string CreatedDateTime { get; set; }

        [JsonPropertyName("updatedDateTime")]
        public string UpdatedDateTime { get; set; }
    }
}
