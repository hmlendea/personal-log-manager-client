# Application Services

**Purpose:** Document the scoped services that coordinate API access, browser state, localisation, and titles.

**Scope:** [PersonalLogService.cs](../../PersonalLogManager.Client/Services/PersonalLogService.cs), [ApiKeyRateLimitService.cs](../../PersonalLogManager.Client/Services/ApiKeyRateLimitService.cs), [ApiKeyService.cs](../../PersonalLogManager.Client/Services/ApiKeyService.cs), [LocaleService.cs](../../PersonalLogManager.Client/Services/LocaleService.cs), [LocalisationStrings.cs](../../PersonalLogManager.Client/Services/LocalisationStrings.cs), and [PageTitleService.cs](../../PersonalLogManager.Client/Services/PageTitleService.cs).

**Primary source areas:** The linked services.

**Related documents:** [Interfaces](../interfaces.md), [Error handling](../error-handling.md), [State and persistence](../state-and-persistence.md), [Concurrency](../concurrency-and-scheduling.md).

## PersonalLogService

This is the API façade. `GetLogsForDateAsync` creates a `GetLogsRequest`, maps the current locale to `en` or `ro`, sends `GET /PersonalLog`, parses each returned string with `^(L\d+)\s+(\d{4}-\d{2}-\d{2}):\s*(.*)`, and reverses the response unless ascending order is requested. `SearchLogsAsync` returns immediately for blank input; otherwise it requests 100000 records with no date and filters parsed `Text` case-insensitively.

`GetLogByIdAsync` sends `GET /PersonalLog/{id}`. `DeleteLogAsync` sends `DELETE /PersonalLog/{id}` with an ID request. `UpdateLogAsync` sends `PUT /PersonalLog/{id}` with date, time, time zone, and arbitrary JSON data. Every operation checks lockout first, reads the API key, and sends a bearer token plus `PersonalLogManagerClient_{Environment.MachineName}` client ID.

Only `AUTHENTICATION_FAILURE` and `UNAUTHORISED` responses are translated. They record a failure and raise a localised invalid-key or lockout exception. Unexpected response types produce an empty list or null for list/detail operations; mutation methods otherwise complete after the client returns. No retry or pagination exists.

## ApiKeyRateLimitService

The service stores UTC failure timestamps and a lock expiry in scoped memory. Failures older than ten minutes are discarded. The fifth failure locks the scope for thirty minutes and clears the failure list. Further failures during lockout are ignored. It is not process-wide or shared between circuits.

## ApiKeyService

`ApiKeyService` wraps JavaScript `localStorage.getItem`, `setItem`, and `removeItem` for `plm_api_key`. It does not validate, encrypt, expire, or redact the value.

## LocaleService and LocalisationStrings

`LocaleService` supports exactly `en-GB` and `ro-RO`. A new service reports Romanian until `InitialiseAsync` reads storage; unsupported stored values also select Romanian. Supported changes persist under `plm_locale` and raise `OnChange`. `LocalisationStrings` contains static English and Romanian label and formatter instances.

## PageTitleService

`PageTitleService` stores a string title and raises `OnChange` only when the new value differs. Layout subscribers invoke `StateHasChanged` asynchronously.
