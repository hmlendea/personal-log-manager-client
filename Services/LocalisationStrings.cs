using System;

namespace PersonalLogManagerClient.Services
{
    public sealed class LocalisationStrings
    {
        // Page title / navigation
        public string TitleToday { get; init; }
        public string TitleYesterday { get; init; }
        public string TitleEntries { get; init; }
        public string PreviousDay { get; init; }
        public string NextDay { get; init; }

        // Buttons / status
        public string Loading { get; init; }
        public string Refresh { get; init; }
        public string SortAscending { get; init; }
        public string SortDescending { get; init; }

        // Notices
        public string NoApiKeyNotice { get; init; }
        public Func<string, string> NoEntries { get; init; }
        public Func<int, string> LogEntries { get; init; }
        public string InvalidApiKey { get; init; }
        public Func<DateTime, string> LockedOut { get; init; }

        // API key widget
        public string ClearKey { get; init; }
        public string ApiKeyPlaceholder { get; init; }
        public string Save { get; init; }

        // Not Found page
        public string NotFoundTitle { get; init; }
        public string NotFoundMessage { get; init; }

        // Footer links
        public string FooterApiSource { get; init; }
        public string FooterClientSource { get; init; }

        public static readonly LocalisationStrings English = new()
        {
            TitleToday = "Today",
            TitleYesterday = "Yesterday",
            TitleEntries = "Entries",
            PreviousDay = "Previous day",
            NextDay = "Next day",
            Loading = "Loading…",
            Refresh = "Refresh",
            SortAscending = "Sort ascending",
            SortDescending = "Sort descending",
            NoApiKeyNotice = "Set an API key in the top bar to load entries.",
            NoEntries = date => $"No entries for {date}.",
            LogEntries = count => $"{count} log entries",
            InvalidApiKey = "Invalid API key.",
            LockedOut = until => $"Too many failed attempts. Requests blocked until {until:HH:mm}.",
            ClearKey = "Clear Key",
            ApiKeyPlaceholder = "API Key",
            Save = "Save",
            NotFoundTitle = "Not Found",
            NotFoundMessage = "Sorry, the content you are looking for does not exist.",
            FooterApiSource = "API Source",
            FooterClientSource = "Client Source"
        };

        public static readonly LocalisationStrings Romanian = new()
        {
            TitleToday = "Astăzi",
            TitleYesterday = "Ieri",
            TitleEntries = "Intrări",
            PreviousDay = "Ziua anterioară",
            NextDay = "Ziua următoare",
            Loading = "Se încarcă…",
            Refresh = "Reîncarcă",
            SortAscending = "Sortare crescătoare",
            SortDescending = "Sortare descrescătoare",
            NoApiKeyNotice = "Setați o cheie API în bara de sus pentru a încărca intrările.",
            NoEntries = date => $"Nu există intrări pentru {date}.",
            LogEntries = count => $"{count} intrări în jurnal",
            InvalidApiKey = "Cheie API invalidă.",
            LockedOut = until => $"Prea multe încercări eșuate. Cererile sunt blocate până la {until:HH:mm}.",
            ClearKey = "Șterge cheia",
            ApiKeyPlaceholder = "Cheia API",
            Save = "Salvează",
            NotFoundTitle = "Pagina nu a fost găsită",
            NotFoundMessage = "Ne pare rău, conținutul căutat nu există.",
            FooterApiSource = "Sursă API",
            FooterClientSource = "Sursă Client"
        };
    }
}
