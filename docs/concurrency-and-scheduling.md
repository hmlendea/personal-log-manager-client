# Concurrency And Scheduling

**Purpose:** Explain asynchronous conduct, scope isolation, ordering, and absence of background scheduling.

**Scope:** Blazor circuit events, async service methods, rate-limit state, and service-worker event handlers.

**Primary source areas:** [Home.razor](../PersonalLogManager.Client/Pages/Home.razor), [EntryDetailPanel.razor](../PersonalLogManager.Client/Layout/EntryDetailPanel.razor), [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs), and [ApiKeyRateLimitService.cs](../PersonalLogManager.Client/Services/ApiKeyRateLimitService.cs).

**Related documents:** [Architecture](architecture.md), [State and persistence](state-and-persistence.md), [Error handling](error-handling.md).

## Application concurrency

API and JS interop operations use `async`/`await`. Blazor event handlers run within the circuit's renderer context; UI methods call `StateHasChanged` or `InvokeAsync` where needed. Loading, saving, and deleting flags provide UI-level suppression for selected controls, but no general request coalescing or cancellation token is implemented.

Scoped services isolate API-key failure state per circuit. `ApiKeyRateLimitService` uses a mutable list without locks; the source assumes normal serialized use within a circuit. It is not a global rate limiter.

## Ordering

List ordering is determined after the API response: ascending retains returned order and descending reverses it. Search filters after that ordering. Reloads after mutation occur through a new request and are not protected against an older concurrent request overwriting newer UI state.

## Scheduling and lifecycle

There are no hosted workers, timers, queues, scheduled jobs, or background services in the .NET process. The service worker has browser event handlers for install, activate, and fetch; these are independent of log API operations and only manage static resource caching.

## Cancellation and disposal

No request cancellation is exposed. Components unsubscribe event handlers during disposal. Browser and API calls that outlive a disposed component are not explicitly cancelled by repository code.
