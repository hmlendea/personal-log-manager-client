using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using NuciAPI.Requests;

namespace PersonalLogManagerClient.Models
{
    public sealed class UpdateLogRequest : NuciApiRequest
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, JsonElement> Data { get; set; }
    }
}
