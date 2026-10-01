# Documentation Maintenance

**Purpose:** Keep this knowledge base synchronised with implementation changes.

**Scope:** All documents under `docs/` plus relevant root documentation.

**Primary source areas:** [docs/README.md](README.md), [Change guide](change-guide.md), and repository source/tests.

**Related documents:** [Documentation coverage](documentation-coverage.md), [Architecture](architecture.md), [Invariants](invariants.md).

## Maintenance rules

- Behavioural changes require the affected [behaviour](behaviour/) and [flow](flows/) documents to be reviewed.
- Architectural changes require [architecture](architecture.md), affected component documents, [repository structure](repository-structure.md), and [dependencies](dependencies.md) review.
- New or changed configuration requires [configuration](configuration.md), settings tests, and relevant operational documentation.
- New integrations require [integrations](integrations.md), [interfaces](interfaces.md), [security](security.md), and dependency review.
- Persistence or representation changes require [data model](data-model.md), [state and persistence](state-and-persistence.md), and invariant review.
- Error, retry, timeout, or lockout changes require [error handling](error-handling.md), [security](security.md), concurrency review, and tests.
- New or changed test architecture requires [testing](testing.md) and coverage review.
- Development, release, or runtime changes require [build and deployment](build-and-deployment.md) and the root [README](../README.md) when user-facing.
- Unresolved source ambiguity belongs in [ambiguities and open questions](ambiguities-and-open-questions.md), not as invented certainty.

## Change-impact matrix

| Code change | Documents to review |
|---|---|
| Host, DI, middleware | Architecture, host component, configuration, build/deployment |
| UI component or route | Presentation component, relevant behaviour and flow, testing |
| API method or endpoint | Application services, integration models, data model, interfaces, errors, security, tests |
| Model or serialisation | Integration models, data model, interfaces, invariants, tests |
| Browser storage | Browser state component, state/persistence, security, configuration, tests |
| Locale or labels | Browser state component, behaviour, testing, README if user-facing |
| PWA/static assets | Repository structure, integrations, build/deployment, security where relevant |
| Release procedure | Build/deployment, integrations, security, README |

## Quality checks

Verify repository-relative links after adding references, run `dotnet restore`, `dotnet build`, and `dotnet test`, inspect the coverage audit, and ensure secrets or real personal data were not copied into documentation. Keep terminology aligned with exact class, method, and configuration names.
