// SPDX-License-Identifier: MIT

using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NixSearch.Core.Configuration;
using NixSearch.Core.Extensions;
using NixSearch.Core.Search;
using NixSearch.MCP.Tools;

namespace NixSearch.MCP.Tests.Integration;

/// <summary>
/// Base class for integration tests that use real backend configuration.
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    private readonly ServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestBase"/> class.
    /// </summary>
    protected IntegrationTestBase()
    {
        // Build configuration from appsettings.json
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        // Setup dependency injection
        ServiceCollection services = new();
        services.AddNixSearch(configuration.GetSection("NixSearch"));

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Register MCP Tools
        services.AddTransient<SearchPackagesTool>();
        services.AddTransient<SearchOptionsTool>();
        services.AddTransient<GetPackageDetailsTool>();
        services.AddTransient<GetOptionDetailsTool>();

        this.serviceProvider = services.BuildServiceProvider();

        // Get the client and tool instances
        this.SearchPackagesTool = this.serviceProvider.GetRequiredService<SearchPackagesTool>();
        this.SearchOptionsTool = this.serviceProvider.GetRequiredService<SearchOptionsTool>();
        this.GetPackageDetailsTool = this.serviceProvider.GetRequiredService<GetPackageDetailsTool>();
        this.GetOptionDetailsTool = this.serviceProvider.GetRequiredService<GetOptionDetailsTool>();
    }

    /// <summary>
    /// Gets the SearchPackagesTool instance.
    /// </summary>
    protected SearchPackagesTool SearchPackagesTool { get; }

    /// <summary>
    /// Gets the SearchOptionsTool instance.
    /// </summary>
    protected SearchOptionsTool SearchOptionsTool { get; }

    /// <summary>
    /// Gets the GetPackageDetailsTool instance.
    /// </summary>
    protected GetPackageDetailsTool GetPackageDetailsTool { get; }

    /// <summary>
    /// Gets the GetOptionDetailsTool instance.
    /// </summary>
    protected GetOptionDetailsTool GetOptionDetailsTool { get; }

    /// <summary>
    /// Disposes the service provider.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the service provider.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.serviceProvider?.Dispose();
        }
    }
}