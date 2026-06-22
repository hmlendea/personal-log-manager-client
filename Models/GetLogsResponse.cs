using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PersonalLogManagerClient.Models
{
    public class GetLogsResponse
    {
        [JsonPropertyName("logs")]
        public List<string> Logs { get; set; } = [];

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
