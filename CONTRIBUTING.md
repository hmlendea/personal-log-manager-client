# Contributing to Personal Log Manager Client

This document covers the guidelines and processes for contributing to this project, including how to report issues, suggest enhancements, submit code changes, and follow project standards.

## 📑 Table of Contents

- [How to Contribute](#how-to-contribute)
- [Security](#security)
- [License](#license)
- [Getting Assistance](#getting-assistance)

## 🤝 How to Contribute

### Reporting Issues

- Search existing issues first.
- Use the issue templates if available.
- Provide clear reproduction steps.
- Include environment details.

### Suggesting Enhancements

- Check the roadmap and existing discussions.
- Explain the use case and expected behaviour.
- Consider implementation complexity.

### Code Contributions

#### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A running [Personal Log Manager](https://github.com/hmlendea/personal-log-manager) API instance

#### Development Setup

```bash
# Clone the repository
git clone https://github.com/hmlendea/personal-log-manager-client.git
cd personal-log-manager-client

# Install dependencies
dotnet restore
```

#### Making Changes

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes.
4. Run tests: `dotnet test PersonalLogManagerClient.slnx`
5. Run linters: `dotnet format --verify-no-changes`
6. Commit with clear and descriptive messages.
7. Push to your fork.
8. Open a Pull Request.

#### Pull Request Guidelines

- Target the `master` branch.
- Keep PRs focused and atomic.
- Revise documentation if applicable.
- Include tests for any new functionality.
- Ensure the CI checks pass.

### Code Style

Follow the project's coding standards:
- C# coding conventions as defined in the repository instructions
- Run `dotnet format` before committing.

### Testing

```bash
# Run all tests
dotnet test PersonalLogManagerClient.slnx

# Run specific test suite
dotnet test PersonalLogManager.Client.UnitTests/PersonalLogManager.Client.UnitTests.csproj
dotnet test PersonalLogManager.Client.IntegrationTests/PersonalLogManager.Client.IntegrationTests.csproj
dotnet test PersonalLogManager.Client.ArchitectureTests/PersonalLogManager.Client.ArchitectureTests.csproj
```

### Documentation

- Revise relevant docs for changes.
- Follow the documentation style guide.
- Preview changes locally if possible.

## 🔒 Security

Report security vulnerabilities per the [Security Policy](SECURITY.md).

## 📄 License

By contributing, you agree that your contributions will be licensed under the GNU General Public License v3.0.

## ❓ Getting Assistance

- [Open an issue](https://github.com/hmlendea/personal-log-manager-client/issues)
- Check existing discussions and documentation first.