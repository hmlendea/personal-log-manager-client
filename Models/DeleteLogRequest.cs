using System.Text.Json.Serialization;
using NuciAPI.Requests;

namespace PersonalLogManagerClient.Models
{
    public class DeleteLogRequest : NuciApiRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
