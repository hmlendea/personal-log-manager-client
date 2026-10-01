# State And Persistence

**Purpose:** Catalogue state ownership, lifetime, persistence, consistency, and invalidation.

**Scope:** Server scopes, component fields, browser local storage, service-worker caches, and upstream API records.

**Primary source areas:** [Home.razor](../PersonalLogManager.Client/Pages/Home.razor), [ApiKeyRateLimitService.cs](../PersonalLogManager.Client/Services/ApiKeyRateLimitService.cs), [ApiKeyService.cs](../PersonalLogManager.Client/Services/ApiKeyService.cs), [LocaleService.cs](../PersonalLogManager.Client/Services/LocaleService.cs), and [service-worker.js](../PersonalLogManager.Client/wwwroot/service-worker.js).

**Related documents:** [Architecture](architecture.md), [Configuration](configuration.md), [Concurrency](concurrency-and-scheduling.md), [Data model](data-model.md).

## State locations

| State | Location | Owner | Persistence |
|---|---|---|---|
| Authoritative logs | Upstream API | External system | Remote API storage |
| Current list, date, search, selection, loading/error flags | `Home` component fields | Active circuit/component | Lost on component/circuit disposal |
| Detail/edit/delete flags and JSON | `EntryDetailPanel` fields | Panel instance | Lost on panel close |
| API key | Browser local storage `plm_api_key` | Browser and `ApiKeyService` | Until cleared; no expiry |
| Locale | Browser local storage `plm_locale` and scoped `LocaleService` | Browser/service | Persistent browser preference |
| Sort and width preferences | Browser local storage | `Home` | Persistent browser preference |
| Auth failures and lock expiry | Scoped `ApiKeyRateLimitService` | Active circuit | Lost with scope; lock lasts by UTC timestamp while scope survives |
| Static resources | Browser service-worker cache | Browser | Cache-version lifecycle |

## Consistency

The client does not optimistically update records. After a successful update or delete, `Home` closes the panel and re-reads the active list. During a pending operation, UI flags disable selected controls. Concurrent requests are possible where separate events are raised, but no explicit serialisation or shared-store conflict policy exists.

## Cache invalidation

Service-worker activation deletes caches whose name differs from `personal-log-manager-v1`. Remote API responses are not deliberately cached by the service worker as API data. Browser local-storage values are changed only by explicit UI actions or browser management.

## Persistence boundary

There is no local database, filesystem log store, migration, transaction, or server-side API-key store. A deployment that requires durable local records would be a new architectural responsibility and must not be introduced casually into the UI or façade.
