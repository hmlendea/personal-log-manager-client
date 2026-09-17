using NuciAPI.Requests;

namespace PersonalLogManagerClient.Models
{
    public class GetLogsRequest : NuciApiRequest
    {
        public string Date { get; set; }

        public int Count { get; set; }

        public string Localisation { get; set; }
    }
}
