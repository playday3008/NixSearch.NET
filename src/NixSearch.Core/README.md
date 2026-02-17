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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NixSearch.Core.Extensions;
using NixSearch.Core.Search;

// Configure services
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build()
    .GetSection("NixSearch");

ServiceCollection services = new ServiceCollection();
services.AddNixSearch(configuration);
ServiceProvider provider = services.BuildServiceProvider();

// Get client
INixSearchClient client = provider.GetRequiredService<INixSearchClient>();
```

### Searching Packages

```csharp
using NixSearch.Core.Models;

// Discover available channels
IReadOnlyList<NixChannel> channels = await client.GetChannelsAsync();

// Simple package search
Nest.ISearchResponse<NixPackage> results = await client.Packages()
    .WithQuery("git")
    .Page(0, 10)
    .ExecuteAsync();

foreach (NixPackage package in results.Documents)
{
    Console.WriteLine($"{package.Name} {package.Version}");
    Console.WriteLine($"Description: {package.Description}");
}

// Advanced package search with filters
Nest.ISearchResponse<NixPackage> filtered = await client.Packages()
    .WithQuery("python")
    .ForChannel(NixChannel.Unstable)
    .WithPlatform("x86_64-linux")
    .WithLicense("mit")
    .Page(0, 20)
    .ExecuteAsync();
```

### Searching Options

```csharp
using NixSearch.Core.Models;

// Search NixOS options
Nest.ISearchResponse<NixOption> options = await client.Options()
    .WithQuery("services.nginx")
    .Page(0, 10)
    .ExecuteAsync();

foreach (NixOption option in options.Documents)
{
    Console.WriteLine($"{option.Name}");
    Console.WriteLine($"Type: {option.Type}");
    Console.WriteLine($"Description: {option.Description}");
}
```

## Configuration

### Using appsettings.json

```json
{
  "NixSearch": {
    "Url": "https://search.nixos.org/backend",
    "Username": "your-username",
    "Password": "your-password",
    "MappingSchemaVersion": 44,
    "Timeout": "00:00:30",
    "MaxRetries": 5,
    "MaxRetryTimeout": "00:02:00",
    "EnableDebugMode": false
  }
}
```

### Programmatic Configuration

```csharp
using NixSearch.Core.Configuration;

services.AddNixSearch(new NixSearchOptions
{
    Url = "https://search.nixos.org/backend",
    Username = "your-username",
    Password = "your-password",
});
```

## Advanced Usage

### Fluent API Methods

#### Package Search Builder

- `WithQuery(string)` - Set search query
- `Page(int from, int size)` - Set pagination offset and page size (default: 0, 50)
- `ForChannel(NixChannel)` - Filter by channel (stable, unstable, beta, flakes)
- `SortBy(SortOrder?)` - Set sort order
- `WithPlatform(params string[])` - Filter by platforms (e.g., "x86_64-linux")
- `WithLicense(params string[])` - Filter by licenses
- `WithMaintainer(params string[])` - Filter by maintainer names
- `WithTeam(params string[])` - Filter by team names
- `WithPackageSet(params string[])` - Filter by package sets (e.g., "python3Packages")
- `ExecuteAsync()` - Execute the search

#### Option Search Builder

- `WithQuery(string)` - Set search query
- `Page(int from, int size)` - Set pagination offset and page size
- `ForChannel(NixChannel)` - Filter by channel
- `SortBy(SortOrder?)` - Set sort order
- `ExecuteAsync()` - Execute the search

### Working with Results

```csharp
Nest.ISearchResponse<NixPackage> results = await client.Packages()
    .WithQuery("neovim")
    .ExecuteAsync();

// Access metadata
Console.WriteLine($"Total hits: {results.Total}");
Console.WriteLine($"Results returned: {results.Documents.Count}");

// Process packages
foreach (NixPackage package in results.Documents)
{
    Console.WriteLine($"Package: {package.Name}");
    Console.WriteLine($"Version: {package.Version}");
    Console.WriteLine($"Description: {package.Description}");

    if (package.License.Length > 0)
        Console.WriteLine($"License: {string.Join(", ", package.License.Select(l => l.FullName))}");

    if (package.Platforms.Length > 0)
        Console.WriteLine($"Platforms: {string.Join(", ", package.Platforms)}");

    if (package.Maintainers.Length > 0)
        Console.WriteLine($"Maintainers: {string.Join(", ", package.Maintainers.Select(m => m.Name))}");
}
```

### Error Handling

```csharp
using NixSearch.Core.Exceptions;

try
{
    Nest.ISearchResponse<NixPackage> results = await client.Packages()
        .WithQuery("package-name")
        .ExecuteAsync();
}
catch (NixSearchException ex)
{
    Console.Error.WriteLine($"Search failed: {ex.Message}");
}
```

## Models

### NixPackage

Core properties:

- `AttrName` - Package attribute name (e.g., "git")
- `Name` - Package name (e.g., "git")
- `Version` - Package version
- `Description` - Package description
- `LongDescription` - Detailed description
- `MainProgram` - Main program name
- `License` - License information (`License[]`)
- `Homepage` - Project homepage URLs (`string[]?`)
- `Platforms` - Supported platforms (`string[]`)
- `Maintainers` - Package maintainers (`Maintainer[]`)
- `Position` - Source location in nixpkgs

### NixOption

Core properties:

- `Name` - Full option path (e.g., "services.nginx.enable")
- `Description` - Option description
- `Type` - Option type (e.g., "boolean", "string")
- `Default` - Default value
- `Example` - Example value
- `Source` - Source location

### NixFlake

`NixFlake` is the abstract base record for both `NixPackage` and `NixOption`.

Core properties:

- `FlakeName` - Flake identifier
- `FlakeDescription` - Flake description
- `FlakeResolved` - Resolved flake repository information (`Repo`)
- `FlakeRevision` - Flake revision hash

## Supported Channels

Channels are resolved dynamically from the backend via `GetChannelsAsync()`:

- `NixChannel.Unstable` - Rolling release (nixos-unstable)
- `NixChannel.Flakes` - Flakes channel
- `NixChannel.Parse("stable", channels)` - Latest stable release (dynamically resolved)
- `NixChannel.Parse("beta", channels)` - Latest beta release (dynamically resolved)

## Requirements

- .NET 10 or later
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
