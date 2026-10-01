# Interfaces And Contracts

**Purpose:** Describe significant browser, component, and upstream API contracts.

**Scope:** HTTP operations invoked through `INuciApiClient`, JavaScript storage calls, component parameters/callbacks, and configuration keys.

**Primary source areas:** [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs), [ServiceCollectionExtensions.cs](../PersonalLogManager.Client/ServiceCollectionExtensions.cs), and Razor component declarations.

**Related documents:** [Data model](data-model.md), [Integrations](integrations.md), [Configuration](configuration.md).

## Upstream API operations

| Operation | Request type | Path | Expected success type | Semantics |
|---|---|---|---|---|
| List by date/search source | `GetLogsRequest` | `GET /PersonalLog` | `GetLogsResponse` | Returns formatted log strings; `Date` may be null for search. |
| Detail | `GetLogByIdRequest` | `GET /PersonalLog/{id}` | `GetLogByIdResponse` | Returns structured detail. |
| Update | `UpdateLogRequest` | `PUT /PersonalLog/{id}` | `NuciApiSuccessResponse` | Sends date, time, time zone, and arbitrary JSON data. |
| Delete | `DeleteLogRequest` | `DELETE /PersonalLog/{id}` | `NuciApiSuccessResponse` | Sends selected ID. |

All requests carry `NuciApiRequestAuthorisationInfo` with bearer API key and a machine-name-derived client ID. Recognised error codes are `AUTHENTICATION_FAILURE` and `UNAUTHORISED`.

## Browser storage contract

`plm_api_key` stores the API key. `plm_locale` stores `en-GB` or `ro-RO`. `sortAscending` and `fullWidth` store lowercase boolean strings. `ApiKeyService` owns the API key calls; `LocaleService` owns locale calls; `Home` directly uses JS interop for the two presentation preferences.

## Component contracts

`EntryDetailPanel` receives `Entry`, `OnClose`, `OnDeleted`, and `OnEdited`. `CustomDatePicker` receives `Value`, `Max`, `Disabled`, and `ValueChanged`. These callbacks are the parent-child coordination boundary; successful mutations are signalled upward rather than directly changing `Home.entries`.

## Error semantics

The façade raises `InvalidOperationException` for lockout and recognised authentication failures. It returns empty or null for unexpected success response types in list/detail methods. UI components display exception messages without a separate error DTO. Browser storage and network exceptions propagate to the relevant component catch blocks.
