// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;

using FluentAssertions;

using Microsoft.Extensions.Options;

using Moq;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Search;
using NixSearch.Core.Search.Builders;

namespace NixSearch.Core.Tests.Search;

/// <summary>
/// Tests for <see cref="NixSearchClient"/>.
/// </summary>
public class NixSearchClientTests
{
    private readonly Mock<IElasticClient> mockClient = new();
    private readonly IOptions<NixSearchOptions> options = Options.Create(new NixSearchOptions
    {
        Url = "https://search.nixos.org/backend",
        Username = "test",
        Password = "test",
        MappingSchemaVersion = 44,
        Timeout = TimeSpan.FromSeconds(30),
        EnableDebugMode = false,
    });

    /// <summary>
    /// Constructor with valid arguments should succeed.
    /// </summary>
    [Fact]
    public void Constructor_WithValidArguments_ShouldSucceed()
    {
        // Act
        NixSearchClient client = new(this.mockClient.Object, this.options);

        // Assert
        client.Should().NotBeNull();
    }

    /// <summary>
    /// Constructor with null client should throw ArgumentNullException.
    /// </summary>
    [Fact]
    public void Constructor_WithNullClient_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => _ = new NixSearchClient(null!, this.options);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("client");
    }

    /// <summary>
    /// Constructor with null options should throw ArgumentNullException.
    /// </summary>
    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => _ = new NixSearchClient(this.mockClient.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    /// <summary>
    /// Packages should return a PackageSearchBuilder.
    /// </summary>
    [Fact]
    public void Packages_ShouldReturnPackageSearchBuilder()
    {
        // Arrange
        NixSearchClient client = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = client.Packages();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PackageSearchBuilder>();
    }

    /// <summary>
    /// Options should return an OptionSearchBuilder.
    /// </summary>
    [Fact]
    public void Options_ShouldReturnOptionSearchBuilder()
    {
        // Arrange
        NixSearchClient client = new(this.mockClient.Object, this.options);

        // Act
        OptionSearchBuilderBase result = client.Options();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OptionSearchBuilder>();
    }
}