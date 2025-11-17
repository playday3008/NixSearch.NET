// SPDX-License-Identifier: MIT

using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Search;

namespace NixSearch.Core.Extensions;

/// <summary>
/// Extension methods for configuring NixSearch services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds NixSearch services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section for NixSearch options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if services or configuration is null.</exception>
    public static IServiceCollection AddNixSearch(
        this IServiceCollection services,
        in IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Bind configuration
        services.Configure<NixSearchOptions>(configuration);

        // Register Elasticsearch client
        services.AddSingleton<IElasticClient>(sp =>
        {
            NixSearchOptions options = sp.GetRequiredService<IOptions<NixSearchOptions>>().Value;
            return CreateElasticClient(options);
        });

        // Register NixSearch client
        services.AddSingleton<INixSearchClient, NixSearchClient>();

        return services;
    }

    /// <summary>
    /// Adds NixSearch services to the service collection with configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if services or configureOptions is null.</exception>
    public static IServiceCollection AddNixSearch(
        this IServiceCollection services,
        in Action<NixSearchOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        // Configure options
        services.Configure(configureOptions);

        // Register Elasticsearch client
        services.AddSingleton<IElasticClient>(sp =>
        {
            NixSearchOptions options = sp.GetRequiredService<IOptions<NixSearchOptions>>().Value;
            return CreateElasticClient(options);
        });

        // Register NixSearch client
        services.AddSingleton<INixSearchClient, NixSearchClient>();

        return services;
    }

    private static ElasticClient CreateElasticClient(in NixSearchOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        ConnectionSettings settings = new ConnectionSettings(new Uri(options.Url))
            .RequestTimeout(options.Timeout)
            .BasicAuthentication(options.Username, options.Password)
            .MaximumRetries(options.MaxRetries)
            .MaxRetryTimeout(options.MaxRetryTimeout)
            .EnableHttpCompression();

        if (options.EnableDebugMode)
        {
            settings.EnableDebugMode();
            settings.DisableDirectStreaming(true);
        }

        return new ElasticClient(settings);
    }
}