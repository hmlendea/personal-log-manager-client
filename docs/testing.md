# Testing Architecture

**Purpose:** Map tests to production components, behaviour, and known gaps.

**Scope:** [PersonalLogManager.Client.UnitTests](../PersonalLogManager.Client.UnitTests/), [PersonalLogManager.Client.IntegrationTests](../PersonalLogManager.Client.IntegrationTests/), and their project declarations.

**Primary source areas:** Test classes in both test projects, [PersonalLogManager.Client.UnitTests.csproj](../PersonalLogManager.Client.UnitTests/PersonalLogManager.Client.UnitTests.csproj), and [PersonalLogManager.Client.IntegrationTests.csproj](../PersonalLogManager.Client.IntegrationTests/PersonalLogManager.Client.IntegrationTests.csproj).

**Related documents:** [Change guide](change-guide.md), [Architecture](architecture.md), [Error handling](error-handling.md).

## Test technology and execution

Both projects target `net10.0` and use NUnit. Unit tests use Moq and Bunit. Integration tests use Bunit for component workflows and `Microsoft.AspNetCore.Mvc.Testing` for in-memory HTTP host checks. Restore and run all tests with:

```bash
dotnet test PersonalLogManagerClient.slnx
```

The tests mock `INuciApiClient` and browser JS interop at external boundaries. Integration fixtures use the production services between those boundaries, and host tests execute the configured ASP.NET Core middleware in memory. No test requires a live upstream API.

## Coverage map

| Test class | Validates |
|---|---|
| `PersonalLogServiceTests` | List parsing, order, null/unexpected responses, search filtering, API paths, payloads, auth errors, and lockout short-circuiting. |
| `ApiKeyRateLimitServiceTests` | Initial state, threshold, approximate expiry, and lock stability. |
| `HomeTests` | Default/calendar/search rendering, loading state, navigation, date validation, search display, ordering, locale reload, and mutation callbacks. |
| `ApiKeyWidgetTests` | Authentication-dialog visibility, required-key validation, trimmed storage, and deauthentication. |
| `ApiKeyServiceTests` | Exact local-storage calls for get, set, and clear. |
| `LocaleServiceTests` | Defaults, supported/unsupported locale handling, storage, and change events. |
| `LocalisationStringsTests` | Formatter output and label population in English and Romanian. |
| `ModelTests` | Model property retention and JSON property names. |
| `PageTitleServiceTests` | Title assignment and change-event suppression for identical values. |
| `ServiceCollectionExtensionsTests` | Configuration binding and registration presence. |
| `EntryDetailWorkflowTests` | Detail loading, complete and fallback rendering, JSON edit validation, update/delete payloads, authentication failures, cancellation, and callbacks. |
| `HomeWorkflowIntegrationTests` | Calendar/search workflows through real services, storage preferences, parsing, localisation, lockout, selection, and mutation reloads. |
| `AuthenticationAndLocaleWorkflowTests` | Locale persistence and rerendering, authentication dialog keyboard and pointer behavior, validation reset, API-key storage, and deauthentication. |
| `CustomDatePickerIntegrationTests` | Input boundaries, invalid and maximum dates, callback suppression, calendar lifecycle, navigation, and month/year selection. |
| `HostAndCompositionIntegrationTests` | Configuration binding, scoped lifetimes, service resolution, document shell, static assets, content types, and path-base middleware. |

## Important absent coverage

There are no live upstream API tests, browser-automation end-to-end tests, service-worker caching tests, real socket transport tests, or concurrent circuit/load tests. Non-authentication error semantics remain limited by the production client, which only turns authentication error responses into exceptions. These absences are documented limitations, not evidence that the paths are unused.

## Test conventions

Tests use descriptive Given/When/Then method names, NUnit assertions, deterministic sample IDs and dates, Moq request predicates, and Bunit DOM assertions. Integration tests replace only external boundaries and retain production services. When changing a facade method, update request/response and failure tests; when changing `Home`, update the Bunit workflow tests and test storage values.
