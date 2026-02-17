// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;

using FluentAssertions;

using Moq;

using Nest;

using NixSearch.CLI.Formatters;
using NixSearch.CLI.Tests.TestHelpers;
using NixSearch.Core.Models;

using YamlDotNet.Serialization;

namespace NixSearch.CLI.Tests.Formatters;

/// <summary>
/// Tests for <see cref="YamlOutputFormatter{T}"/>.
/// </summary>
public class YamlOutputFormatterTests
{
    /// <summary>
    /// Format should produce valid YAML output.
    /// </summary>
    [Fact]
    public void Format_WithPackages_ShouldProduceValidYaml()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreatePackage("vim", "vim", "9.0", "Vi IMproved text editor")
        ]);

        YamlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().NotBeNullOrEmpty();
        IDeserializer deserializer = new DeserializerBuilder().Build();
        Action act = () => deserializer.Deserialize<object>(result);
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

        YamlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain("total:");
        result.Should().Contain("results:");
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

        YamlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain("total:");
        result.Should().Contain("results:");
    }

    /// <summary>
    /// Format should handle empty results.
    /// </summary>
    [Fact]
    public void Format_WithEmptyResults_ShouldProduceValidYaml()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(0);
        mockResponse.Setup(r => r.Documents).Returns([]);

        YamlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain("total: 0");
        result.Should().Contain("results: []");
    }

    /// <summary>
    /// Format should work with options.
    /// </summary>
    [Fact]
    public void Format_WithOptions_ShouldProduceValidYaml()
    {
        // Arrange
        Mock<ISearchResponse<NixOption>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreateOption("services.nginx.enable", "Whether to enable nginx.")
        ]);

        YamlOutputFormatter<NixOption> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        IDeserializer deserializer = new DeserializerBuilder().Build();
        Action act = () => deserializer.Deserialize<object>(result);
        act.Should().NotThrow();
    }

    /// <summary>
    /// Format should contain package information in YAML format.
    /// </summary>
    [Fact]
    public void Format_WithPackages_ShouldContainPackageInfo()
    {
        // Arrange
        Mock<ISearchResponse<NixPackage>> mockResponse = new();
        mockResponse.Setup(r => r.Total).Returns(1);
        mockResponse.Setup(r => r.Documents).Returns(
        [
            TestDataFactory.CreatePackage("vim", "vim", "9.0")
        ]);

        YamlOutputFormatter<NixPackage> formatter = new();

        // Act
        string result = formatter.Format(mockResponse.Object, detailed: false);

        // Assert
        result.Should().Contain("attrName: vim");
        result.Should().Contain("name: vim");
        result.Should().Contain("version: 9.0");
    }
}
