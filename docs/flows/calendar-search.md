# Calendar And Search Flow

**Purpose:** Provide a low-level execution trace for date browsing and whole-journal search.

**Scope:** `Home` event handlers and `PersonalLogService` list methods.

**Primary source areas:** [Home.razor](../../PersonalLogManager.Client/Pages/Home.razor), [PersonalLogService.cs](../../PersonalLogManager.Client/Services/PersonalLogService.cs), [CustomDatePicker.razor](../../PersonalLogManager.Client/Layout/CustomDatePicker.razor).

**Related documents:** [Browse and search](../behaviour/browse-and-search.md), [Application services](../components/application-services.md), [Interfaces](../interfaces.md).

## Calendar trace

1. An initial render, refresh, date event, locale event, order toggle, or mutation callback invokes `Home.LoadEntries`.
2. `ApiKeyService.GetApiKeyAsync` reads `plm_api_key`. Missing or empty values stop the process before the client is called.
3. `Home` sets `loading`, clears `error` and selection, and requests the current ISO date.
4. `PersonalLogService.GetLogsForDateAsync` rejects a locked scope, maps locale to `en` or `ro`, reads the key, builds `GetLogsRequest`, and builds bearer/client-id metadata.
5. `INuciApiClient.SendRequestAsync` sends `GET /PersonalLog`.
6. Authentication errors record a failure and raise a localised exception. A typed `GetLogsResponse` is parsed; null logs become an empty list. Each string becomes a `LogEntry`, and descending mode reverses the materialised list.
7. `Home` renders entries or an empty/error state and clears loading in `finally`.

## Search trace

1. Switching to Search only changes view state and clears entries; no request occurs.
2. Form submission trims `searchTerm` into `appliedSearchTerm` and invokes `LoadEntries`.
3. Blank terms return an empty list before API access. Non-blank terms call `GetLogsForDateAsync(null, 100000, ascending)`.
4. The service parses and orders all returned records, then filters `Text` with ordinal case-insensitive containment.
5. `Home` renders the matching dates and text, count, or a no-results message. Refresh and sorting repeat the same process using `appliedSearchTerm`.

## Date validation

`CustomDatePicker` emits only parseable dates not exceeding `Max`; numeric parts are clamped. `Home` rejects empty, unchanged, and future values. Previous-day navigation can move earlier without an explicit lower bound.
