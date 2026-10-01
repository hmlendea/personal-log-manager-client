# Dependencies

**Purpose:** Describe dependencies by architectural role and replacement impact.

**Scope:** Direct package, framework, browser, asset, and external script dependencies.

**Primary source areas:** [PersonalLogManagerClient.csproj](../PersonalLogManager.Client/PersonalLogManagerClient.csproj), [PersonalLogManager.Client.UnitTests.csproj](../PersonalLogManager.Client.UnitTests/PersonalLogManager.Client.UnitTests.csproj), [ServiceCollectionExtensions.cs](../PersonalLogManager.Client/ServiceCollectionExtensions.cs), and [release.sh](../release.sh).

**Related documents:** [Architecture](architecture.md), [Integrations](integrations.md), [Build and deployment](build-and-deployment.md), [Testing](testing.md).

## Runtime dependencies

| Dependency | Role | Encapsulation | Replacement consequence |
|---|---|---|---|
| .NET 10 / ASP.NET Core | Host, DI, routing, static files, antiforgery, Razor components | Framework APIs throughout host and UI | Requires migration of process and component hosting. |
| `NuciAPI.Client` 1.2.3 | Typed outbound API requests, auth metadata, response hierarchy | Exposed through `INuciApiClient` in service constructor | All request and response integration code must be adapted. |
| Browser JS interop | Local storage and framework interaction | API-key and locale services; direct preference calls | Storage and component lifecycle paths change. |
| Bootstrap assets | CSS framework resources | Static files under `wwwroot/lib/bootstrap` | Visual/layout asset changes. |
| Font Awesome assets | Icons and font resources | Static files and class names in Razor | UI icon classes and MIME mappings change. |
| Service worker/PWA APIs | Browser installation and static caching | `wwwroot/manifest.json` and `service-worker.js` | Offline/install behaviour changes. |

## Test dependencies

NUnit provides assertions and test discovery, Moq replaces API and JS boundaries, and Bunit renders Blazor components. Tests are coupled to public component markup classes and service constructor dependencies.

## Delivery dependency

`release.sh` depends on `wget`, Bash, network access, and the mutable external deployment script. It is operationally distinct from application runtime and must be reviewed as executable supply-chain input.
