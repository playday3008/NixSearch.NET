// SPDX-License-Identifier: MIT

using System;
using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NixSearch.Core.Configuration;
using NixSearch.Core.Extensions;
using NixSearch.Core.Search;

namespace NixSearch.CLI;

/// <summary>
/// Factory for creating NixSearch clients.
/// </summary>
public static class NixSearchClientFactory
{
    /// <summary>
    /// Creates a NixSearch client from configuration.
    /// </summary>
    /// <returns>The NixSearch client.</returns>
    public static INixSearchClient Create()
    {
        // Build configuration
        IConfigurationBuilder configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("nixsearch.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables("NIXSEARCH_");

        // Check for config file in user's home directory
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (!string.IsNullOrEmpty(homeDir))
        {
            string userConfigPath = Path.Combine(homeDir, ".config", "nixsearch", "config.json");
            if (File.Exists(userConfigPath))
            {
                configBuilder.AddJsonFile(userConfigPath, optional: true, reloadOnChange: false);
            }
        }

        IConfiguration configuration = configBuilder.Build();

        // Setup dependency injection
        ServiceCollection services = new();

        // Check if configuration section exists
        IConfigurationSection nixSearchSection = configuration.GetSection("NixSearch");
        if (!nixSearchSection.Exists())
        {
            // Use default configuration with empty credentials
            services.AddNixSearch(new NixSearchOptions
            {
                Url = "https://search.nixos.org/backend",
                Username = string.Empty,
                Password = string.Empty,
                MappingSchemaVersion = 44,
                Timeout = TimeSpan.FromSeconds(30),
                EnableDebugMode = false,
            });
        }
        else
        {
            services.AddNixSearch(nixSearchSection);
        }

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<INixSearchClient>();
    }
}
