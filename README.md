[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/personal-log-manager-client)](https://github.com/hmlendea/personal-log-manager-client/releases/latest)
[![Build Status](https://github.com/hmlendea/personal-log-manager-client/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/personal-log-manager-client/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# Personal Log Manager Client

A Blazor WebAssembly front-end for the [Personal Log Manager](https://github.com/hmlendea/personal-log-manager) API.

## Table of Contents

- [Overview](#overview)
- [Requirements](#requirements)
- [Configuration](#configuration)
- [Running](#running)
- [Development](#development)

## Overview

Personal Log Manager Client is a single-page web application that connects to a running Personal Log Manager API instance and lets you browse your personal log entries by date.

Features:
- Browse log entries for any past date or today
- Navigate between days using previous/next buttons or a date picker
- Entries are displayed in reverse chronological order
- API key authentication stored in browser local storage

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A running [Personal Log Manager](https://github.com/hmlendea/personal-log-manager) API instance

## Configuration

The default configuration is in `wwwroot/appsettings.json`:

```json
{
  "personalLogManager": {
    "baseUrl": "http://localhost:5000"
  }
}
```

| Setting | Description |
|---|---|
| `PersonalLogManager.BaseUrl` | Base URL of the Personal Log Manager API instance |

## Running

```bash
dotnet restore
dotnet run
```

The app will be available at `http://localhost:5294` by default.

On first launch, enter your API key in the top bar. It will be saved in the browser's local storage.

## Development

### Build

```bash
dotnet build
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

This script downloads and executes an external release helper from: `https://raw.githubusercontent.com/hmlendea/deployment-scripts/master/release/dotnet/10.0.sh`

**Note:** Piping into `bash` is an intensely controversial topic. Please review any external scripts before running them in your environment!

## Contributing

Contributions are welcome.

Please:

- keep the pull requests focused and consistent with the existing style
- update the documentation when the behaviour changes

## Related Projects

- [Personal Data Logger](https://github.com/hmlendea/personal-data-logger)
- [Personal Log Manager](https://github.com/hmlendea/personal-log-manager)
- [Personal Log Manager Client](https://github.com/hmlendea/personal-log-manager-client)

## License

Licensed under the GNU General Public License v3.0 or later.
See [LICENSE](./LICENSE) for details.