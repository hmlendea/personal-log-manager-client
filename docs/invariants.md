# Invariants And Rules

**Purpose:** Collect rules future modifications must preserve or consciously revise.

**Scope:** Cross-component, transport, state, validation, and lifecycle assumptions.

**Primary source areas:** [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs), [Home.razor](../PersonalLogManager.Client/Pages/Home.razor), [EntryDetailPanel.razor](../PersonalLogManager.Client/Layout/EntryDetailPanel.razor), and tests.

**Related documents:** [Data model](data-model.md), [Interfaces](interfaces.md), [Design decisions](design-decisions.md), [Change guide](change-guide.md).

## Rules

| Invariant | Established by | Required by | Violation consequence |
|---|---|---|---|
| API operations use `/PersonalLog` and `/PersonalLog/{id}` paths | `PersonalLogService` | Upstream API contract | Requests fail or target another resource. |
| API-key bearer metadata is supplied on every operation | `PersonalLogService` | API authentication | Requests are unauthorised. |
| `AUTHENTICATION_FAILURE` and `UNAUTHORISED` trigger failure tracking | Façade error branches | Lockout and user messaging | Invalid keys may bypass intended client lockout. |
| Fifth failure within ten minutes locks current scope for thirty minutes | `ApiKeyRateLimitService` | Service preflight checks | Brute-force behaviour and tests change. |
| List strings parse as `Ldigits date: text` | Regex and tests | Detail selection and search/date display | Entries may remain visible but lose usable IDs/dates. |
| Descending order reverses API result; ascending retains it | `GetLogsForDateAsync` | Calendar/search display and tests | Ordering controls become misleading. |
| Search matches final text only and case-insensitively | `SearchLogsAsync` | Search semantics | IDs/dates become unintended searchable fields. |
| Edit JSON contains `data` | `EntryDetailPanel.ExecuteEditAsync` | Update request construction | Save is rejected locally. |
| Successful mutations notify parent before reload | Panel callbacks and `Home` handlers | List consistency | Stale selection or list state may remain. |
| Supported locales are exactly `en-GB` and `ro-RO` | `LocaleService` | Localisation selection | Unsupported values fall back or are ignored. |
| Future dates are rejected by picker and Home | Date picker and handlers | Journal navigation | API may receive unsupported future queries. |
| Components unsubscribe event handlers when disposed | `IDisposable` implementations | Circuit lifecycle | Retained subscriptions can cause stale updates. |

These are implementation invariants, not guarantees supplied by the upstream API unless explicitly stated as an external contract.
