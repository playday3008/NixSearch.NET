# NixSearch.NET

A comprehensive .NET solution for searching and exploring NixOS packages, options, and flakes using the search.nixos.org Elasticsearch backend.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)

## Overview

NixSearch.NET provides multiple ways to interact with the NixOS package and option search ecosystem:

- **Library**: A robust .NET client library for programmatic access
- **CLI Tool**: A command-line interface for quick searches
- **MCP Server**: A Model Context Protocol server for AI assistant integration

## Projects

### [NixSearch.Core](src/NixSearch.Core/README.md)

The core library providing search functionality for NixOS packages, options, and flakes.

- **NuGet Package**: `NixSearch.Core`
- **Target Framework**: .NET 9.0
- **Features**: Full Elasticsearch query support, strongly-typed models, async/await patterns

### [NixSearch.CLI](src/NixSearch.CLI/README.md)

A command-line tool for searching NixOS packages and options from your terminal.

- **NuGet Package**: `NixSearch.CLI`
- **Tool Command**: `nixsearch`
- **Installation**: `dotnet tool install -g NixSearch.CLI`

### [NixSearch.MCP](src/NixSearch.MCP/README.md)

A Model Context Protocol (MCP) server enabling AI assistants to search NixOS packages and options.

- **NuGet Package**: `NixSearch.MCP`
- **Deployment**: AWS Lambda (ARM64)
- **Protocol**: [Model Context Protocol](https://modelcontextprotocol.io/)

## Quick Start

### Using the Library

```bash
dotnet add package NixSearch.Core
```

```csharp
using NixSearch.Core;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddNixSearchClient();
var provider = services.BuildServiceProvider();

var client = provider.GetRequiredService<INixSearchClient>();
var results = await client.SearchPackagesAsync("git", pageSize: 10);

foreach (var package in results.Items)
{
    Console.WriteLine($"{package.PackageName}: {package.Version}");
}
```

### Using the CLI

```bash
# Install globally
dotnet tool install -g NixSearch.CLI

# Search for packages
nixsearch packages git --size 10

# Search for options
nixsearch options services.nginx --size 5
```

### Using the MCP Server

The MCP server can be deployed to AWS Lambda or run locally. See the [MCP documentation](src/NixSearch.MCP/README.md) for details on integration with AI assistants like Claude.

## Building

```bash
# Build all projects
dotnet build

# Run tests
dotnet test

# Pack NuGet packages
dotnet pack
```

## Requirements

- .NET 9.0 SDK or later
- For AWS Lambda deployment: AWS CLI configured

## Repository Structure

```tree
NixSearch.NET/
├── src/
│   ├── NixSearch.Core/      # Core library
│   ├── NixSearch.CLI/       # CLI tool
│   └── NixSearch.MCP/       # MCP server
├── tests/
│   ├── NixSearch.Core.Tests/
│   ├── NixSearch.CLI.Tests/
│   └── NixSearch.MCP.Tests/
├── .github/
│   └── workflows/           # CI/CD workflows
└── NixSearch.sln            # Solution file
```

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built on top of the [search.nixos.org](https://search.nixos.org) infrastructure
- Uses [NEST](https://github.com/elastic/elasticsearch-net) for Elasticsearch integration
- MCP protocol implementation from [ModelContextProtocol](https://github.com/modelcontextprotocol/dotnet-sdk)
