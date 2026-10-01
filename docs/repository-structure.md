# Repository Structure

**Purpose:** Locate meaningful source, test, configuration, asset, and documentation areas.

**Scope:** Tracked repository files; generated `bin/` and `obj/` are excluded except as build outputs.

**Primary source areas:** Repository root, [PersonalLogManager.Client](../PersonalLogManager.Client/), and [PersonalLogManager.Client.UnitTests](../PersonalLogManager.Client.UnitTests/).

**Related documents:** [Architecture](architecture.md), [Change guide](change-guide.md), [Build and deployment](build-and-deployment.md).

## Annotated tree

```text
.
|-- PersonalLogManager.Client.slnx       Solution entry point
|-- PersonalLogManager.Client/            Executable ASP.NET Core and Blazor project
|   |-- Program.cs                        Process entry point
|   |-- Startup.cs                        Host middleware and endpoint configuration
|   |-- ServiceCollectionExtensions.cs    Settings and DI registration
|   |-- Configuration/                    Bound configuration models
|   |-- Models/                           API and UI data representations
|   |-- Services/                         API façade and cross-cutting services
|   |-- Layout/                           Shared layout and interaction components
|   |-- Pages/                            Routed pages
|   |-- wwwroot/                          Browser static files, PWA assets, libraries
|   |-- appsettings.json                  Runtime configuration defaults
|   |-- Properties/launchSettings.json    Local launch profiles
|   `-- css/                              Project stylesheet source
|-- PersonalLogManager.Client.UnitTests/  NUnit, Moq, and Bunit tests
|-- ARCHITECTURE.md                       Existing high-level architecture document
|-- SECURITY.md                           Security policy and reporting scope
|-- README.md                             User and contributor entry point
|-- release.sh                            External release-helper wrapper
`-- docs/                                 Agent-oriented documentation corpus
```

## Placement rules

- Put host registration and middleware changes in `Startup.cs` or extension methods, not in UI components.
- Put remote log request orchestration in `Services/PersonalLogService.cs`.
- Put transport shapes in `Models/`; preserve the distinction between list, detail, update, and delete representations.
- Put browser-local state access behind the existing service where a reusable concern exists; presentation-only preferences currently remain in `Home.razor`.
- Put routed user views in `Pages/` and shared controls in `Layout/`.
- Put tests in the unit-test project and keep test doubles at the boundary they replace.
- Do not treat `bin/` or `obj/` as source. Do not place secrets in `appsettings.json` or documentation.

## Significant files

| File | Architectural purpose |
|---|---|
| [PersonalLogManagerClient.csproj](../PersonalLogManager.Client/PersonalLogManagerClient.csproj) | Target framework and `NuciAPI.Client` dependency. |
| [appsettings.json](../PersonalLogManager.Client/appsettings.json) | Default `server.pathBase` and `personalLogManager.baseUrl`. |
| [manifest.json](../PersonalLogManager.Client/wwwroot/manifest.json) | PWA identity, display, theme, and icons. |
| [service-worker.js](../PersonalLogManager.Client/wwwroot/service-worker.js) | Cache installation, navigation fallback, and same-origin asset caching. |
| [release.sh](../release.sh) | Downloads and executes the maintainer's .NET 10 deployment helper. |

See [components](components/) for semantic ownership and [coverage audit](documentation-coverage.md) for the source-to-document map.
