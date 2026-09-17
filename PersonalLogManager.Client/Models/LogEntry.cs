namespace PersonalLogManagerClient.Models
{
    public sealed class LogEntry
    {
        public string Id { get; init; }

        public string Date { get; init; }

        public string Text { get; init; }

        public string RawText { get; init; }
    }
}
