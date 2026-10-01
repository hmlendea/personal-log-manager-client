# External Integrations

**Purpose:** Describe external systems, protocols, libraries, and browser boundaries.

**Scope:** Personal Log Manager API, `NuciAPI.Client`, browser local storage, browser service-worker cache, and external release helper.

**Primary source areas:** [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs), [ServiceCollectionExtensions.cs](../PersonalLogManager.Client/ServiceCollectionExtensions.cs), [ApiKeyService.cs](../PersonalLogManager.Client/Services/ApiKeyService.cs), [service-worker.js](../PersonalLogManager.Client/wwwroot/service-worker.js), and [release.sh](../release.sh).

**Related documents:** [Interfaces](interfaces.md), [Dependencies](dependencies.md), [Security](security.md), [Build and deployment](build-and-deployment.md).

## Personal Log Manager API

The API is required for log data. `PersonalLogService` sends four authenticated operations through `INuciApiClient`. The API key is sent as a bearer token; the client ID includes `Environment.MachineName`. The repository defines no timeout, retry, cache, circuit breaker, or response logging policy. Authentication errors are recognised by exact response code; other failures depend on `NuciAPI.Client` behaviour and are not normalised here.

The API owns log persistence, validation, authorisation, and response semantics. The client assumes the list string format and detail/update field names documented in [Data model](data-model.md).

## NuciAPI.Client

`NuciAPI.Client` version `1.2.3` supplies `INuciApiClient`, `NuciApiClient`, request/response base types, and authentication metadata. It is scoped through the repository composition root, with the configured base URL. Replacing it affects every API operation and requires equivalent typed response and error behaviour.

## Browser integration

JavaScript interop calls browser `localStorage` for credentials and preferences. The service worker caches same-origin app-shell and framework/static resources, but it does not cache API calls as a separate remote-data strategy. Navigation is network-first with cached `/` fallback; framework files are cache-first; other same-origin GETs are network-first with cache fallback.

## Release helper

`release.sh` retrieves `https://raw.githubusercontent.com/hmlendea/deployment-scripts/master/release/dotnet/10.0.sh` with `wget` and pipes it to Bash, passing through script arguments. The helper is not versioned in this repository, so its behaviour is external and can change independently.
