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
/// Tests for <see cref="OptionSearchBuilder"/>.
/// </summary>
public class OptionSearchBuilderTests
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
        OptionSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Assert
        builder.Should().NotBeNull();
        builder.Should().BeAssignableTo<OptionSearchBuilder>();
    }

    /// <summary>
    /// ExecuteAsync with valid query should call Elasticsearch.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithValidQuery_ShouldCallElasticsearch()
    {
        // Arrange
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);
        mockResponse.Setup(r => r.Documents).Returns([]);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixOption>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        OptionSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        ISearchResponse<NixOption> result = await builder
            .WithQuery("boot.loader")
            .ForChannel(NixChannel.Unstable)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        this.mockClient.Verify(
            c => c
                .SearchAsync<NixOption>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// ExecuteAsync with pagination should call Elasticsearch.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithPagination_ShouldCallElasticsearch()
    {
        // Arrange
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixOption>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        OptionSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        await builder
            .WithQuery("services")
            .Page(50, 30)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        this.mockClient.Verify(
            c => c
                .SearchAsync<NixOption>(
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
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixOption>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        OptionSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        await builder
            .WithQuery("networking")
            .SortBy(Nest.SortOrder.Ascending)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        this.mockClient.Verify(
            c => c
                .SearchAsync<NixOption>(
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
        OptionSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        OptionSearchBuilder result = builder
            .WithQuery("boot")
            .ForChannel(NixChannel.FromValue("nixos-24.11"))
            .Page(0, 100)
            .SortBy(Nest.SortOrder.Descending);

        // Assert
        result.Should().BeSameAs(builder);
    }

    /// <summary>
    /// ExecuteAsync from NixSearchClient should work.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsyncFromNixSearchClient_ShouldWork()
    {
        // Arrange
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);
        mockResponse.Setup(r => r.Documents).Returns([]);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixOption>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        NixSearchClient nixClient = new(this.mockClient.Object, this.options);

        // Act
        ISearchResponse<NixOption> result = await nixClient.Options()
            .WithQuery("services.nginx")
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// ExecuteAsync with empty query should still work.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithEmptyQuery_ShouldStillWork()
    {
        // Arrange
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);
        mockResponse.Setup(r => r.Documents).Returns([]);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixOption>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        OptionSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        ISearchResponse<NixOption> result = await builder
            .WithQuery(string.Empty)
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        this.mockClient.Verify(
            c => c
                .SearchAsync<NixOption>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// ExecuteAsync with different channels should call Elasticsearch.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithDifferentChannels_ShouldCallElasticsearch()
    {
        // Arrange
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixOption>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        OptionSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        await builder
            .WithQuery("test")
            .ForChannel(NixChannel.FromValue("nixos-24.11"))
            .ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        this.mockClient.Verify(
            c => c
                .SearchAsync<NixOption>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
