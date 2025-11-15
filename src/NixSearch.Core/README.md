# NixSearch.Core

A .NET client library for searching Nix packages, options, and flakes using the search.nixos.org Elasticsearch backend.

## Features

- **Strongly-typed Models**: Full type safety with `NixPackage`, `NixOption`, and `NixFlake` models
- **Fluent API**: Intuitive search builder pattern for complex queries
- **Async/Await**: Modern asynchronous programming patterns
- **Dependency Injection**: Built-in DI support with `IServiceCollection` extensions
- **Advanced Filtering**: Filter by platforms, licenses, maintainers, channels, and more
- **Elasticsearch Integration**: Powered by NEST for robust search capabilities

## Installation

```bash
dotnet add package NixSearch.Core
```

## Quick Start

### Basic Setup

```csharp
using NixSearch.Core;
using NixSearch.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

// Configure services
var services = new ServiceCollection();
services.AddNixSearchClient();
var provider = services.BuildServiceProvider();

// Get client
var client = provider.GetRequiredService<INixSearchClient>();
```

### Searching Packages

```csharp
// Simple package search
var results = await client.Packages()
    .WithQuery("git")
    .WithSize(10)
    .ExecuteAsync();

foreach (var package in results.Items)
{
    Console.WriteLine($"{package.PackageName} {package.Version}");
    Console.WriteLine($"Description: {package.Description}");
}

// Advanced package search with filters
var filtered = await client.Packages()
    .WithQuery("python")
    .WithChannel(NixChannel.Unstable)
    .WithPlatforms("x86_64-linux")
    .WithLicenses("mit")
    .WithSize(20)
    .WithFrom(0)
    .ExecuteAsync();
```

### Searching Options

```csharp
// Search NixOS options
var options = await client.Options()
    .WithQuery("services.nginx")
    .WithSize(10)
    .ExecuteAsync();

foreach (var option in options.Items)
{
    Console.WriteLine($"{option.OptionName}");
    Console.WriteLine($"Type: {option.Type}");
    Console.WriteLine($"Description: {option.Description}");
}
```

## Configuration

### Using appsettings.json

```json
{
  "NixSearch": {
    "ElasticsearchUrl": "https://search.nixos.org/backend",
    "DefaultChannel": "unstable",
    "DefaultPageSize": 50
  }
}
```

### Programmatic Configuration

```csharp
services.AddNixSearchClient(options =>
{
    options.ElasticsearchUrl = "https://search.nixos.org/backend";
    options.DefaultChannel = "unstable";
    options.DefaultPageSize = 50;
});
```

## Advanced Usage

### Fluent API Methods

#### Package Search Builder

- `WithQuery(string)` - Set search query
- `WithSize(int)` - Set page size (default: 50)
- `WithFrom(int)` - Set pagination offset
- `WithChannel(NixChannel)` - Filter by channel (stable, unstable)
- `WithPlatforms(params string[])` - Filter by platforms (e.g., "x86_64-linux")
- `WithLicenses(params string[])` - Filter by licenses
- `WithMaintainers(params string[])` - Filter by maintainer names
- `WithTeams(params string[])` - Filter by team names
- `WithAttributes(params string[])` - Filter by package attributes
- `ExecuteAsync()` - Execute the search

#### Option Search Builder

- `WithQuery(string)` - Set search query
- `WithSize(int)` - Set page size
- `WithFrom(int)` - Set pagination offset
- `WithChannel(NixChannel)` - Filter by channel
- `ExecuteAsync()` - Execute the search

### Working with Results

```csharp
var results = await client.Packages()
    .WithQuery("neovim")
    .ExecuteAsync();

// Access metadata
Console.WriteLine($"Total hits: {results.Total}");
Console.WriteLine($"Results returned: {results.Items.Count}");

// Process packages
foreach (var package in results.Items)
{
    Console.WriteLine($"Package: {package.PackageName}");
    Console.WriteLine($"Version: {package.Version}");
    Console.WriteLine($"Description: {package.Description}");

    if (package.License != null)
        Console.WriteLine($"License: {package.License}");

    if (package.Platforms?.Count > 0)
        Console.WriteLine($"Platforms: {string.Join(", ", package.Platforms)}");

    if (package.Maintainers?.Count > 0)
        Console.WriteLine($"Maintainers: {string.Join(", ", package.Maintainers.Select(m => m.Name))}");
}
```

### Error Handling

```csharp
try
{
    var results = await client.Packages()
        .WithQuery("package-name")
        .ExecuteAsync();
}
catch (ElasticsearchClientException ex)
{
    Console.Error.WriteLine($"Search failed: {ex.Message}");
}
```

## Models

### NixPackage

Core properties:

- `PackageName` - Package attribute name
- `PackageProgramName` - Program name
- `Version` - Package version
- `Description` - Package description
- `LongDescription` - Detailed description
- `License` - License information
- `Homepage` - Project homepage
- `Platforms` - Supported platforms
- `Maintainers` - Package maintainers
- `Position` - Source location in nixpkgs

### NixOption

Core properties:

- `OptionName` - Full option path (e.g., "services.nginx.enable")
- `Description` - Option description
- `Type` - Option type (e.g., "boolean", "string")
- `Default` - Default value
- `Example` - Example value
- `Source` - Source location

### NixFlake

Core properties:

- `FlakeName` - Flake identifier
- `Description` - Flake description
- `Resolved` - Resolved flake information
- `Packages` - Available packages in the flake

## Supported Channels

- `NixChannel.Stable` - Latest stable release (e.g., 24.05)
- `NixChannel.Unstable` - Rolling release (nixos-unstable)

## Requirements

- .NET 9.0 or later
- Internet connection to access search.nixos.org

## Architecture

The library is built on:

- **NEST 7.13.2** - Elasticsearch client
- **YamlDotNet** - YAML parsing
- **Microsoft.Extensions.*** - Configuration and DI support

## See Also

- [NixSearch.CLI](../NixSearch.CLI/README.md) - Command-line tool using this library
- [NixSearch.MCP](../NixSearch.MCP/README.md) - MCP server using this library
- [search.nixos.org](https://search.nixos.org) - Official NixOS search

## License

MIT License - see the [LICENSE](../../LICENSE) file for details.
