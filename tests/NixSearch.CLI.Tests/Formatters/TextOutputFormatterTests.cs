// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;

using FluentAssertions;

using Moq;

using Nest;

using NixSearch.CLI.Formatters;
using NixSearch.CLI.Tests.TestHelpers;
using NixSearch.Core.Models;

namespace NixSearch.CLI.Tests.Formatters;

/// <summary>
/// Tests for <see cref="TextOutputFormatter{T}"/>.
/// </summary>
public class TextOutputFormatterTests
{
    /// <summary>
    /// Format should display package information correctly.
    /// </summary>
    [Fact]
    public void Format_WithPackages_ShouldDisplayCorrectly()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreatePackage("vim", "vim", "9.0", "Vi IMproved text editor")
        ]);

        TextOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain("Found 1 results");
        result.Should().Contain("Package: vim");
        result.Should().Contain("Name: vim");
        result.Should().Contain("Version: 9.0");
        result.Should().Contain("Description: Vi IMproved text editor");
    }

    /// <summary>
    /// Format should display detailed package information.
    /// </summary>
    [Fact]
    public void Format_WithPackagesDetailed_ShouldDisplayAdditionalInfo()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreateDetailedPackage()
        ]);

        TextOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: true);

        // Assert
        result.Should().Contain("Platforms: x86_64-linux, aarch64-darwin");
        result.Should().Contain("Programs: vim, vi");
        result.Should().Contain("Main Program: vim");
        result.Should().Contain("License: Vim");
        result.Should().Contain("Homepage: https://www.vim.org/");
        result.Should().Contain("Position: pkgs/vim/default.nix:10");
        result.Should().Contain("Attribute Set: packages");
        result.Should().Contain("System: x86_64-linux");
    }

    /// <summary>
    /// Format should display option information correctly.
    /// </summary>
    [Fact]
    public void Format_WithOptions_ShouldDisplayCorrectly()
    {
        // Arrange
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreateOption("services.nginx.enable", "Whether to enable nginx.")
        ]);

        TextOutputFormatter<NixOption> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain("Found 1 results");
        result.Should().Contain("Option: services.nginx.enable");
        result.Should().Contain("Description: Whether to enable nginx.");
    }

    /// <summary>
    /// Format should display detailed option information.
    /// </summary>
    [Fact]
    public void Format_WithOptionsDetailed_ShouldDisplayAdditionalInfo()
    {
        // Arrange
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreateDetailedOption()
        ]);

        TextOutputFormatter<NixOption> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: true);

        // Assert
        result.Should().Contain("Type: boolean");
        result.Should().Contain("Default: false");
        result.Should().Contain("Example: true");
        result.Should().Contain("Source: nixos/modules/services/web-servers/nginx/default.nix");
    }

    /// <summary>
    /// Format should handle empty results.
    /// </summary>
    [Fact]
    public void Format_WithEmptyResults_ShouldDisplayZeroResults()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(0);
        mockResponse.Setup(r => r.Documents).Returns([]);

        TextOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain("Found 0 results");
    }

    /// <summary>
    /// Format should handle packages with null fields.
    /// </summary>
    [Fact]
    public void Format_WithNullFields_ShouldNotThrow()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreatePackage()
        ]);

        TextOutputFormatter<NixPackage> formatter = new();

        // Act
        Action act = () => formatter.Format(mockResponse.Object, detailed: true);

        // Assert
        act.Should().NotThrow();
    }
}
