// SPDX-License-Identifier: MIT

using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NixSearch.Core.Extensions;
using NixSearch.Core.Search;

namespace NixSearch.Core.Tests.Integration;

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

        this.serviceProvider = services.BuildServiceProvider();

        // Get the client instance
        this.Client = this.serviceProvider.GetRequiredService<INixSearchClient>();
    }

    /// <summary>
    /// Gets the NixSearch client instance.
    /// </summary>
    protected INixSearchClient Client { get; }

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
            this.serviceProvider.Dispose();
        }
    }
}
