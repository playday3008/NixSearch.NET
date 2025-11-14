// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.Threading.Tasks;

using FluentAssertions;

using NixSearch.MCP.Models;

namespace NixSearch.MCP.Tests.Integration;

/// <summary>
/// Integration tests for <see cref="Tools.SearchOptionsTool"/>.
/// </summary>
public class SearchOptionsToolIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that SearchOptions with a common option name returns results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchOptions_WithCommonOption_ShouldReturnResults()
    {
        // Act
        SearchResponse<OptionResult> result = await this.SearchOptionsTool.SearchOptions(
            "networking",
            size: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().BeGreaterThan(0);
        result.Results.Should().NotBeEmpty();
        result.Results.Should().Contain(o => o.Name
            .Contains("networking", System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Tests that SearchOptions with pagination works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchOptions_WithPagination_ShouldReturnCorrectPage()
    {
        // Act
        SearchResponse<OptionResult> page0 = await this.SearchOptionsTool.SearchOptions(
            "services",
            page: 0,
            size: 5,
            cancellationToken: TestContext.Current.CancellationToken);
        SearchResponse<OptionResult> page1 = await this.SearchOptionsTool.SearchOptions(
            "services",
            page: 1,
            size: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        page0.Should().NotBeNull();
        page0.Results.Should().HaveCount(5);
        page0.Page.Should().Be(0);

        page1.Should().NotBeNull();
        page1.Results.Should().HaveCount(5);
        page1.Page.Should().Be(1);

        // Results should be different
        page0.Results[0].Name.Should().NotBe(page1.Results[0].Name);
    }

    /// <summary>
    /// Tests that SearchOptions with stable channel works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchOptions_WithStableChannel_ShouldReturnResults()
    {
        // Act
        SearchResponse<OptionResult> result = await this.SearchOptionsTool.SearchOptions(
            "boot",
            channel: "stable",
            size: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that SearchOptions with nonexistent option returns empty.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchOptions_WithNonexistentOption_ShouldReturnEmpty()
    {
        // Act
        SearchResponse<OptionResult> result = await this.SearchOptionsTool.SearchOptions(
            "this-option-definitely-does-not-exist-xyz123",
            size: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().Be(0);
        result.Results.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that SearchOptions returns all expected fields.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchOptions_ShouldReturnAllFields()
    {
        // Act
        SearchResponse<OptionResult> result = await this.SearchOptionsTool.SearchOptions(
            "services.nginx.enable",
            size: 1,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeEmpty();

        OptionResult option = result.Results[0];
        option.Name.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that SearchOptions calculates HasMore correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact(Timeout = 30000)]
    public async Task SearchOptions_ShouldCalculateHasMoreCorrectly()
    {
        // Act - Get a small page from a query that has many results
        SearchResponse<OptionResult> result = await this.SearchOptionsTool.SearchOptions(
            "enable",
            size: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().BeGreaterThan(5);
        result.HasMore.Should().BeTrue();
    }
}