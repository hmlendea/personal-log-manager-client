# Data Model

**Purpose:** Catalogue domain, view, transport, configuration, and browser-state representations.

**Scope:** [Models](../PersonalLogManager.Client/Models/), [Configuration](../PersonalLogManager.Client/Configuration/), and service-owned state.

**Primary source areas:** Model classes, [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs), [Home.razor](../PersonalLogManager.Client/Pages/Home.razor), and [EntryDetailPanel.razor](../PersonalLogManager.Client/Layout/EntryDetailPanel.razor).

**Related documents:** [Integration models](components/integration-models.md), [Interfaces](interfaces.md), [State and persistence](state-and-persistence.md).

## Representations

| Representation | Role | Lifetime | Authoritative owner |
|---|---|---|---|
| Raw list string | API list item, conventionally `L{id} yyyy-MM-dd: text` | One response and parse operation | Upstream API |
| `LogEntry` | Client list and selection model | Component/circuit memory | Client-derived |
| `GetLogByIdResponse` | Structured detail response | Detail panel state | Upstream API |
| `UpdateLogRequest` | Mutation payload with JSON-preserving data values | One request | Client-created, API-validated |
| `DeleteLogRequest` | Delete payload containing ID | One request | Client-created |
| `GetLogsRequest` | Date, count, and language query/body representation | One request | Client-created |
| `GetLogsResponse` | List response and count | One response | Upstream API |
| `ServerSettings` | Optional `PathBase` | Process lifetime | Configuration providers |
| `PersonalLogManagerSettings` | API `BaseUrl` | Process lifetime | Configuration providers |
| Browser storage values | API key and UI preferences | Browser lifetime until cleared | Browser |

## Relationships and transformations

`GetLogsResponse.Logs` maps one-to-one to `LogEntry` records. A successful regex parse supplies `Id`, `Date`, `Text`, and `RawText`; failure supplies empty ID/date and raw text. `Home` stores the list and selected item. Detail data is formatted into a display wrapper, then edit JSON is converted to `UpdateLogRequest` fields.

There is no local persistence schema, database table, index, migration, transaction, or repository. Remote consistency is obtained by reloading after successful mutations.

## Nullability and validation

The project suppresses CS8669 rather than enabling nullable reference types across model code. Runtime validation is limited: blank search input, API lockout, list parsing fallback, required edit `data` property, date-picker parse/future checks, and supported locale values. The upstream API remains responsible for semantic validation of update fields and authorisation.

## Identifiers and dates

List IDs are recognised only when matching `L` followed by digits. Dates are represented as strings, normally `yyyy-MM-dd`; the date picker emits that format. Detail timestamps are strings and are not parsed by the client. Rate-limit times use UTC internally and local time for lockout messages.
