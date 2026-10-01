# Browse And Search Behaviour

**Purpose:** Describe the normal list workflow, date navigation, ordering, and search semantics.

**Scope:** [Home.razor](../../PersonalLogManager.Client/Pages/Home.razor) and [PersonalLogService.cs](../../PersonalLogManager.Client/Services/PersonalLogService.cs).

**Primary source areas:** `Home` lifecycle and event handlers; `GetLogsForDateAsync`; `SearchLogsAsync`; [GetLogsRequest](../../PersonalLogManager.Client/Models/GetLogsRequest.cs); [LogEntry](../../PersonalLogManager.Client/Models/LogEntry.cs).

**Related documents:** [Presentation](../components/presentation.md), [Calendar and search flow](../flows/calendar-search.md), [Data model](../data-model.md), [Error handling](../error-handling.md).

## Calendar mode

Calendar mode is the default. On initialisation, or after refresh, date navigation, order change, locale change, or a completed mutation, `Home.LoadEntries` checks for a key, sets loading state, clears errors and selection, then calls `GetLogsForDateAsync(currentDate, ascending: ascending)`. The service requests `GET /PersonalLog` with the date, count default `1000`, and locale code.

A successful response is parsed into list entries. The API response order is retained for ascending mode and reversed for descending mode. The page renders text only, count, and notices for no entries. The current date title distinguishes today, yesterday, and other dates.

Previous-day navigation has no lower date limit. Next-day navigation and date-picker selection reject dates later than today. The custom picker accepts dates from 1995 through 2099 at its numeric input boundary, but `Home` also rejects future dates.

## Search mode

Switching to search clears current entries and selection and does not call the API. Submitting a non-blank term trims it into `appliedSearchTerm`, then loads. `SearchLogsAsync` returns an empty list without an API call for blank or whitespace input.

For a non-blank term, the service requests `GET /PersonalLog` with `Date = null`, `Count = 100000`, and the selected locale, then filters only `LogEntry.Text` with ordinal case-insensitive containment. IDs and dates are not searchable. Search results use the same ascending or descending ordering as the fetched list; the page displays dates beside result text.

Editing the input does not alter the applied term. Refresh and order changes reuse the applied term, not the unsubmitted input. Returning to calendar mode immediately reloads the current date.

## Success and empty states

No API key displays a localised notice and prevents requests. Loading displays a notice and disables relevant controls. An empty calendar displays the selected date; an empty search displays either the no-term prompt or the submitted term in a no-results message. List exceptions clear entries and display the exception message.
