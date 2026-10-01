# Repository Overview

**Purpose:** Describe the system purpose and conceptual boundaries.

**Scope:** The current client process, browser interaction, upstream API boundary, tests, and delivery artefacts.

**Primary source areas:** [Program.cs](../PersonalLogManager.Client/Program.cs), [Startup.cs](../PersonalLogManager.Client/Startup.cs), [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs), [Home.razor](../PersonalLogManager.Client/Pages/Home.razor), and [PersonalLogManagerClient.csproj](../PersonalLogManager.Client/PersonalLogManagerClient.csproj).

**Related documents:** [Architecture](architecture.md), [Data model](data-model.md), [Integrations](integrations.md), [Build and deployment](build-and-deployment.md).

## Purpose and problems solved

The application supplies a browser interface over the Personal Log Manager API. It reduces the need for direct API interaction by providing date navigation, whole-journal text search, detail inspection, JSON editing, deletion confirmation, API-key entry, localisation, and presentation preferences.

The upstream API remains the source of truth for logs. This repository does not contain a database, repository implementation, migration, or log file store. It owns the client process, UI state, API request construction, response parsing, authentication metadata, local browser preferences, and user-visible error translation.

## Principal consumers and use cases

- A person loads entries for today, yesterday, or a selected historical date.
- A person searches final displayed entry text across the journal.
- A person opens a list item to retrieve structured detail.
- A person edits date, time, time zone, and arbitrary data through JSON.
- A person deletes an entry after explicit confirmation.
- A person stores or clears the API key and chooses English or Romanian.

## Boundaries

Inside the repository are the ASP.NET Core host, Blazor components, scoped application services, transport models, configuration binding, static assets, tests, and release wrapper. Outside are the API's authentication and authorisation rules, authoritative log persistence, API validation, deployment infrastructure, browser local-storage implementation, and the external deployment helper downloaded by [release.sh](../release.sh).

## Runtime processes

There is one web process. It serves the Razor component application and maintains interactive server circuits. Each circuit invokes the configured API through `NuciAPI.Client`. Browser local storage supplies the API key and UI preferences through JavaScript interop. The process has no worker, scheduler, queue, or background persistence process.

## Inputs and outputs

Inputs include HTTP requests from the browser, local-storage values, configuration providers, date/search/edit controls, and upstream API responses. Outputs include rendered HTML and interactive circuit updates, browser-local storage writes, authenticated API requests, and user-visible notices or errors.

## Architectural style

The implementation is a server-hosted component UI with a composition root, scoped façade services, plain transport models, and an outbound API adapter. The most important ownership rule is that UI components do not construct HTTP requests directly; [PersonalLogService](../PersonalLogManager.Client/Services/PersonalLogService.cs) is the API-facing façade.

## External dependencies

The project targets `net10.0`, references `NuciAPI.Client` version `1.2.3`, and uses ASP.NET Core Razor Components, Blazor interactive server rendering, browser JavaScript interop, Bootstrap assets, and Font Awesome assets. Testing uses NUnit, Moq, and Bunit as declared or transitively restored by the test project.

See [Dependencies](dependencies.md), [Interfaces](interfaces.md), and [Security](security.md) for consequences of these boundaries.
