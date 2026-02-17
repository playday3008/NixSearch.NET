// SPDX-License-Identifier: MIT

using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NixSearch.Core.Extensions;
using NixSearch.MCP.Tools;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add AWS Lambda support
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Load configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables("NIXSEARCH_");

// Register NixSearch.Core services
builder.Services.AddNixSearch(builder.Configuration.GetSection("NixSearch"));

// Register MCP server with tools
builder.Services.AddMcpServer()
    .WithHttpTransport(options =>
    {
        // Enable stateless mode by default for a read-only search API
        // This eliminates session timeout issues since each request is independent
        options.Stateless = builder.Configuration.GetValue<bool>("MCP:Stateless", defaultValue: true);

        // If not stateless, configure a longer idle timeout (default 24 hours)
        if (!options.Stateless)
        {
            TimeSpan idleTimeout = builder.Configuration.GetValue<TimeSpan>(
                "MCP:IdleTimeout",
                defaultValue: TimeSpan.FromHours(24));
            options.IdleTimeout = idleTimeout;
        }
    })
    .WithTools<SearchPackagesTool>()
    .WithTools<SearchOptionsTool>()
    .WithTools<GetPackageDetailsTool>()
    .WithTools<GetOptionDetailsTool>();

string[] tools = [
    "search_packages",
    "search_options",
    "get_package_details",
    "get_option_details",
];

// CORS is required by the MCP protocol specification for browser-based transports (SSE, streamable HTTP).
// Permissive CORS is intentional: this is a read-only public search API deployed behind Lambda.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

WebApplication app = builder.Build();

// Use CORS
app.UseCors();

// Map MCP endpoint (SDK handles /mcp by default)
app.MapMcp();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "nixsearch-mcp" }));

// Info endpoint
app.MapGet("/info", () => Results.Ok(new
{
    name = "NixSearch MCP Server",
    version = "1.0.0",
    description = "Model Context Protocol server for NixOS package and option search",
    endpoints = new
    {
        mcp = "/",
        health = "/health",
        info = "/info",
    },
    tools = tools,
}));

ILogger<Program> startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogStartingMcpServer();
startupLogger.LogAvailableTools(string.Join(", ", tools));

bool isStateless = builder.Configuration.GetValue<bool>("MCP:Stateless", defaultValue: true);
if (isStateless)
{
    startupLogger.LogStatelessMode();
}
else
{
    TimeSpan idleTimeout = builder.Configuration.GetValue<TimeSpan>(
        "MCP:IdleTimeout",
        defaultValue: TimeSpan.FromHours(24));
    startupLogger.LogStatefulMode(idleTimeout);
}

app.Run();

/// <summary>
/// Program logger extensions.
/// </summary>
internal static partial class ProgramLoggerExtension
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "NixSearch MCP Server starting...")]
    public static partial void LogStartingMcpServer(this ILogger logger);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Available tools: {Tools}")]
    public static partial void LogAvailableTools(this ILogger logger, string tools);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "MCP Server running in STATELESS mode (no session tracking)")]
    public static partial void LogStatelessMode(this ILogger logger);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Information,
        Message = "MCP Server running in STATEFUL mode (idle timeout: {IdleTimeout})")]
    public static partial void LogStatefulMode(this ILogger logger, TimeSpan idleTimeout);
}