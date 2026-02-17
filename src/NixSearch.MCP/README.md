# NixSearch.MCP

A Model Context Protocol (MCP) server that enables AI assistants to search and explore NixOS packages and options. Deployed on AWS Lambda for serverless operation.

## Features

- **MCP Protocol Support**: Standard Model Context Protocol implementation
- **Package Search**: Search nixpkgs with advanced filtering
- **Option Discovery**: Find NixOS configuration options
- **Detailed Information**: Get comprehensive package and option details
- **Serverless Deployment**: Runs on AWS Lambda (ARM64) with Function URL
- **Stateless by Default**: No session management required for simple search operations
- **AI Assistant Integration**: Works with Claude and other MCP-compatible AI assistants

## MCP Tools

The server exposes four MCP tools:

### `search_packages`

Search for NixOS packages in nixpkgs.

**Parameters:**

- `query` (required): Search query (package name, description keywords)
- `channel`: NixOS channel - `unstable`, `stable`, or `flakes` (default: unstable)
- `platform`: Filter by platforms (e.g., `x86_64-linux`, `aarch64-darwin`)
- `packageSet`: Filter by package sets (e.g., `python3Packages`, `haskellPackages`)
- `license`: Filter by license names
- `maintainer`: Filter by maintainer usernames
- `team`: Filter by team names
- `page`: Page number, 0-indexed (default: 0)
- `size`: Results per page (default: 50)

**Returns:** Package details including name, version, description, maintainers, licenses, platforms, and more.

### `search_options`

Search for NixOS configuration options.

**Parameters:**

- `query` (required): Search query (option name, description keywords)
- `channel`: NixOS channel - `unstable`, `stable`, or `flakes` (default: unstable)
- `page`: Page number, 0-indexed (default: 0)
- `size`: Results per page (default: 50)

**Returns:** Option details including name, type, description, default value, and examples.

### `get_package_details`

Get detailed information about a specific package.

**Parameters:**

- `packageName` (required): Exact package attribute name (e.g., `git`, `python3`)
- `channel`: NixOS channel (default: unstable)

**Returns:** Complete package information including metadata, dependencies, and build information.

### `get_option_details`

Get detailed information about a specific NixOS option.

**Parameters:**

- `optionName` (required): Exact option path (e.g., `services.nginx.enable`)
- `channel`: NixOS channel (default: unstable)

**Returns:** Complete option information including type, description, default, example, and declarations.

## Deployment

### AWS Lambda (Recommended)

The server is designed for AWS Lambda deployment with ARM64 architecture.

#### Prerequisites

- AWS CLI configured with appropriate credentials
- .NET 10 SDK

#### Deploy via GitHub Actions

The repository includes a CI/CD workflow that automatically deploys to AWS Lambda on push to main branch.

#### Manual Deployment

```bash
# Navigate to the project directory
cd src/NixSearch.MCP

# Build and publish for AWS Lambda
dotnet lambda deploy-function NixSearchMCP \
  --region eu-central-1 \
  --function-architecture arm64
```

#### Production Endpoint

The official deployment is available at: <https://zhyxgxr7amp6usu6o2qdeab6ja0bpybd.lambda-url.eu-central-1.on.aws/>

### Local Development

Run the server locally for testing:

```bash
cd src/NixSearch.MCP
dotnet run
```

The server will start on `http://localhost:5000` (or the configured port).

## Integration with AI Assistants

### Claude Desktop

Add to your Claude Desktop configuration (`~/Library/Application Support/Claude/claude_desktop_config.json` on macOS):

```json
{
  "mcpServers": {
    "nixsearch": {
      "url": "https://zhyxgxr7amp6usu6o2qdeab6ja0bpybd.lambda-url.eu-central-1.on.aws",
      "transport": "http"
    }
  }
}
```

### Using with Local Instance

For local development:

```json
{
  "mcpServers": {
    "nixsearch": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/NixSearch.MCP"],
      "transport": "stdio"
    }
  }
}
```

## API Endpoints

The server exposes several HTTP endpoints:

### MCP Endpoint

- **Path**: `/` (root)
- **Purpose**: Main MCP protocol endpoint
- **Transport**: HTTP/HTTPS with MCP protocol

### Health Check

- **Path**: `/health`
- **Method**: GET
- **Response**: `{"status": "healthy", "service": "nixsearch-mcp"}`

### Service Information

- **Path**: `/info`
- **Method**: GET
- **Response**: Server metadata including version, available tools, and endpoints

Example:

```bash
curl https://zhyxgxr7amp6usu6o2qdeab6ja0bpybd.lambda-url.eu-central-1.on.aws/info
```

## Configuration

### appsettings.json

```json
{
  "NixSearch": {
    "ElasticsearchUrl": "https://search.nixos.org/backend",
    "DefaultChannel": "unstable",
    "DefaultPageSize": 50
  },
  "MCP": {
    "Stateless": true,
    "IdleTimeout": "24:00:00"
  }
}
```

### Environment Variables

Configuration can be overridden with environment variables prefixed with `NIXSEARCH_`:

```bash
export NIXSEARCH_NixSearch__ElasticsearchUrl="https://search.nixos.org/backend"
export NIXSEARCH_NixSearch__DefaultChannel="stable"
export NIXSEARCH_MCP__Stateless="true"
```

### Stateless vs Stateful Mode

- **Stateless Mode (default)**: No session tracking, each request is independent. Recommended for Lambda.
- **Stateful Mode**: Maintains session state with configurable idle timeout. Useful for long-running processes.

## Example Usage

### With Claude

Once configured, you can ask Claude:

```prompt
Find the latest version of git in NixOS
```

```prompt
What are the nginx configuration options in NixOS?
```

```prompt
Search for Python packages in nixpkgs that relate to machine learning
```

### With curl (Direct API)

While the MCP protocol is designed for AI assistants, you can test the endpoints:

```bash
# Health check
curl https://zhyxgxr7amp6usu6o2qdeab6ja0bpybd.lambda-url.eu-central-1.on.aws/health

# Service info
curl https://zhyxgxr7amp6usu6o2qdeab6ja0bpybd.lambda-url.eu-central-1.on.aws/info
```

## Architecture

```diagram
┌─────────────────┐
│  AI Assistant   │
│   (e.g. Claude) │
└────────┬────────┘
         │ MCP Protocol
         │
┌────────▼────────┐
│  NixSearch.MCP  │
│  AWS Lambda     │
└────────┬────────┘
         │
┌────────▼────────┐
│ NixSearch.Core  │
│  Library        │
└────────┬────────┘
         │
┌────────▼────────┐
│ search.nixos.org│
│  Elasticsearch  │
└─────────────────┘
```

## Development

### Building

```bash
dotnet build src/NixSearch.MCP/NixSearch.MCP.csproj
```

### Running Tests

```bash
dotnet test tests/NixSearch.MCP.Tests/NixSearch.MCP.Tests.csproj
```

### Lambda Tools

Install AWS Lambda tools for .NET:

```bash
dotnet tool install -g Amazon.Lambda.Tools
```

## Requirements

- .NET 10 or later
- For AWS deployment: AWS account with Lambda and IAM permissions

## See Also

- [NixSearch.Core](../NixSearch.Core/README.md) - The underlying library
- [Model Context Protocol](https://modelcontextprotocol.io/) - Protocol specification
- [Claude Desktop](https://claude.ai/desktop) - AI assistant with MCP support
- [search.nixos.org](https://search.nixos.org) - Official NixOS search

## License

MIT License - see the [LICENSE](../../LICENSE) file for details.
