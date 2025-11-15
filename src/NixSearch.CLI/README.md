# NixSearch.CLI

A command-line tool for searching NixOS packages and options from your terminal.

## Features

- **Fast Package Search**: Quickly find NixOS packages with rich filtering options
- **Option Discovery**: Search NixOS configuration options
- **Multiple Output Formats**: Text, JSON, YAML, or XML output
- **Advanced Filtering**: Filter by platform, license, maintainer, and more
- **Channel Support**: Search across stable, unstable, or flakes channels
- **Pagination**: Control result size and offset for large result sets

## Installation

### Install as a global .NET tool

```bash
dotnet tool install -g NixSearch.CLI
```

### Update to the latest version

```bash
dotnet tool update -g NixSearch.CLI
```

### Uninstall

```bash
dotnet tool uninstall -g NixSearch.CLI
```

## Usage

The tool is invoked using the `nixsearch` command.

### Basic Syntax

```bash
nixsearch <command> <query> [options]
```

## Commands

### `packages` - Search for NixOS Packages

Search for packages in the Nix package repository.

#### Basic Examples

```bash
# Search for git packages
nixsearch packages git

# Search with specific result count
nixsearch packages python --size 20

# Search in stable channel
nixsearch packages neovim --channel stable

# Filter by platform
nixsearch packages firefox --platform x86_64-linux

# Filter by license
nixsearch packages nginx --license mit

# Search with pagination
nixsearch packages nodejs --size 10 --from 20
```

#### Options

- `--channel <stable|unstable|flakes>` - Specify the channel (default: unstable)
- `--size <number>` - Number of results to return (default: 50)
- `--from <number>` - Offset for pagination (default: 0)
- `--format <text|json|yaml|xml>` - Output format (default: text)
- `--sort <asc|desc>` - Sort order (omit for relevance sorting)
- `--platform <platform>...` - Filter by platform (e.g., x86_64-linux, aarch64-darwin)
- `--package-set <set>...` - Filter by package set (e.g., python3Packages, haskellPackages)
- `--license <license>...` - Filter by license (e.g., mit, gpl3, apache2)
- `--maintainer <name>...` - Filter by maintainer
- `--team <name>...` - Filter by team

#### Advanced Examples

```bash
# Search Python packages for Linux
nixsearch packages requests --platform x86_64-linux --package-set python3Packages

# Find MIT-licensed packages
nixsearch packages web --license mit --size 30

# Search by maintainer
nixsearch packages kubernetes --maintainer "John Doe"

# Get JSON output for scripting
nixsearch packages docker --format json --size 5

# Multiple filters
nixsearch packages compiler \
  --platform x86_64-linux \
  --license mit \
  --size 10 \
  --channel stable
```

### `options` - Search for NixOS Configuration Options

Search for NixOS system configuration options.

#### Basic Examples

```bash
# Search for nginx options
nixsearch options services.nginx

# Search in stable channel
nixsearch options boot.loader --channel stable

# Get more results
nixsearch options networking --size 20
```

#### Options

- `--channel <stable|unstable|flakes>` - Specify the channel (default: unstable)
- `--size <number>` - Number of results to return (default: 50)
- `--from <number>` - Offset for pagination (default: 0)
- `--format <text|json|yaml|xml>` - Output format (default: text)
- `--sort <asc|desc>` - Sort order (omit for relevance sorting)

#### Advanced Examples

```bash
# Search system service options
nixsearch options systemd --size 30

# Export to JSON
nixsearch options hardware --format json

# YAML output for documentation
nixsearch options security --format yaml --size 100
```

## Output Formats

### Text (Default)

Human-readable text output with key information highlighted.

```bash
nixsearch packages git
```

### JSON

Structured JSON for programmatic use.

```bash
nixsearch packages git --format json | jq '.items[0].packageName'
```

### YAML

YAML format for configuration files or documentation.

```bash
nixsearch packages docker --format yaml
```

### XML

XML format for integration with other tools.

```bash
nixsearch packages postgresql --format xml
```

## Configuration

The CLI tool reads configuration from `appsettings.json` in the application directory.

```json
{
  "NixSearch": {
    "ElasticsearchUrl": "https://search.nixos.org/backend",
    "DefaultChannel": "unstable",
    "DefaultPageSize": 50
  }
}
```

You can also configure via environment variables:

```bash
export NixSearch__ElasticsearchUrl="https://search.nixos.org/backend"
export NixSearch__DefaultChannel="stable"
export NixSearch__DefaultPageSize=100
```

## Scripting Examples

### Find All Python Packages

```bash
#!/bin/bash
nixsearch packages python \
  --package-set python3Packages \
  --format json \
  --size 1000 \
  | jq -r '.items[].packageName'
```

### Check Package Availability

```bash
#!/bin/bash
PACKAGE=$1
RESULT=$(nixsearch packages "$PACKAGE" --format json --size 1)
if echo "$RESULT" | jq -e '.total > 0' > /dev/null; then
  echo "Package '$PACKAGE' found!"
else
  echo "Package '$PACKAGE' not found."
fi
```

### List Packages by Maintainer

```bash
nixsearch packages "" \
  --maintainer "Your Name" \
  --format json \
  --size 100 \
  | jq -r '.items[].packageName'
```

## Tips

1. **Use Quotes**: Wrap queries with spaces in quotes:

   ```bash
   nixsearch packages "visual studio code"
   ```

2. **Wildcards**: Use wildcards in queries:

   ```bash
   nixsearch packages "python*" --package-set python3Packages
   ```

3. **Multiple Filters**: Combine filters for precise results:

   ```bash
   nixsearch packages web --license mit --platform x86_64-linux
   ```

4. **Pagination**: Use `--from` and `--size` for large result sets:

   ```bash
   nixsearch packages lib --size 50 --from 100
   ```

5. **JSON Processing**: Pipe JSON output to `jq` for filtering:

   ```bash
   nixsearch packages rust --format json | jq '.items[].version'
   ```

## Requirements

- .NET 9.0 runtime or later
- Internet connection to access search.nixos.org

## See Also

- [NixSearch.Core](../NixSearch.Core/README.md) - The underlying library
- [search.nixos.org](https://search.nixos.org) - Official web interface
- [NixOS Options](https://search.nixos.org/options) - Browse options online

## License

MIT License - see the [LICENSE](../../LICENSE) file for details.
