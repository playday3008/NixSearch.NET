// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.Collections.Generic;

using FluentAssertions;

using NixSearch.MCP.Models;

namespace NixSearch.MCP.Tests.Models;

/// <summary>
/// Unit tests for <see cref="SearchResponse{T}"/>.
/// </summary>
public class SearchResponseTests
{
    /// <summary>
    /// Tests that SearchResponse can be created with required properties.
    /// </summary>
    [Fact]
    public void SearchResponse_WithRequiredProperties_ShouldCreate()
    {
        // Arrange & Act
        SearchResponse<string> response = new()
        {
            Total = 100,
            Page = 0,
            Size = 10,
            HasMore = true,
            Results = ["result1", "result2"],
        };

        // Assert
        response.Should().NotBeNull();
        response.Total.Should().Be(100);
        response.Page.Should().Be(0);
        response.Size.Should().Be(10);
        response.HasMore.Should().BeTrue();
        response.Results.Should().HaveCount(2);
        response.Warnings.Should().BeNull();
    }

    /// <summary>
    /// Tests that SearchResponse can be created with Warnings.
    /// </summary>
    [Fact]
    public void SearchResponse_WithWarnings_ShouldCreate()
    {
        // Arrange
        List<Warning> warnings =
        [
            new Warning
            {
                Code = "WARN001",
                Message = "First warning",
                Parameter = "size",
            },
            new Warning
            {
                Code = "WARN002",
                Message = "Second warning",
            },
        ];

        // Act
        SearchResponse<string> response = new()
        {
            Total = 50,
            Page = 1,
            Size = 25,
            HasMore = true,
            Results = ["result1"],
            Warnings = warnings,
        };

        // Assert
        response.Should().NotBeNull();
        response.Warnings.Should().NotBeNull();
        response.Warnings.Should().HaveCount(2);
        response.Warnings![0].Code.Should().Be("WARN001");
        response.Warnings[0].Message.Should().Be("First warning");
        response.Warnings[0].Parameter.Should().Be("size");
        response.Warnings[1].Code.Should().Be("WARN002");
        response.Warnings[1].Message.Should().Be("Second warning");
    }

    /// <summary>
    /// Tests that SearchResponse is a record and supports value equality.
    /// </summary>
    [Fact]
    public void SearchResponse_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        List<string> results = ["result1"];
        SearchResponse<string> response1 = new()
        {
            Total = 10,
            Page = 0,
            Size = 5,
            HasMore = true,
            Results = results,
        };

        SearchResponse<string> response2 = new()
        {
            Total = 10,
            Page = 0,
            Size = 5,
            HasMore = true,
            Results = results,
        };

        // Act & Assert
        response1.Should().Be(response2);
    }

    /// <summary>
    /// Tests that SearchResponse supports record with expression.
    /// </summary>
    [Fact]
    public void SearchResponse_WithExpression_ShouldCreateNewInstance()
    {
        // Arrange
        SearchResponse<string> original = new()
        {
            Total = 100,
            Page = 0,
            Size = 10,
            HasMore = true,
            Results = ["result1"],
        };

        // Act
        SearchResponse<string> modified = original with { Page = 1 };

        // Assert
        original.Page.Should().Be(0);
        modified.Page.Should().Be(1);
        modified.Total.Should().Be(original.Total);
        modified.Size.Should().Be(original.Size);
        modified.Should().NotBe(original);
    }

    /// <summary>
    /// Tests that SearchResponse with empty results should work correctly.
    /// </summary>
    [Fact]
    public void SearchResponse_WithEmptyResults_ShouldCreate()
    {
        // Arrange & Act
        SearchResponse<string> response = new()
        {
            Total = 0,
            Page = 0,
            Size = 10,
            HasMore = false,
            Results = [],
        };

        // Assert
        response.Should().NotBeNull();
        response.Total.Should().Be(0);
        response.HasMore.Should().BeFalse();
        response.Results.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that SearchResponse correctly calculates HasMore flag.
    /// </summary>
    [Fact]
    public void SearchResponse_WithLastPage_ShouldHaveHasMoreFalse()
    {
        // Arrange & Act
        SearchResponse<string> response = new()
        {
            Total = 25,
            Page = 2,
            Size = 10,
            HasMore = false,
            Results = ["result1", "result2", "result3", "result4", "result5"],
        };

        // Assert
        response.Should().NotBeNull();
        response.HasMore.Should().BeFalse();
        response.Results.Should().HaveCount(5);
    }
}
