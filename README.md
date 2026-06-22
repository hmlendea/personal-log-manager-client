[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/personal-log-manager-clint)](https://github.com/hmlendea/personal-log-manager-client/releases/latest)
[![Build Status](https://github.com/hmlendea/personal-log-manager-client/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/personal-log-manager-client/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# Personal Log Manager Client

Web client for Personal Log Manager.

## Table of Contents

## Overview

## Requirements

- .NET SDK/runtime with support for `net10.0`

## Configuration

Default configuration is defined in `wwwroot/appsettings.json`:

```json
{
  "PersonalLogManager": {
    "BaseUrl": "http://localhost:5000"
  }
}
```

Important settings:

- `personalLogManager.baseUrl`: the base URL of the Personal Log Manager API instance.

At startup, the app creates the store directory/file automatically if missing.

## Run the web client

```bash
dotnet restore
dotnet run
```

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