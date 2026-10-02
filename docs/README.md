# Documentation Index

**Purpose:** Navigate the implementation-grounded knowledge base for Personal Log Manager Client.

**Scope:** Current source, tests, configuration, static assets, release script, and existing repository policies.

**Primary source areas:** [PersonalLogManager.Client](../PersonalLogManager.Client/), [unit tests](../PersonalLogManager.Client.UnitTests/), [integration tests](../PersonalLogManager.Client.IntegrationTests/), [project file](../PersonalLogManager.Client/PersonalLogManagerClient.csproj), [README](../README.md), [ARCHITECTURE.md](../ARCHITECTURE.md), and [SECURITY.md](../SECURITY.md).

**Related documents:** All documents below are mutually linked; [ambiguities and open questions](ambiguities-and-open-questions.md) records matters that source evidence does not settle.

## Repository synopsis

This repository contains a .NET 10 Blazor Server client for browsing, searching, inspecting, editing, and deleting personal log entries owned by a separate Personal Log Manager API. The client owns presentation and browser preferences, but not authoritative log persistence.

## Documentation map

| Area | Document | Use when |
|---|---|---|
| Orientation | [Repository overview](repository-overview.md) | Establish purpose, boundaries, and runtime shape. |
| Architecture | [Architecture](architecture.md) | Trace ownership, dependencies, composition, and topology. |
| Physical layout | [Repository structure](repository-structure.md) | Locate files and preserve placement conventions. |
| Components | [Host and composition](components/host-and-composition.md), [presentation](components/presentation.md), [application services](components/application-services.md), [integration models](components/integration-models.md), [browser state and localisation](components/browser-state-and-localisation.md) | Modify a subsystem. |
| Behaviour | [Browse and search](behaviour/browse-and-search.md), [inspect, edit, and delete](behaviour/inspect-edit-delete.md) | Change user-visible capability. |
| Execution traces | [Startup and rendering](flows/startup-and-rendering.md), [calendar and search](flows/calendar-search.md), [detail mutation](flows/detail-mutation.md) | Reconstruct call order and branches. |
| Data and contracts | [Data model](data-model.md), [interfaces](interfaces.md) | Change request shapes, mapping, or API calls. |
| Runtime concerns | [Configuration](configuration.md), [integrations](integrations.md), [state and persistence](state-and-persistence.md), [error handling](error-handling.md), [security](security.md), [concurrency](concurrency-and-scheduling.md) | Assess operational or cross-cutting impact. |
| Delivery | [Build and deployment](build-and-deployment.md), [dependencies](dependencies.md) | Change packaging, runtime, or packages. |
| Tests | [Testing](testing.md) | Add or adjust verification. |
| Constraints | [Design decisions](design-decisions.md), [invariants](invariants.md), [ambiguities](ambiguities-and-open-questions.md) | Avoid accidental architectural regressions. |
| Agent workflow | [Change guide](change-guide.md), [documentation maintenance](documentation-maintenance.md), [coverage audit](documentation-coverage.md) | Plan modifications and keep knowledge synchronised. |

## Complete document list

- [Architecture](architecture.md)
- [Repository overview](repository-overview.md)
- [Repository structure](repository-structure.md)
- [Data model](data-model.md)
- [Interfaces](interfaces.md)
- [Configuration](configuration.md)
- [Integrations](integrations.md)
- [State and persistence](state-and-persistence.md)
- [Error handling](error-handling.md)
- [Security](security.md)
- [Concurrency and scheduling](concurrency-and-scheduling.md)
- [Testing](testing.md)
- [Build and deployment](build-and-deployment.md)
- [Dependencies](dependencies.md)
- [Design decisions](design-decisions.md)
- [Invariants](invariants.md)
- [Change guide](change-guide.md)
- [Documentation maintenance](documentation-maintenance.md)
- [Documentation coverage](documentation-coverage.md)
- [Ambiguities and open questions](ambiguities-and-open-questions.md)
- [Component: host and composition](components/host-and-composition.md)
- [Component: presentation](components/presentation.md)
- [Component: application services](components/application-services.md)
- [Component: integration models](components/integration-models.md)
- [Component: browser state and localisation](components/browser-state-and-localisation.md)
- [Behaviour: browse and search](behaviour/browse-and-search.md)
- [Behaviour: inspect, edit, and delete](behaviour/inspect-edit-delete.md)
- [Flow: startup and rendering](flows/startup-and-rendering.md)
- [Flow: calendar and search](flows/calendar-search.md)
- [Flow: detail mutation](flows/detail-mutation.md)

## Recommended reading sequences

- **General orientation:** [Repository overview](repository-overview.md) -> [Architecture](architecture.md) -> [Repository structure](repository-structure.md).
- **UI capability change:** [Presentation](components/presentation.md) -> relevant [behaviour](behaviour/browse-and-search.md) or [behaviour](behaviour/inspect-edit-delete.md) -> relevant [flow](flows/calendar-search.md) or [flow](flows/detail-mutation.md) -> [Testing](testing.md).
- **API or model change:** [Application services](components/application-services.md) -> [Integration models](components/integration-models.md) -> [Data model](data-model.md) -> [Interfaces](interfaces.md) -> [Invariants](invariants.md).
- **Deployment or security change:** [Configuration](configuration.md) -> [Integrations](integrations.md) -> [Build and deployment](build-and-deployment.md) -> [Security](security.md).
- **Any change:** [Change guide](change-guide.md), then [Documentation maintenance](documentation-maintenance.md).

## Agent retrieval guidance

Start here, identify the affected capability or component, read its process trace, then inspect only the linked source locations. Consult [ambiguities and open questions](ambiguities-and-open-questions.md) before relying on undocumented intent. Treat current implementation and tests as evidence, and treat README statements as historical or user-facing claims when they conflict with source.
