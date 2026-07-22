[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/personal-log-manager-client)](https://github.com/hmlendea/personal-log-manager-client/releases/latest)
[![Build Status](https://github.com/hmlendea/personal-log-manager-client/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/personal-log-manager-client/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# Personal Log Manager Client

A Blazor Server web application for browsing and managing personal log entries through the [Personal Log Manager](https://github.com/hmlendea/personal-log-manager) API.

## 📑 Table of Contents

- [Features](#-features)
- [Usage](#-usage)
- [Configuration](#️-configuration)
- [Development](#️-development)
  - [Requirements](#requirements)
  - [Build](#build)
  - [Run](#run)
  - [Release](#release)
  - [Dependencies](#dependencies)
- [Contributing](#-contributing)
- [Related Projects](#-related-projects)
- [Support](#-support)
- [License](#-license)

## ✨ Features

- Browse log entries for any past date or today
- Navigate between days using previous/next buttons or a date picker
- View full entry details in a side panel, retrieved via the GET by ID endpoint
- Create, edit, and delete log entries directly from the UI
- Configurable sort order: ascending or descending chronological order
- Entry count displayed below the list
- API key authentication stored in browser local storage
- Localisation support: English and Romanian
- Rate limiting: API key input is blocked for 30 minutes after 5 failed attempts within 10 minutes
- Installable as a Progressive Web App (PWA), compatible with desktop and mobile, including iPhone via Safari
- Offline-capable via service worker caching

## 🚀 Usage

Launch the application and navigate to `http://localhost:5294`. Enter your API key in the top bar on first launch - it will be saved in browser local storage.

Utilise the date navigation controls to browse log entries. Click any entry to open the detail panel. From the panel you can:

- View the full structured entry data returned by the API
- Edit the entry using the ✏️ button, which opens an editable JSON form pre-populated with the current values
- Delete the entry using the 🗑️ button, which requires confirmation before dispatching the request

## ⚙️ Configuration

All settings are loaded from `appsettings.json`. The subsequent keys are recognised:

| Section | Key | Description |
|---------|-----|-------------|
| `server` | `pathBase` | Optional path base for reverse-proxy deployments (e.g. `/logs`) |
| `personalLogManager` | `baseUrl` | Base URL of the Personal Log Manager API instance |

## 🛠️ Development

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A running [Personal Log Manager](https://github.com/hmlendea/personal-log-manager) API instance

All NuGet dependencies are restored automatically by `dotnet restore`.

### Build

```bash
dotnet build PersonalLogManagerClient.csproj
```

### Run

```bash
dotnet run
```

### Release

The repository includes `release.sh`, which delegates to the upstream deployment script used by the project maintainer.

```bash
bash ./release.sh 1.0.0
```

This script downloads and executes an external release helper from `https://raw.githubusercontent.com/hmlendea/deployment-scripts/master/release/dotnet/10.0.sh`.

**Note:** Piping into `bash` is an intensely controversial topic. Please review any external scripts before running them in your environment!

### Dependencies

| Package | Purpose |
|---------|---------|
| `NuciAPI.Client` | HTTP client and request/response infrastructure for communicating with the Personal Log Manager API |

## 🤝 Contributing

You are welcome to bring any suggestion, feedback or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Maintain the pull requests as focused and consistent with the existing code style
- Revise the documentation when behaviour changes

## 🔗 Related Projects

- [Personal Data Logger](https://github.com/hmlendea/personal-data-logger): The data collection tool that feeds into the Personal Log Manager
- [Personal Log Manager](https://github.com/hmlendea/personal-log-manager): The server-side API that this client communicates with

## 💝 Support

Discovered a bug or have a suggestion? [Open an issue](https://github.com/hmlendea/personal-log-manager-client/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 📄 License

Licensed under the `GNU General Public License v3.0` or later.
See [LICENSE](./LICENSE) for details.
