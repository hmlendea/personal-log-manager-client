# Documentation Coverage Audit

**Purpose:** Map substantial implementation areas to documentation and expose residual gaps.

**Scope:** Current tracked source, tests, assets, scripts, and existing root documentation.

**Primary source areas:** Repository file inventory, [Repository structure](repository-structure.md), and all linked component documents.

**Related documents:** [Documentation index](README.md), [Documentation maintenance](documentation-maintenance.md), [Ambiguities](ambiguities-and-open-questions.md).

## Area map

| Implementation area | Documentation |
|---|---|
| Host entry, middleware, DI, settings | [Host and composition](components/host-and-composition.md), [Architecture](architecture.md), [Configuration](configuration.md) |
| Routes, app shell, layout | [Presentation](components/presentation.md), [Startup flow](flows/startup-and-rendering.md) |
| Home calendar/search state | [Browse and search](behaviour/browse-and-search.md), [Calendar/search flow](flows/calendar-search.md) |
| Detail, edit, delete | [Inspect/edit/delete](behaviour/inspect-edit-delete.md), [Detail mutation flow](flows/detail-mutation.md) |
| API façade and lockout | [Application services](components/application-services.md), [Error handling](error-handling.md), [Security](security.md) |
| Browser key, locale, title state | [Browser state component](components/browser-state-and-localisation.md) |
| Models and JSON mapping | [Integration models](components/integration-models.md), [Data model](data-model.md), [Interfaces](interfaces.md) |
| PWA manifest and service worker | [Integrations](integrations.md), [Build and deployment](build-and-deployment.md), [Ambiguities](ambiguities-and-open-questions.md) |
| Tests and doubles | [Testing](testing.md) |
| Release script and runtime | [Build and deployment](build-and-deployment.md), [Dependencies](dependencies.md) |
| Root architecture and security policies | [Architecture](../ARCHITECTURE.md), [Security policy](../SECURITY.md), [Repository overview](repository-overview.md) |

## Principal capabilities

Calendar browsing, search, detail retrieval, edit, delete, API-key entry/clear, locale selection, ordering, full-width preference, PWA/static asset delivery, startup, and release are each represented by the documents above. Error and lockout branches are covered in [Error handling](error-handling.md) and process traces.

## Significant gaps and limits

There are no live integration or browser end-to-end tests, no documented upstream API implementation, no local persistence mechanism, and no in-repository CI workflow. Static CSS and third-party minified asset internals are not semantically decomposed because they do not decide application behaviour. The `index.html` versus server-shell discrepancy and service-worker registration uncertainty remain in [Ambiguities](ambiguities-and-open-questions.md).

This audit does not claim 100 percent semantic coverage. It claims that every substantial application source area identified during inventory has a retrieval path, while external behaviour and generated/vendor assets remain bounded or explicitly marked unresolved.
