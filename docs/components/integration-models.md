# Integration Models

**Purpose:** Explain transport and view representations at the API boundary.

**Scope:** [Models](../../PersonalLogManager.Client/Models/) and the request construction in [PersonalLogService.cs](../../PersonalLogManager.Client/Services/PersonalLogService.cs).

**Primary source areas:** `GetLogsRequest`, `GetLogsResponse`, `GetLogByIdRequest`, `GetLogByIdResponse`, `UpdateLogRequest`, `DeleteLogRequest`, and `LogEntry`.

**Related documents:** [Data model](../data-model.md), [Interfaces](../interfaces.md), [Application services](application-services.md).

## Model roles

- `GetLogsRequest` extends `NuciApiRequest` and carries nullable `Date`, integer `Count`, and API localisation (`en` or `ro`).
- `GetLogsResponse` extends `NuciApiSuccessResponse`, contains `logs` as a `List<string>` defaulting to an empty list, and an integer `count`.
- `LogEntry` is the client list model with immutable-after-initialisation `Id`, `Date`, `Text`, and `RawText`.
- `GetLogByIdRequest` is an empty request marker.
- `GetLogByIdResponse` carries nullable string-like fields `id`, `date`, `time`, `timeZone`, `template`, `createdDateTime`, `updatedDateTime`, plus `data` as `Dictionary<string,string>`.
- `UpdateLogRequest` carries `date`, `time`, `timeZone`, and `data` as `Dictionary<string,JsonElement>` so edited values can retain JSON types.
- `DeleteLogRequest` carries `id` and serialises it with the JSON name `id`.
- `EntryViewMode` has `Calendar` and `Search` values.

## Conversion boundaries

The remote list representation is a formatted string. `PersonalLogService.CreateLogEntry` extracts ID, date, and final text when the regex matches; otherwise it preserves the raw value as text and leaves ID and date empty. Detail responses remain structured until `EntryDetailPanel` formats them for display or converts edited JSON into an update request.

The edit panel deliberately narrows only the wrapper fields. It enumerates `data` into cloned `JsonElement` values and does not enforce field names, scalar types, dates, or time zones.

## Defaults and nulls

`GetLogsResponse.Logs` defaults to an empty collection but service code also handles null. A null or malformed list string becomes an empty or raw `LogEntry`. Unexpected success response types become empty list or null detail results. Model property nullability is suppressed by project warning configuration rather than by runtime validation.
