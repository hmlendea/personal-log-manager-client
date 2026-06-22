using System.Collections.Generic;
using System.Text.Json.Serialization;
using NuciAPI.Responses;

namespace PersonalLogManagerClient.Models
{
    public class GetLogsResponse : NuciApiSuccessResponse
    {
        [JsonPropertyName("logs")]
        public List<string> Logs { get; set; } = [];

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
