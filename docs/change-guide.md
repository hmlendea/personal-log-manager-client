# Agent-Oriented Change Guide

**Purpose:** Give future coding agents a modification map that minimises source rediscovery.

**Scope:** Common changes in this repository and their required architectural, test, and documentation impact.

**Primary source areas:** [Architecture](architecture.md), [Repository structure](repository-structure.md), [Testing](testing.md), and [Invariants](invariants.md).

**Related documents:** All component, behaviour, flow, data, and maintenance documents.

## Add or change an API operation

Start in [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs). Add or revise the request/response model in [Models](../PersonalLogManager.Client/Models/), preserve `INuciApiClient` usage and auth metadata, update [interfaces](interfaces.md), [data model](data-model.md), error semantics, relevant flow documents, and `PersonalLogServiceTests`. Check lockout behaviour and [invariants](invariants.md).

## Change list parsing or search

Modify `CreateLogEntry`, `GetLogsForDateAsync`, or `SearchLogsAsync`. Update [browse and search](behaviour/browse-and-search.md), [calendar flow](flows/calendar-search.md), [data model](data-model.md), parsing/order/search tests, and UI tests in `HomeTests`. Consider malformed input, null logs, result limits, and ordering.

## Change a UI workflow

Start in [Home.razor](../PersonalLogManager.Client/Pages/Home.razor) or [EntryDetailPanel.razor](../PersonalLogManager.Client/Layout/EntryDetailPanel.razor). Trace parent callbacks, loading/error state, service calls, and title/localisation events. Update the corresponding behaviour and flow document plus Bunit tests. Preserve selection clearing and post-mutation reload semantics unless intentionally changing them.

## Add or change a model field

Edit the specific transport model, including `JsonPropertyName` where required. Update [data model](data-model.md), [interfaces](interfaces.md), serialization tests, façade request predicates, and any UI formatting or edit parsing. Confirm null/default semantics and upstream compatibility.

## Add configuration

Add a property to the appropriate settings class, bind it in [ServiceCollectionExtensions.cs](../PersonalLogManager.Client/ServiceCollectionExtensions.cs), provide a non-secret default only when justified, and document it in [configuration](configuration.md). Add binding tests and trace every consumer. Do not put API keys or personal data in `appsettings.json`.

## Add browser state or localisation

Use an existing scoped service for reusable state, or document why a presentation-only preference remains in `Home`. Add storage/event tests, update [state and persistence](state-and-persistence.md), [interfaces](interfaces.md), and localisation tests. Ensure event subscriptions are disposed.

## Add an external integration

Define the abstraction boundary, registration lifetime, configuration, authentication, failure and retry semantics first. Update [integrations](integrations.md), [security](security.md), [dependencies](dependencies.md), [configuration](configuration.md), relevant flows, tests, and [README](../README.md) only for user-visible setup.

## Change deployment or PWA behaviour

Inspect [Startup.cs](../PersonalLogManager.Client/Startup.cs), [manifest.json](../PersonalLogManager.Client/wwwroot/manifest.json), [service-worker.js](../PersonalLogManager.Client/wwwroot/service-worker.js), [release.sh](../release.sh), and launch settings. Update [build and deployment](build-and-deployment.md), [integrations](integrations.md), [security](security.md) when relevant, and validate the solution.

## Complete change checklist

1. Identify the owning component using [Architecture](architecture.md).
2. Trace the relevant process document before editing.
3. Preserve or revise [Invariants](invariants.md) and [Design decisions](design-decisions.md).
4. Update focused production tests and run restore, build, and tests.
5. Update affected docs using [Documentation maintenance](documentation-maintenance.md).
6. Run the coverage and link checks described in [Documentation coverage](documentation-coverage.md).
