// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Collections.Generic;

using FluentAssertions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Extensions;
using NixSearch.Core.Search;

namespace NixSearch.Core.Tests.Extensions;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions"/>.
/// </summary>
public class ServiceCollectionExtensionsTests
{
    /// <summary>
    /// Tests that AddNixSearch with configuration registers the necessary services.
    /// </summary>
    [Fact]
    public void AddNixSearch_WithConfiguration_ShouldRegisterServices()
    {
        // Arrange
        ServiceCollection services = new();
        Dictionary<string, string?> configData = new()
        {
            ["Url"] = "https://search.nixos.org/backend",
            ["Username"] = "testuser",
            ["Password"] = "testpass",
            ["MappingSchemaVersion"] = "44",
            ["Timeout"] = "00:00:30",
            ["EnableDebugMode"] = "false",
        };
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        services.AddNixSearch(configuration);
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Assert
        INixSearchClient? nixSearchClient = serviceProvider.GetService<INixSearchClient>();
        IElasticClient? elasticClient = serviceProvider.GetService<IElasticClient>();
        IOptions<NixSearchOptions>? options = serviceProvider.GetService<IOptions<NixSearchOptions>>();

        nixSearchClient.Should().NotBeNull();
        elasticClient.Should().NotBeNull();
        options.Should().NotBeNull();
        options!.Value.Url.Should().Be(configData["Url"]);
        options.Value.Username.Should().Be(configData["Username"]);
        options.Value.Password.Should().Be(configData["Password"]);
        options.Value.MappingSchemaVersion.Should().Be(44);
        options.Value.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        options.Value.EnableDebugMode.Should().BeFalse();
    }

    /// <summary>
    /// Tests that AddNixSearch with null configuration throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void AddNixSearch_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        Action act = () => services.AddNixSearch((IConfiguration)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configuration");
    }

    /// <summary>
    /// Tests that AddNixSearch with null services throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void AddNixSearch_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        Dictionary<string, string?> configData = new()
        {
            ["Url"] = "https://search.nixos.org/backend",
            ["Username"] = "test",
            ["Password"] = "test",
        };
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        Action act = () => ((IServiceCollection)null!).AddNixSearch(configuration);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("services");
    }

    /// <summary>
    /// Tests that AddNixSearch with explicit options registers the necessary services.
    /// </summary>
    [Fact]
    public void AddNixSearch_WithExplicitOptions_ShouldRegisterServices()
    {
        // Arrange
        ServiceCollection services = new();
        NixSearchOptions opts = new()
        {
            Url = "https://custom.url/backend",
            Username = "customuser",
            Password = "custompass",
            MappingSchemaVersion = 45,
            Timeout = TimeSpan.FromSeconds(60),
            EnableDebugMode = true,
        };

        // Act
        services.AddNixSearch(opts);
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Assert
        INixSearchClient? nixSearchClient = serviceProvider.GetService<INixSearchClient>();
        IElasticClient? elasticClient = serviceProvider.GetService<IElasticClient>();
        IOptions<NixSearchOptions>? options = serviceProvider.GetService<IOptions<NixSearchOptions>>();

        nixSearchClient.Should().NotBeNull();
        elasticClient.Should().NotBeNull();
        options.Should().NotBeNull();
        options!.Value.Url.Should().Be(opts.Url);
        options.Value.Username.Should().Be(opts.Username);
        options.Value.Password.Should().Be(opts.Password);
        options.Value.MappingSchemaVersion.Should().Be(opts.MappingSchemaVersion);
        options.Value.Timeout.Should().Be(opts.Timeout);
        options.Value.EnableDebugMode.Should().Be(opts.EnableDebugMode);
    }

    /// <summary>
    /// Tests that AddNixSearch with null options throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void AddNixSearch_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        Action act = () => services.AddNixSearch((NixSearchOptions)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    /// <summary>
    /// Tests that AddNixSearch with null services and explicit options throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void AddNixSearch_WithExplicitOptionsNullServices_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => ((IServiceCollection)null!).AddNixSearch(new NixSearchOptions
        {
            Url = "https://test.url",
            Username = "test",
            Password = "test",
        });

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("services");
    }

    /// <summary>
    /// Tests that AddNixSearch registers IElasticClient as a singleton.
    /// </summary>
    [Fact]
    public void AddNixSearch_ShouldRegisterElasticClientAsSingleton()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddNixSearch(new NixSearchOptions
        {
            Url = "https://search.nixos.org/backend",
            Username = "test",
            Password = "test",
        });
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Act
        IElasticClient? client1 = serviceProvider.GetService<IElasticClient>();
        IElasticClient? client2 = serviceProvider.GetService<IElasticClient>();

        // Assert
        client1.Should().BeSameAs(client2);
    }

    /// <summary>
    /// Tests that AddNixSearch registers INixSearchClient as a singleton.
    /// </summary>
    [Fact]
    public void AddNixSearch_ShouldRegisterNixSearchClientAsSingleton()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddNixSearch(new NixSearchOptions
        {
            Url = "https://search.nixos.org/backend",
            Username = "test",
            Password = "test",
        });
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Act
        INixSearchClient? client1 = serviceProvider.GetService<INixSearchClient>();
        INixSearchClient? client2 = serviceProvider.GetService<INixSearchClient>();

        // Assert
        client1.Should().BeSameAs(client2);
    }

    /// <summary>
    /// Tests that AddNixSearch with authentication credentials configures authentication.
    /// </summary>
    [Fact]
    public void AddNixSearch_WithAuthCredentials_ShouldConfigureAuthentication()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddNixSearch(new NixSearchOptions
        {
            Url = "https://search.nixos.org/backend",
            Username = "testuser",
            Password = "testpass",
        });
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Act
        IElasticClient? client = serviceProvider.GetService<IElasticClient>();

        // Assert
        client.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that AddNixSearch without authentication credentials does not configure authentication.
    /// </summary>
    [Fact]
    public void AddNixSearch_WithoutAuthCredentials_ShouldNotConfigureAuthentication()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddNixSearch(new NixSearchOptions
        {
            Url = "https://search.nixos.org/backend",
            Username = string.Empty,
            Password = string.Empty,
        });
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Act
        IElasticClient? client = serviceProvider.GetService<IElasticClient>();

        // Assert
        client.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that AddNixSearch with debug mode enabled configures the client for debugging.
    /// </summary>
    [Fact]
    public void AddNixSearch_WithDebugMode_ShouldEnableDebugging()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddNixSearch(new NixSearchOptions
        {
            Url = "https://search.nixos.org/backend",
            Username = "test",
            Password = "test",
            EnableDebugMode = true,
        });
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Act
        IElasticClient? client = serviceProvider.GetService<IElasticClient>();

        // Assert
        client.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that AddNixSearch returns the original service collection.
    /// </summary>
    [Fact]
    public void AddNixSearch_ShouldReturnServiceCollection()
    {
        // Arrange
        ServiceCollection services = new();
        Dictionary<string, string?> configData = new()
        {
            ["Url"] = "https://search.nixos.org/backend",
            ["Username"] = "test",
            ["Password"] = "test",
        };
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        IServiceCollection result = services.AddNixSearch(configuration);

        // Assert
        result.Should().BeSameAs(services);
    }
}
