// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Text.Json;

using FluentAssertions;

using Moq;

using Nest;

using NixSearch.CLI.Formatters;
using NixSearch.CLI.Tests.TestHelpers;
using NixSearch.Core.Models;

namespace NixSearch.CLI.Tests.Formatters;

/// <summary>
/// Tests for <see cref="JsonOutputFormatter{T}"/>.
/// </summary>
public class JsonOutputFormatterTests
{
    /// <summary>
    /// Format should produce valid JSON output.
    /// </summary>
    [Fact]
    public void Format_WithPackages_ShouldProduceValidJson()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreatePackage("vim", "vim", "9.0", "Vi IMproved text editor")
        ]);

        JsonOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().NotBeNullOrEmpty();
        Action act = () => JsonDocument.Parse(result);
        act.Should().NotThrow();
    }

    /// <summary>
    /// Format should include total and results fields.
    /// </summary>
    [Fact]
    public void Format_ShouldIncludeTotalAndResults()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(2);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreatePackage("vim", "vim", "9.0"),
            TestDataFactory.CreatePackage("neovim", "neovim", "0.9.0")
        ]);

        JsonOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);
        using JsonDocument doc = JsonDocument.Parse(result);

        // Assert
        doc.RootElement.GetProperty("total").GetInt64().Should().Be(2);
        doc.RootElement.GetProperty("results").GetArrayLength().Should().Be(2);
    }

    /// <summary>
    /// Format should use camelCase naming.
    /// </summary>
    [Fact]
    public void Format_ShouldUseCamelCase()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(0);
        mockResponse.Setup(r => r.Documents).Returns([]);

        JsonOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain("\"total\"");
        result.Should().Contain("\"results\"");
    }

    /// <summary>
    /// Format should handle empty results.
    /// </summary>
    [Fact]
    public void Format_WithEmptyResults_ShouldProduceValidJson()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(0);
        mockResponse.Setup(r => r.Documents).Returns([]);

        JsonOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);
        using JsonDocument doc = JsonDocument.Parse(result);

        // Assert
        doc.RootElement.GetProperty("total").GetInt64().Should().Be(0);
        doc.RootElement.GetProperty("results").GetArrayLength().Should().Be(0);
    }

    /// <summary>
    /// Format should work with options.
    /// </summary>
    [Fact]
    public void Format_WithOptions_ShouldProduceValidJson()
    {
        // Arrange
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreateOption("services.nginx.enable", "Whether to enable nginx.")
        ]);

        JsonOutputFormatter<NixOption> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        Action act = () => JsonDocument.Parse(result);
        act.Should().NotThrow();
    }

    /// <summary>
    /// Format should be indented for readability.
    /// </summary>
    [Fact]
    public void Format_ShouldBeIndented()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(0);
        mockResponse.Setup(r => r.Documents).Returns([]);

        JsonOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain(Environment.NewLine);
        result.Should().Contain("  "); // Check for indentation
    }
}