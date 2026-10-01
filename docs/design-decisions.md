# Design Decisions And Non-Obvious Constraints

**Purpose:** Record implementation choices future changes must not simplify accidentally.

**Scope:** Decisions evidenced by current source; historical rationale is marked unknown where absent.

**Primary source areas:** [Startup.cs](../PersonalLogManager.Client/Startup.cs), [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs), [Home.razor](../PersonalLogManager.Client/Pages/Home.razor), and [service-worker.js](../PersonalLogManager.Client/wwwroot/service-worker.js).

**Related documents:** [Architecture](architecture.md), [Invariants](invariants.md), [Ambiguities](ambiguities-and-open-questions.md), [Change guide](change-guide.md).

## Decisions evidenced by implementation

| Decision | Evidence | Consequence |
|---|---|---|
| Use interactive server rendering without prerendering | `App.razor` sets `InteractiveServerRenderMode(prerender: false)` | Component initialisation and JS interop occur after circuit establishment. |
| Centralise API operations in `PersonalLogService` | All four methods call injected `INuciApiClient`; UI injects the façade | API changes should be concentrated at this boundary. |
| Treat API list strings as a client parsing contract | Compiled regex and fallback `LogEntry` construction | List-format changes can silently produce empty IDs/dates. |
| Search by fetching up to 100000 records and filtering locally | `SearchResultMaximumCount` and `SearchLogsAsync` | Search cost and completeness depend on API maximum and returned ordering. |
| Scope rate limiting to a circuit | `ApiKeyRateLimitService` is registered scoped | It is not protection against distributed or cross-circuit attempts. |
| Use browser local storage for credentials and preferences | JS interop keys in services and `Home` | Values persist in the browser, not on the server; security depends on browser context. |
| Reload after successful mutation | `Home.OnEntryDeleted` and `OnEntryEdited` call `LoadEntries` | Remote state is authoritative; no optimistic local mutation. |
| Let upstream validate edited data | Panel checks JSON syntax and presence of `data`, then sends arbitrary values | Date/time/schema errors are remote concerns. |
| Cache static resources through a service worker | `service-worker.js` cache strategies | Offline behaviour is limited to cached app resources, not log data. |

The repository does not expose historical rationales for these decisions; motivations above are behavioural consequences, not asserted author intent.
