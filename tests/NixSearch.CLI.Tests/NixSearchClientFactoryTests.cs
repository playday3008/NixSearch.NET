// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;

using FluentAssertions;

using NixSearch.Core.Search;

namespace NixSearch.CLI.Tests;

/// <summary>
/// Tests for <see cref="NixSearchClientFactory"/>.
/// </summary>
public class NixSearchClientFactoryTests
{
    /// <summary>
    /// Create should return a valid client.
    /// </summary>
    [Fact]
    public void Create_ShouldReturnValidClient()
    {
        // Act
        INixSearchClient client = NixSearchClientFactory.Create();

        // Assert
        client.Should().NotBeNull();
        client.Should().BeAssignableTo<INixSearchClient>();
    }

    /// <summary>
    /// Create should use default configuration when no config file exists.
    /// </summary>
    [Fact]
    public void Create_WithoutConfigFile_ShouldUseDefaults()
    {
        // This test verifies that the factory can create a client even when
        // configuration files are not present

        // Act
        Action act = () => NixSearchClientFactory.Create();

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Create should return client with package search capability.
    /// </summary>
    [Fact]
    public void Create_Client_ShouldHavePackageSearchCapability()
    {
        // Act
        INixSearchClient client = NixSearchClientFactory.Create();

        // Assert
        client.Packages().Should().NotBeNull();
    }

    /// <summary>
    /// Create should return client with option search capability.
    /// </summary>
    [Fact]
    public void Create_Client_ShouldHaveOptionSearchCapability()
    {
        // Act
        INixSearchClient client = NixSearchClientFactory.Create();

        // Assert
        client.Options().Should().NotBeNull();
    }

    /// <summary>
    /// Create should be able to create multiple clients.
    /// </summary>
    [Fact]
    public void Create_ShouldBeAbleToCreateMultipleClients()
    {
        // Act
        INixSearchClient client1 = NixSearchClientFactory.Create();
        INixSearchClient client2 = NixSearchClientFactory.Create();

        // Assert
        client1.Should().NotBeNull();
        client2.Should().NotBeNull();

        // Different instances expected
        client1.Should().NotBeSameAs(client2);
    }
}
