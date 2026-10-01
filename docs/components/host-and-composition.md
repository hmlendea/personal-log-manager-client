# Host And Composition

**Purpose:** Document process startup, configuration binding, service registration, middleware, and component endpoint mapping.

**Scope:** [Program.cs](../../PersonalLogManager.Client/Program.cs), [Startup.cs](../../PersonalLogManager.Client/Startup.cs), [ServiceCollectionExtensions.cs](../../PersonalLogManager.Client/ServiceCollectionExtensions.cs), and configuration classes.

**Primary source areas:** [Program.cs](../../PersonalLogManager.Client/Program.cs), [Startup.cs](../../PersonalLogManager.Client/Startup.cs), [ServiceCollectionExtensions.cs](../../PersonalLogManager.Client/ServiceCollectionExtensions.cs), [ServerSettings.cs](../../PersonalLogManager.Client/Configuration/ServerSettings.cs), [PersonalLogManagerSettings.cs](../../PersonalLogManager.Client/Configuration/PersonalLogManagerSettings.cs).

**Related documents:** [Architecture](../architecture.md), [Configuration](../configuration.md), [Startup flow](../flows/startup-and-rendering.md), [Build and deployment](../build-and-deployment.md).

## Responsibilities and boundaries

`Program.Main` creates and runs one `WebApplication`. `Startup.ConfigureServices` adds Razor Components and interactive server components, then delegates settings and custom service registration. `Startup.Configure` applies HTTP pipeline behaviour and maps Razor components.

This layer owns composition and middleware only. It does not load log records, inspect API responses, or maintain user workflow state.

## Registration

`AddConfigurations` binds `server` to `ServerSettings` and `personalLogManager` to `PersonalLogManagerSettings`, registering both as singletons. `AddCustomServices` registers `INuciApiClient` and the five application services as scoped. `NuciApiClient` uses the configured base URL or `http://localhost:5000` when the setting is null.

## Middleware and endpoint conduct

When `ServerSettings.PathBase` is non-empty, `UsePathBase` is applied. Static files use a custom content-type provider for `.woff2`, `.woff`, and `.ttf`. Routing and antiforgery are enabled, then Razor components are mapped with interactive server render mode.

No custom authentication middleware, exception middleware, health endpoint, controller, database, or background hosted service is registered.

## Failure behaviour

Configuration binding itself does not validate values. A malformed or unreachable API URL fails later when `NuciApiClient` sends a request. Missing settings use the client base URL fallback but not a `PathBase` fallback beyond empty behaviour. Static-file and framework failures are handled by ASP.NET Core defaults.
