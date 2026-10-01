# Configuration

**Purpose:** Inventory runtime configuration and precedence visible in the repository.

**Scope:** ASP.NET Core configuration binding, settings objects, browser-local values, and launch configuration.

**Primary source areas:** [appsettings.json](../PersonalLogManager.Client/appsettings.json), [Startup.cs](../PersonalLogManager.Client/Startup.cs), [ServiceCollectionExtensions.cs](../PersonalLogManager.Client/ServiceCollectionExtensions.cs), and [launchSettings.json](../PersonalLogManager.Client/Properties/launchSettings.json).

**Related documents:** [Host and composition](components/host-and-composition.md), [Build and deployment](build-and-deployment.md), [State and persistence](state-and-persistence.md).

## Server configuration

| Key | Bound property | Default in repository | Consumer | Effect |
|---|---|---|---|---|
| `server:pathBase` | `ServerSettings.PathBase` | Empty string | `Startup`, `App.razor` | Applies reverse-proxy path base and base href. |
| `personalLogManager:baseUrl` | `PersonalLogManagerSettings.BaseUrl` | `http://localhost:5000` | `ServiceCollectionExtensions` | Selects `NuciApiClient` API base URL. |

Settings bind during service registration and are singletons. The code does not add custom validation or document a precedence override beyond normal ASP.NET Core configuration providers. Environment-specific files or environment variables are not present in the repository, but standard host configuration may supply them.

## Browser runtime configuration

- `plm_api_key`: runtime credential entered through `ApiKeyWidget`; no repository default.
- `plm_locale`: `en-GB` or `ro-RO`; unsupported values fall back to Romanian.
- `sortAscending`: lowercase `true` or `false`; only `true` enables ascending order.
- `fullWidth`: lowercase `true` or `false`; only `true` enables full-width content.

These values are independent of server settings and persist in browser local storage.

## Operational consequences

An empty or null API base URL causes the client registration factory to use `http://localhost:5000`. A non-empty path base changes both middleware routing and the HTML `<base>` value. The API key is never read from server configuration. See [Security](security.md) for storage implications.
