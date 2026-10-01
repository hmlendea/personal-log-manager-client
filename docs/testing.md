# Testing Architecture

**Purpose:** Map tests to production components, behaviour, and known gaps.

**Scope:** [PersonalLogManager.Client.UnitTests](../PersonalLogManager.Client.UnitTests/) and its project declaration.

**Primary source areas:** Test classes in the unit-test project and [PersonalLogManager.Client.UnitTests.csproj](../PersonalLogManager.Client.UnitTests/PersonalLogManager.Client.UnitTests.csproj).

**Related documents:** [Change guide](change-guide.md), [Architecture](architecture.md), [Error handling](error-handling.md).

## Test technology and execution

The project targets `net10.0` and uses NUnit with Moq and Bunit. Restore and run all tests with:

```bash
dotnet test PersonalLogManagerClient.slnx
```

The tests mock `INuciApiClient` and `IJSRuntime`; `HomeTests` uses a Bunit context and loose JS interop. No test requires a live upstream API according to the source inspected.

## Coverage map

| Test class | Validates |
|---|---|
| `PersonalLogServiceTests` | List parsing, order, null/unexpected responses, search filtering, API paths, payloads, auth errors, and lockout short-circuiting. |
| `ApiKeyRateLimitServiceTests` | Initial state, threshold, approximate expiry, and lock stability. |
| `HomeTests` | Default/calendar/search rendering, loading state, navigation, date validation, search display, ordering, locale reload, and mutation callbacks. |
| `ApiKeyServiceTests` | Exact local-storage calls for get, set, and clear. |
| `LocaleServiceTests` | Defaults, supported/unsupported locale handling, storage, and change events. |
| `LocalisationStringsTests` | Formatter output and label population in English and Romanian. |
| `ModelTests` | Model property retention and JSON property names. |
| `PageTitleServiceTests` | Title assignment and change-event suppression for identical values. |
| `ServiceCollectionExtensionsTests` | Configuration binding and registration presence. |

## Important absent coverage

There are no live API integration tests, browser-level end-to-end tests, tests of `EntryDetailPanel` JSON validation, tests of `CustomDatePicker` internals, tests of service-worker caching, tests of real HTTP transport, tests of non-authentication API failures, or tests of concurrent component loads. These absences are documented limitations, not evidence that the paths are unused.

## Test conventions

Tests use descriptive Given/When/Then method names, NUnit assertions, deterministic sample IDs and dates, Moq request predicates, and Bunit DOM assertions. When changing a façade method, update request/response and failure tests; when changing `Home`, update the Bunit workflow tests and test storage values.
