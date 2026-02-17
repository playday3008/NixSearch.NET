// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Xml;

using FluentAssertions;

using Moq;

using Nest;

using NixSearch.CLI.Formatters;
using NixSearch.CLI.Tests.TestHelpers;
using NixSearch.Core.Models;

namespace NixSearch.CLI.Tests.Formatters;

/// <summary>
/// Tests for <see cref="XmlOutputFormatter{T}"/>.
/// </summary>
public class XmlOutputFormatterTests
{
    /// <summary>
    /// Format should produce valid XML output.
    /// </summary>
    [Fact]
    public void Format_WithPackages_ShouldProduceValidXml()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreatePackage("vim", "vim", "9.0", "Vi IMproved text editor")
        ]);

        XmlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().NotBeNullOrEmpty();
        Action act = () =>
        {
            XmlDocument doc = new();
            doc.LoadXml(result);
        };
        act.Should().NotThrow();
    }

    /// <summary>
    /// Format should include XML declaration.
    /// </summary>
    [Fact]
    public void Format_ShouldIncludeXmlDeclaration()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(0);
        mockResponse.Setup(r => r.Documents).Returns([]);

        XmlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().StartWith("<?xml");
    }

    /// <summary>
    /// Format should include total and results elements.
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

        XmlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);
        XmlDocument doc = new();
        doc.LoadXml(result);

        // Assert
        XmlNode? totalNode = doc.SelectSingleNode("/SearchResponse/Total");
        totalNode.Should().NotBeNull();
        totalNode.InnerText.Should().Be("2");

        XmlNodeList? resultNodes = doc.SelectNodes("/SearchResponse/Results/Item");
        resultNodes.Should().NotBeNull();
        resultNodes.Count.Should().Be(2);
    }

    /// <summary>
    /// Format should use proper root element name.
    /// </summary>
    [Fact]
    public void Format_ShouldUseSearchResponseRootElement()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(0);
        mockResponse.Setup(r => r.Documents).Returns([]);

        XmlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);
        XmlDocument doc = new();
        doc.LoadXml(result);

        // Assert
        doc.DocumentElement.Should().NotBeNull();
        doc.DocumentElement!.Name.Should().Be("SearchResponse");
    }

    /// <summary>
    /// Format should handle empty results.
    /// </summary>
    [Fact]
    public void Format_WithEmptyResults_ShouldProduceValidXml()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(0);
        mockResponse.Setup(r => r.Documents).Returns([]);

        XmlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);
        XmlDocument doc = new();
        doc.LoadXml(result);

        // Assert
        XmlNode? totalNode = doc.SelectSingleNode("/SearchResponse/Total");
        totalNode!.InnerText.Should().Be("0");

        XmlNodeList? resultNodes = doc.SelectNodes("/SearchResponse/Results/Item");
        resultNodes!.Count.Should().Be(0);
    }

    /// <summary>
    /// Format should work with options.
    /// </summary>
    [Fact]
    public void Format_WithOptions_ShouldProduceValidXml()
    {
        // Arrange
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreateOption("services.nginx.enable", "Whether to enable nginx.")
        ]);

        XmlOutputFormatter<NixOption> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        Action act = () =>
        {
            XmlDocument doc = new();
            doc.LoadXml(result);
        };
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

        XmlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain(Environment.NewLine);
        result.Should().Contain("  "); // Check for indentation
    }

    /// <summary>
    /// Format should serialize package properties correctly.
    /// </summary>
    [Fact]
    public void Format_WithPackage_ShouldSerializeAllProperties()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreatePackage("vim", "nixpkgs.vim", "9.0.1234", "Vi IMproved text editor")
        ]);

        XmlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain("<AttrName>vim</AttrName>");
        result.Should().Contain("<Name>nixpkgs.vim</Name>");
        result.Should().Contain("<Version>9.0.1234</Version>");
        result.Should().Contain("<Description>Vi IMproved text editor</Description>");
    }
}
