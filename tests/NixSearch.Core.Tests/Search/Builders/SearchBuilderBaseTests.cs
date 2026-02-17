// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Threading;
using System.Threading.Tasks;

using Elasticsearch.Net;

using FluentAssertions;

using Microsoft.Extensions.Options;

using Moq;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Exceptions;
using NixSearch.Core.Models;
using NixSearch.Core.Search;
using NixSearch.Core.Search.Builders;

namespace NixSearch.Core.Tests.Search.Builders;

/// <summary>
/// Tests for <see cref="SearchBuilderBase{TSource, TBuilder}"/>.
/// </summary>
public class SearchBuilderBaseTests
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
        // Arrange

        // Act
        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Assert
        builder.Should().NotBeNull();
    }

    /// <summary>
    /// Constructor with null client should throw <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void Constructor_WithNullClient_ShouldThrowArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _ = new TestSearchBuilder(null!, this.options);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("client");
    }

    /// <summary>
    /// WithQuery with valid query should set query.
    /// </summary>
    [Fact]
    public void WithQuery_WithValidQuery_ShouldSetQuery()
    {
        // Arrange
        TestSearchBuilder builder = new(this.mockClient.Object, this.options);
        const string query = "python";

        // Act
        TestSearchBuilder result = builder.WithQuery(query);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetQuery().Should().Be(query);
    }

    /// <summary>
    /// Page with valid parameters should set From and Size.
    /// </summary>
    [Fact]
    public void Page_WithValidParameters_ShouldSetFromAndSize()
    {
        // Arrange
        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        TestSearchBuilder result = builder.Page(10, 20);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetFrom().Should().Be(10);
        builder.GetSize().Should().Be(20);
    }

    /// <summary>
    /// Page with negative from should throw <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void Page_WithNegativeFrom_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        Action act = () => builder.Page(-1, 10);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("from");
    }

    /// <summary>
    /// Page with zero size should throw <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void Page_WithZeroSize_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        Action act = () => builder.Page(0, 0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("size");
    }

    /// <summary>
    /// Page with negative size should throw <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void Page_WithNegativeSize_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        Action act = () => builder.Page(0, -1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("size");
    }

    /// <summary>
    /// SortBy with ascending should set order.
    /// </summary>
    [Fact]
    public void SortBy_WithAscending_ShouldSetOrder()
    {
        // Arrange
        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        TestSearchBuilder result = builder.SortBy(SortOrder.Ascending);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetOrder().Should().Be(SortOrder.Ascending);
    }

    /// <summary>
    /// SortBy with descending should set order.
    /// </summary>
    [Fact]
    public void SortBy_WithDescending_ShouldSetOrder()
    {
        // Arrange
        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        TestSearchBuilder result = builder.SortBy(SortOrder.Descending);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetOrder().Should().Be(SortOrder.Descending);
    }

    /// <summary>
    /// SortBy with null should set order to null.
    /// </summary>
    [Fact]
    public void SortBy_WithNull_ShouldSetOrderToNull()
    {
        // Arrange
        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        TestSearchBuilder result = builder.SortBy(null);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetOrder().Should().BeNull();
    }

    /// <summary>
    /// ExecuteAsync should call Elasticsearch client.
    /// </summary>
    /// <returns>>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_ShouldCallElasticsearchClient()
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

        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        ISearchResponse<NixPackage> result = await builder.ExecuteAsync(TestContext.Current.CancellationToken);

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
    /// ExecuteAsync with cancellation token should pass token to client.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_ShouldPassTokenToClient()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);

        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    cancellationToken))
            .ReturnsAsync(mockResponse.Object);

        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        ISearchResponse<NixPackage> result = await builder.ExecuteAsync(cancellationToken);

        // Assert
        result.Should().NotBeNull();
        this.mockClient.Verify(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    cancellationToken),
            Times.Once);
    }

    /// <summary>
    /// ExecuteAsync with query and pagination should call Elasticsearch client.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithQueryAndPagination_ShouldCallElasticsearchClient()
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

        TestSearchBuilder builder = new(this.mockClient.Object, this.options);
        builder.WithQuery("python")
               .ForChannel(NixChannel.FromValue("nixos-24.11"))
               .Page(10, 20)
               .SortBy(SortOrder.Ascending);

        // Act
        ISearchResponse<NixPackage> result = await builder.ExecuteAsync(TestContext.Current.CancellationToken);

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
    /// Execute should call Elasticsearch client.
    /// </summary>
    [Fact]
    public void Execute_ShouldCallElasticsearchClient()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);

        this.mockClient.Setup(
            c => c
                .Search<NixPackage>(It.IsAny<ISearchRequest>()))
            .Returns(mockResponse.Object);

        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        ISearchResponse<NixPackage> result = builder.Execute();

        // Assert
        result.Should().NotBeNull();
        this.mockClient.Verify(
            c => c
                .Search<NixPackage>(It.IsAny<ISearchRequest>()),
            Times.Once);
    }

    /// <summary>
    /// Execute with query and pagination should call Elasticsearch client.
    /// </summary>
    [Fact]
    public void Execute_WithQueryAndPagination_ShouldCallElasticsearchClient()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(true);

        this.mockClient.Setup(
            c => c
                .Search<NixPackage>(It.IsAny<ISearchRequest>()))
            .Returns(mockResponse.Object);

        TestSearchBuilder builder = new(this.mockClient.Object, this.options);
        builder.WithQuery("python")
               .ForChannel(NixChannel.FromValue("nixos-24.11"))
               .Page(10, 20)
               .SortBy(SortOrder.Ascending);

        // Act
        ISearchResponse<NixPackage> result = builder.Execute();

        // Assert
        result.Should().NotBeNull();
        this.mockClient.Verify(
            c => c
                .Search<NixPackage>(It.IsAny<ISearchRequest>()),
            Times.Once);
    }

    /// <summary>
    /// Execute with invalid response should throw <see cref="NixSearchException"/> with original exception.
    /// </summary>
    [Fact]
    public void Execute_WithInvalidResponseAndOriginalException_ShouldThrowNixSearchException()
    {
        // Arrange
        InvalidOperationException originalException = new("Connection failed");
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(false);
        mockResponse.Setup(r => r.OriginalException).Returns(originalException);

        this.mockClient.Setup(
            c => c
                .Search<NixPackage>(It.IsAny<ISearchRequest>()))
            .Returns(mockResponse.Object);

        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        Action act = () => builder.Execute();

        // Assert
        act.Should().Throw<NixSearchException>()
            .WithMessage("Search request failed: Connection failed")
            .WithInnerException<InvalidOperationException>()
            .Which.Should().BeSameAs(originalException);
    }

    /// <summary>
    /// Execute with invalid response and no error details should throw <see cref="NixSearchException"/> with unknown error.
    /// </summary>
    [Fact]
    public void Execute_WithInvalidResponseAndNoErrorDetails_ShouldThrowNixSearchExceptionWithUnknownError()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(false);
        mockResponse.Setup(r => r.OriginalException).Returns((Exception)null!);
        mockResponse.Setup(r => r.ServerError).Returns((ServerError)null!);

        this.mockClient.Setup(
            c => c
                .Search<NixPackage>(It.IsAny<ISearchRequest>()))
            .Returns(mockResponse.Object);

        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        Action act = () => builder.Execute();

        // Assert
        act.Should().Throw<NixSearchException>()
            .WithMessage("Search request failed: Unknown error");
    }

    /// <summary>
    /// ExecuteAsync with invalid response should throw <see cref="NixSearchException"/> with original exception.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithInvalidResponseAndOriginalException_ShouldThrowNixSearchException()
    {
        // Arrange
        InvalidOperationException originalException = new("Connection failed");
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(false);
        mockResponse.Setup(r => r.OriginalException).Returns(originalException);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        Func<Task> act = async () => await builder.ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<NixSearchException>()
            .WithMessage("Search request failed: Connection failed")
            .Where(e => e.InnerException == originalException);
    }

    /// <summary>
    /// ExecuteAsync with invalid response and no error details should throw <see cref="NixSearchException"/> with unknown error.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithInvalidResponseAndNoErrorDetails_ShouldThrowNixSearchExceptionWithUnknownError()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.IsValid).Returns(false);
        mockResponse.Setup(r => r.OriginalException).Returns((Exception)null!);
        mockResponse.Setup(r => r.ServerError).Returns((ServerError)null!);

        this.mockClient.Setup(
            c => c
                .SearchAsync<NixPackage>(
                    It.IsAny<ISearchRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        TestSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        Func<Task> act = async () => await builder.ExecuteAsync(TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<NixSearchException>()
            .WithMessage("Search request failed: Unknown error");
    }

    /// <summary>
    /// Test implementation of SearchBuilderBase for testing purposes.
    /// </summary>
    private sealed class TestSearchBuilder(
        IElasticClient client,
        IOptions<NixSearchOptions> options)
        : SearchBuilderBase<NixPackage, TestSearchBuilder>(
            client,
            options)
    {
        public string GetQuery() => this.Query;

        public int GetFrom() => this.From;

        public int GetSize() => this.Size;

        public SortOrder? GetOrder() => this.Order;

        protected override string[] GetMatchFields() => [
            "package_attr_name",
            "package_pname"
        ];

        protected override SortDescriptor<NixPackage> GetSortDescriptor()
        {
            SortDescriptor<NixPackage> sortDescriptor = new();

            return sortDescriptor;
        }

        protected override SearchDescriptor<NixPackage> GetSearchDescriptor()
        {
            return new SearchDescriptor<NixPackage>()
                .Index(this.GetIndexName())
                .From(this.From)
                .Size(this.Size)
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(this.Query)
                        .Fields(this.GetMatchFields())))
                .Sort(_ => this.GetSortDescriptor());
        }

        private new string GetIndexName() => base.GetIndexName();
    }
}
