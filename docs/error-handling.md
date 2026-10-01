# Error Handling And Resilience

**Purpose:** Record failure taxonomy, propagation, recovery, and absent resilience mechanisms.

**Scope:** Service, component, browser, host, and external-operation failures visible in source.

**Primary source areas:** [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs), [ApiKeyRateLimitService.cs](../PersonalLogManager.Client/Services/ApiKeyRateLimitService.cs), [Home.razor](../PersonalLogManager.Client/Pages/Home.razor), and [EntryDetailPanel.razor](../PersonalLogManager.Client/Layout/EntryDetailPanel.razor).

**Related documents:** [Security](security.md), [Concurrency](concurrency-and-scheduling.md), [Browse and search](behaviour/browse-and-search.md), [Detail mutation](flows/detail-mutation.md).

## Categories and boundaries

- **Missing credential:** `Home` stops before API access and displays a localised notice.
- **Lockout:** The façade rejects every operation before reading or sending a request while its scoped tracker is locked.
- **Recognised authentication failure:** Exact error codes record a failure and raise localised `InvalidOperationException`.
- **Malformed list item:** The parser preserves raw text and creates an empty ID/date rather than failing the whole list.
- **Unexpected list/detail response:** List returns empty; detail returns null. Mutation methods do not add a type-specific failure check.
- **Invalid edit JSON:** The panel reports invalid JSON or missing `data`; no API call occurs for parse failure.
- **Network, storage, or other API exception:** Propagates to the component catch block where the relevant operation has one, and is displayed using `Exception.Message`.
- **Unhandled host/framework failure:** Uses ASP.NET Core/Blazor defaults, including the browser error UI assets where applicable.

## Rate-limit recovery

Five authentication failures recorded within ten minutes produce a thirty-minute lockout. The lock is circuit-scoped, uses UTC for comparison, and has no manual reset except service disposal or clearing/recreating the circuit. Clearing the browser API key does not reset the scoped tracker.

## Retry and partial failure

No explicit retry, timeout, fallback API endpoint, transaction, compensation, or circuit breaker is implemented. A successful remote mutation followed by a failed reload can leave the UI list stale while the remote record has changed; the parent callback is invoked only after the mutation call itself succeeds.

## Logging and exposure

No repository-owned logging or telemetry service is registered. UI error display exposes exception messages, and API-key values are not deliberately written to logs by this repository. Future error changes should preserve credential and personal-log confidentiality.
