// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.Options;

using Moq;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Models;
using NixSearch.Core.Search;
using NixSearch.Core.Search.Builders;

namespace NixSearch.Core.Tests.Search.Builders;

/// <summary>
/// Tests for <see cref="PackageSearchBuilder"/>.
/// </summary>
public class PackageSearchBuilderTests
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
    /// Constructor should create a valid builder.
    /// </summary>
    [Fact]
    public void Constructor_ShouldCreateValidBuilder()
    {
        // Arrange

        // Act
        PackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Assert
        builder.Should().NotBeNull();
        builder.Should().BeAssignableTo<PackageSearchBuilderBase>();
    }

    /// <summary>
    /// ExecuteAsync with valid query should call Elasticsearch.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithValidQuery_ShouldCallElasticsearch()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);
        mockResponse.Setup(r => r.Documents).Returns([]);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        PackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        ISearchResponse<NixPackage> result = await builder
            .WithQuery("vim")
            .ForChannel(NixChannel.Unstable)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        this.mockClient.Verify(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// ExecuteAsync with filters should call Elasticsearch.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithFilters_ShouldCallElasticsearch()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);
        mockResponse.Setup(r => r.Documents).Returns([]);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        PackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        ISearchResponse<NixPackage> result = await builder
            .WithQuery("editor")
            .WithPackageSet("vimPlugins")
            .WithLicense("mit")
            .WithMaintainer("alice")
            .WithTeam("vim-team")
            .WithPlatform("x86_64-linux")
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        this.mockClient.Verify(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// ExecuteAsync with filters should call Elasticsearch.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithPagination_ShouldCallElasticsearch()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        PackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        await builder
            .WithQuery("test")
            .Page(100, 25)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        this.mockClient.Verify(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// ExecuteAsync with sort order should call Elasticsearch.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithSortOrder_ShouldCallElasticsearch()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        PackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        await builder
            .WithQuery("test")
            .SortBy(SortOrder.Ascending)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        this.mockClient.Verify(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Fluent API should support method chaining.
    /// </summary>
    [Fact]
    public void FluentAPI_ShouldSupportMethodChaining()
    {
        // Arrange
        PackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder
            .WithQuery("python")
            .ForChannel(NixChannel.FromValue("nixos-24.11"))
            .WithPackageSet("python3Packages")
            .WithLicense("mit", "gpl3")
            .WithMaintainer("alice", "bob")
            .WithTeam("python-team")
            .WithPlatform("x86_64-linux", "aarch64-darwin")
            .Page(0, 50)
            .SortBy(SortOrder.Descending);

        // Assert
        result.Should().BeSameAs(builder);
    }

    /// <summary>
    /// ExecuteAsync from NixSearchClient should work.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_FromNixSearchClient_ShouldWork()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);
        mockResponse.Setup(r => r.Documents).Returns([]);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        NixSearchClient nixClient = new(this.mockClient.Object, this.options);

        // Act
        ISearchResponse<NixPackage> result = await nixClient.Packages()
            .WithQuery("vim")
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }
}
