# Security Model

**Purpose:** Document implemented security controls, trust boundaries, and security assumptions.

**Scope:** API-key handling, browser trust boundary, API transport, antiforgery, input handling, and repository policy.

**Primary source areas:** [ApiKeyService.cs](../PersonalLogManager.Client/Services/ApiKeyService.cs), [ApiKeyWidget.razor](../PersonalLogManager.Client/Layout/ApiKeyWidget.razor), [PersonalLogService.cs](../PersonalLogManager.Client/Services/PersonalLogService.cs), [Startup.cs](../PersonalLogManager.Client/Startup.cs), and [SECURITY.md](../SECURITY.md).

**Related documents:** [Integrations](integrations.md), [Error handling](error-handling.md), [State and persistence](state-and-persistence.md), [Configuration](configuration.md).

## Trust boundaries

1. The browser and its local storage are outside the server process and can expose or modify values to anyone controlling that browser profile.
2. The Blazor Server circuit is the application process boundary for UI state and scoped services.
3. The configured Personal Log Manager API is an external authority for authentication, authorisation, validation, and record persistence.
4. The release helper downloaded from GitHub is an external execution boundary.

## Implemented controls

- API-key input uses an HTML password input with autocomplete disabled and stores only in browser local storage.
- Requests use bearer authentication metadata through `NuciAPI.Client`.
- Recognised authentication failures trigger scoped lockout after five failures in ten minutes for thirty minutes.
- ASP.NET Core antiforgery middleware is enabled.
- External footer links use `rel="noopener noreferrer"` with new-tab targets.
- Edited JSON is parsed before transport and rendered JSON uses `JavaScriptEncoder.UnsafeRelaxedJsonEscaping` for readable output. This is an intentional display setting; it is not a substitute for server validation.
- User-provided search text is rendered through Razor text content, not raw HTML.

## Security assumptions and limitations

The client does not encrypt local storage, expire keys, define API authorisation, validate update semantics, enforce HTTPS, or implement process-wide brute-force protection. The configured API URL may be HTTP in the repository default. The client ID includes machine name, which is sent externally and may be sensitive operational metadata.

Personal log details are rendered to the authenticated browser. The upstream API and deployment environment must provide transport and access controls appropriate to the data.

## Security maintenance

Do not place keys, tokens, personal log examples containing real data, or private endpoints in source or documentation. Review [SECURITY.md](../SECURITY.md) for reporting scope and supported-version policy. Changes to storage, authentication metadata, error display, edit parsing, or release execution require a security review.
