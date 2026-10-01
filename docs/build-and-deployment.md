# Build, Deployment, And Operations

**Purpose:** Document restoration, compilation, local execution, release, runtime requirements, and operational assumptions.

**Scope:** Solution, project files, launch settings, [release.sh](../release.sh), static assets, and runtime configuration.

**Primary source areas:** [PersonalLogManagerClient.slnx](../PersonalLogManagerClient.slnx), [PersonalLogManagerClient.csproj](../PersonalLogManager.Client/PersonalLogManagerClient.csproj), [launchSettings.json](../PersonalLogManager.Client/Properties/launchSettings.json), and [release.sh](../release.sh).

**Related documents:** [Configuration](configuration.md), [Integrations](integrations.md), [Dependencies](dependencies.md), [Testing](testing.md).

## Restore, build, test, and run

```bash
dotnet restore PersonalLogManagerClient.slnx
dotnet build PersonalLogManagerClient.slnx
dotnet test PersonalLogManagerClient.slnx
dotnet run --project PersonalLogManager.Client/PersonalLogManagerClient.csproj
```

The project targets .NET 10 and restores `NuciAPI.Client` 1.2.3. The web project produces the ASP.NET Core application; the test project produces the test assembly. Build outputs appear in generated `bin/` and `obj/` directories and are not source.

## Local runtime

The README and launch profile identify `http://localhost:5294` as the usual client URL. The default API base URL is `http://localhost:5000`, so a usable local session requires a running Personal Log Manager API and a valid API key. Path-base deployments require matching `server.pathBase` configuration and reverse-proxy routing.

## Release

`release.sh` accepts arguments and downloads the maintainer's .NET 10 helper from GitHub, then executes it with Bash. The helper is not pinned by commit or vendored, so review its current content and network assumptions before execution. No CI workflow file exists in the tracked repository; README badges and release policy are not evidence of an in-repository workflow.

## Runtime and operational limits

There is no repository health check, structured logging, metrics, database migration, worker, or graceful shutdown hook beyond framework defaults. Static content includes Bootstrap, Font Awesome, PWA icons, manifest, and service worker. The service worker caches application resources but does not make remote log data available offline.
