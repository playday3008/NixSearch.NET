// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Collections.Generic;
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
/// Tests for <see cref="PackageSearchBuilderBase"/>.
/// </summary>
public class PackageSearchBuilderBaseTests
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
    /// WithPackageSet with valid sets should add to attributes.
    /// </summary>
    [Fact]
    public void WithPackageSet_WithValidSets_ShouldAddToAttributes()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder.WithPackageSet("python3Packages", "haskellPackages");

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetAttributes().Should().ContainInOrder("python3Packages", "haskellPackages");
    }

    /// <summary>
    /// WithPackageSet with null sets should not throw.
    /// </summary>
    [Fact]
    public void WithPackageSet_WithNullSets_ShouldNotThrow()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder.WithPackageSet(null!);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetAttributes().Should().BeEmpty();
    }

    /// <summary>
    /// WithPackageSet called multiple times should accumulate values.
    /// </summary>
    [Fact]
    public void WithPackageSetCalledMultipleTimes_ShouldAccumulateValues()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        builder.WithPackageSet("python3Packages")
               .WithPackageSet("haskellPackages");

        // Assert
        builder.GetAttributes().Should().ContainInOrder("python3Packages", "haskellPackages");
    }

    /// <summary>
    /// WithLicense with valid licenses should add to licenses.
    /// </summary>
    [Fact]
    public void WithLicense_WithValidLicenses_ShouldAddToLicenses()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder.WithLicense("mit", "gpl3");

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetLicenses().Should().ContainInOrder("mit", "gpl3");
    }

    /// <summary>
    /// WithLicense with null licenses should not throw.
    /// </summary>
    [Fact]
    public void WithLicense_WithNullLicenses_ShouldNotThrow()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder.WithLicense(null!);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetLicenses().Should().BeEmpty();
    }

    /// <summary>
    /// WithMaintainer with valid maintainers should add to maintainers.
    /// </summary>
    [Fact]
    public void WithMaintainer_WithValidMaintainers_ShouldAddToMaintainers()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder.WithMaintainer("alice", "bob");

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetMaintainers().Should().ContainInOrder("alice", "bob");
    }

    /// <summary>
    /// WithMaintainer with null maintainers should not throw.
    /// </summary>
    [Fact]
    public void WithMaintainer_WithNullMaintainers_ShouldNotThrow()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder.WithMaintainer(null!);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetMaintainers().Should().BeEmpty();
    }

    /// <summary>
    /// WithTeam with valid teams should add to teams.
    /// </summary>
    [Fact]
    public void WithTeam_WithValidTeams_ShouldAddToTeams()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder.WithTeam("team1", "team2");

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetTeams().Should().ContainInOrder("team1", "team2");
    }

    /// <summary>
    /// WithTeam with null teams should not throw.
    /// </summary>
    [Fact]
    public void WithTeam_WithNullTeams_ShouldNotThrow()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder.WithTeam(null!);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetTeams().Should().BeEmpty();
    }

    /// <summary>
    /// WithPlatform with valid platforms should add to platforms.
    /// </summary>
    [Fact]
    public void WithPlatform_WithValidPlatforms_ShouldAddToPlatforms()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder.WithPlatform("x86_64-linux", "aarch64-darwin");

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetPlatforms().Should().ContainInOrder("x86_64-linux", "aarch64-darwin");
    }

    /// <summary>
    /// WithPlatform with null platforms should not throw.
    /// </summary>
    [Fact]
    public void WithPlatform_WithNullPlatforms_ShouldNotThrow()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder.WithPlatform(null!);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetPlatforms().Should().BeEmpty();
    }

    /// <summary>
    /// Fluent chaining should work for all methods.
    /// </summary>
    [Fact]
    public void FluentChaining_ShouldWorkForAllMethods()
    {
        // Arrange
        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

        // Act
        PackageSearchBuilderBase result = builder
            .WithQuery("vim")
            .ForChannel(NixChannel.FromValue("nixos-24.11"))
            .WithPackageSet("vimPlugins")
            .WithLicense("mit")
            .WithMaintainer("alice")
            .WithTeam("vim-team")
            .WithPlatform("x86_64-linux")
            .Page(0, 20)
            .SortBy(SortOrder.Ascending);

        // Assert
        result.Should().BeSameAs(builder);
        builder.GetQuery().Should().Be("vim");
        builder.GetAttributes().Should().Contain("vimPlugins");
        builder.GetLicenses().Should().Contain("mit");
        builder.GetMaintainers().Should().Contain("alice");
        builder.GetTeams().Should().Contain("vim-team");
        builder.GetPlatforms().Should().Contain("x86_64-linux");
        builder.GetFrom().Should().Be(0);
        builder.GetSize().Should().Be(20);
        builder.GetOrder().Should().Be(SortOrder.Ascending);
    }

    /// <summary>
    /// ExecuteAsync should call Elasticsearch client.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
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

        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

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

        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);

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
    /// ExecuteAsync with filters should call Elasticsearch client.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ExecuteAsync_WithFilters_ShouldCallElasticsearchClient()
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

        TestPackageSearchBuilder builder = new(this.mockClient.Object, this.options);
        builder.WithQuery("python")
               .WithPackageSet("python3Packages")
               .WithLicense("mit")
               .WithMaintainer("alice")
               .WithTeam("python-team")
               .WithPlatform("x86_64-linux");

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
    /// Test implementation of PackageSearchBuilderBase for testing purposes.
    /// </summary>
    private sealed class TestPackageSearchBuilder(
        IElasticClient client,
        IOptions<NixSearchOptions> options)
        : PackageSearchBuilderBase(
            client,
            options)
    {
        public string GetQuery() => this.Query;

        public int GetFrom() => this.From;

        public int GetSize() => this.Size;

        public SortOrder? GetOrder() => this.Order;

        public List<string> GetAttributes() => this.Attributes;

        public List<string> GetLicenses() => this.Licenses;

        public List<string> GetMaintainers() => this.Maintainers;

        public List<string> GetTeams() => this.Teams;

        public List<string> GetPlatforms() => this.Platforms;

        protected override string[] GetMatchFields() => [
            "package_attr_name",
            "package_pname",
            "package_description"
        ];

        protected override SortDescriptor<NixPackage> GetSortDescriptor()
        {
            SortDescriptor<NixPackage> sortDescriptor = new();

            return sortDescriptor;
        }

        protected override AggregationContainerDescriptor<NixPackage> GetAggregations()
        {
            return new AggregationContainerDescriptor<NixPackage>()
                .Terms("package_attr_set", t => t
                    .Field("package_attr_set")
                    .Size(20))
                .Terms("package_license_set", t => t
                    .Field("package_license_set")
                    .Size(20));
        }

        protected override SearchDescriptor<NixPackage> GetSearchDescriptor()
        {
            return new SearchDescriptor<NixPackage>()
                .Index(this.GetIndexName())
                .From(this.From)
                .Size(this.Size)
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .MultiMatch(mm => mm
                                .Query(this.Query)
                                .Fields(this.GetMatchFields())))))
                .Aggregations(aggs => this.GetAggregations())
                .Sort(s => this.GetSortDescriptor());
        }
    }
}
