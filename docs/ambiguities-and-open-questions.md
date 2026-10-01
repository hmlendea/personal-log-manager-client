# Ambiguities And Open Questions

**Purpose:** Preserve uncertainty that source evidence does not resolve.

**Scope:** Conflicts, undocumented rationale, and externally controlled behaviour discovered during repository analysis.

**Primary source areas:** [index.html](../PersonalLogManager.Client/wwwroot/index.html), [App.razor](../PersonalLogManager.Client/App.razor), [README.md](../README.md), [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs), and [release.sh](../release.sh).

**Related documents:** [Architecture](architecture.md), [Integrations](integrations.md), [Security](security.md), [Coverage audit](documentation-coverage.md).

## Current ambiguities

1. **Static shell discrepancy.** `App.razor` is configured for Blazor Server interactive rendering, while `wwwroot/index.html` contains WebAssembly framework references and an app-shell loading UI. The active hosting path is determined by `App.razor` and endpoint mapping; the intended status of `index.html` is not established. This matters before changing hosting or PWA bootstrapping.
2. **PWA registration path.** `index.html` registers `/service-worker.js`, but the active `App.razor` shell does not visibly include that script. It is unclear whether the service worker is currently registered in production. This matters before claiming or changing offline/install behaviour.
3. **API error coverage.** The façade explicitly translates only two error codes. The response and retry behaviour for other API errors is delegated to `NuciAPI.Client` and the upstream API, not established here.
4. **Search maximum rationale.** `SearchResultMaximumCount` is `100000`; no source explains whether this is an API maximum, performance compromise, or completeness guarantee.
5. **Client ID semantics.** `Environment.MachineName` is included in the client ID. The upstream API's use, retention, or privacy treatment is outside this repository.
6. **Configuration validation.** Settings bind without explicit validation. Requiredness of `baseUrl`, accepted path-base formats, and deployment overrides are not defined locally.
7. **Release helper trust.** `release.sh` downloads a mutable branch script. Its exact release actions and security guarantees cannot be established without inspecting that external script at execution time.
8. **Accessibility support.** The source contains semantic controls but no repository-level accessibility target, automated accessibility tests, or documented keyboard contract beyond API-key Enter submission.

## Confirmed versus inferred

Confirmed claims in the corpus cite source behaviour. Statements about upstream persistence, API validation, authorisation, deployment infrastructure, and external script conduct are intentionally limited to what this repository demonstrates.
