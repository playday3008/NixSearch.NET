// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.CommandLine;
using System.Linq;

using FluentAssertions;

using Nest;

using NixSearch.CLI.Formatters;
using NixSearch.Core.Models;
using NixSearch.Core.Search;

namespace NixSearch.CLI.Tests.Commands;

/// <summary>
/// Tests for <see cref="BaseSearchCommand{T}"/>.
/// </summary>
public class BaseSearchCommandTests
{
    /// <summary>
    /// CreateCommand should create a command with correct name and description.
    /// </summary>
    [Fact]
    public void CreateCommand_ShouldSetNameAndDescription()
    {
        // Arrange
        TestSearchCommand command = new();

        // Act
        Command result = command.CreateTestCommand("test", "Test command");

        // Assert
        result.Name.Should().Be("test");
        result.Description.Should().Be("Test command");
    }

    /// <summary>
    /// CreateCommand should include required query argument.
    /// </summary>
    [Fact]
    public void CreateCommand_ShouldIncludeQueryArgument()
    {
        // Arrange
        TestSearchCommand command = new();

        // Act
        Command result = command.CreateTestCommand("test", "Test command");

        // Assert
        result.Arguments.Should().HaveCount(1);
        result.Arguments.First().Name.Should().Be("query");
    }

    /// <summary>
    /// CreateCommand should include common options.
    /// </summary>
    [Fact]
    public void CreateCommand_ShouldIncludeCommonOptions()
    {
        // Arrange
        TestSearchCommand command = new();

        // Act
        Command result = command.CreateTestCommand("test", "Test command");

        // Assert
        result.Options.Should().Contain(o => o.Name == "--channel");
        result.Options.Should().Contain(o => o.Name == "--from");
        result.Options.Should().Contain(o => o.Name == "--size");
        result.Options.Should().Contain(o => o.Name == "--sort");
        result.Options.Should().Contain(o => o.Name == "--format");
        result.Options.Should().Contain(o => o.Name == "--detailed");
    }

    /// <summary>
    /// CreateCommand should have default values for options.
    /// </summary>
    [Fact]
    public void CreateCommand_OptionsShouldHaveDefaultValues()
    {
        // Arrange
        TestSearchCommand command = new();

        // Act
        Command result = command.CreateTestCommand("test", "Test command");

        // Assert
        var channelOption = result.Options.First(o => o.Name == "--channel");
        channelOption.Should().BeOfType<Option<string>>();

        var fromOption = result.Options.First(o => o.Name == "--from");
        fromOption.Should().BeOfType<Option<int>>();

        var sizeOption = result.Options.First(o => o.Name == "--size");
        sizeOption.Should().BeOfType<Option<int>>();

        var formatOption = result.Options.First(o => o.Name == "--format");
        formatOption.Should().NotBeNull();

        var detailedOption = result.Options.First(o => o.Name == "--detailed");
        detailedOption.Should().BeOfType<Option<bool>>();
    }

    /// <summary>
    /// PackagesCommand.Create should create a packages command.
    /// </summary>
    [Fact]
    public void PackagesCommand_Create_ShouldCreateCommand()
    {
        // Act
        Command result = PackagesCommand.Create();

        // Assert
        result.Name.Should().Be("packages");
        result.Description.Should().Be("Search for NixOS packages");
    }

    /// <summary>
    /// OptionsCommand.Create should create an options command.
    /// </summary>
    [Fact]
    public void OptionsCommand_Create_ShouldCreateCommand()
    {
        // Act
        Command result = OptionsCommand.Create();

        // Assert
        result.Name.Should().Be("options");
        result.Description.Should().Be("Search for NixOS options");
    }

    /// <summary>
    /// ParseChannel should parse "unstable" to NixChannel.Unstable.
    /// </summary>
    [Fact]
    public void ParseChannel_WithUnstable_ShouldReturnUnstable()
    {
        // Act
        NixChannel result = BaseSearchCommand<NixPackage>.ParseChannel("unstable");

        // Assert
        result.Should().Be(NixChannel.Unstable);
    }

    /// <summary>
    /// ParseChannel should parse "stable" to NixChannel.Stable.
    /// </summary>
    [Fact]
    public void ParseChannel_WithStable_ShouldReturnStable()
    {
        // Act
        NixChannel result = BaseSearchCommand<NixPackage>.ParseChannel("stable");

        // Assert
        result.Should().Be(NixChannel.Stable);
    }

    /// <summary>
    /// ParseChannel should parse "flakes" to NixChannel.Flakes.
    /// </summary>
    [Fact]
    public void ParseChannel_WithFlakes_ShouldReturnFlakes()
    {
        // Act
        NixChannel result = BaseSearchCommand<NixPackage>.ParseChannel("flakes");

        // Assert
        result.Should().Be(NixChannel.Flakes);
    }

    /// <summary>
    /// ParseChannel should be case-insensitive.
    /// </summary>
    /// <param name="channel">The channel string to test.</param>
    [Theory]
    [InlineData("UNSTABLE")]
    [InlineData("Unstable")]
    [InlineData("UnStAbLe")]
    public void ParseChannel_ShouldBeCaseInsensitive(string channel)
    {
        // Act
        NixChannel result = BaseSearchCommand<NixPackage>.ParseChannel(channel);

        // Assert
        result.Should().Be(NixChannel.Unstable);
    }

    /// <summary>
    /// ParseChannel should throw ArgumentException for invalid channel.
    /// </summary>
    /// <param name="channel">The invalid channel string to test.</param>
    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData("testing")]
    public void ParseChannel_WithInvalidChannel_ShouldThrowArgumentException(string channel)
    {
        // Act
        Action act = () => BaseSearchCommand<NixPackage>.ParseChannel(channel);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"Invalid channel: {channel}*");
    }

    /// <summary>
    /// ParseSortOrder should parse "asc" to SortOrder.Ascending.
    /// </summary>
    [Fact]
    public void ParseSortOrder_WithAsc_ShouldReturnAscending()
    {
        // Act
        SortOrder? result = BaseSearchCommand<NixPackage>.ParseSortOrder("asc");

        // Assert
        result.Should().Be(SortOrder.Ascending);
    }

    /// <summary>
    /// ParseSortOrder should parse "desc" to SortOrder.Descending.
    /// </summary>
    [Fact]
    public void ParseSortOrder_WithDesc_ShouldReturnDescending()
    {
        // Act
        SortOrder? result = BaseSearchCommand<NixPackage>.ParseSortOrder("desc");

        // Assert
        result.Should().Be(SortOrder.Descending);
    }

    /// <summary>
    /// ParseSortOrder should return null for null input.
    /// </summary>
    [Fact]
    public void ParseSortOrder_WithNull_ShouldReturnNull()
    {
        // Act
        SortOrder? result = BaseSearchCommand<NixPackage>.ParseSortOrder(null);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// ParseSortOrder should be case-insensitive.
    /// </summary>
    /// <param name="sort">The sort string to test.</param>
    [Theory]
    [InlineData("ASC")]
    [InlineData("Asc")]
    [InlineData("AsC")]
    public void ParseSortOrder_ShouldBeCaseInsensitive(string sort)
    {
        // Act
        SortOrder? result = BaseSearchCommand<NixPackage>.ParseSortOrder(sort);

        // Assert
        result.Should().Be(SortOrder.Ascending);
    }

    /// <summary>
    /// ParseSortOrder should throw ArgumentException for invalid sort order.
    /// </summary>
    /// <param name="sort">The invalid sort string to test.</param>
    [Theory]
    [InlineData("invalid")]
    [InlineData("ascending")]
    [InlineData("descending")]
    public void ParseSortOrder_WithInvalidSort_ShouldThrowArgumentException(string sort)
    {
        // Act
        Action act = () => BaseSearchCommand<NixPackage>.ParseSortOrder(sort);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"Invalid sort order: {sort}*");
    }

    /// <summary>
    /// CreateFormatter should return JsonOutputFormatter for Json format.
    /// </summary>
    [Fact]
    public void CreateFormatter_WithJson_ShouldReturnJsonFormatter()
    {
        // Act
        IOutputFormatter<NixPackage> result = BaseSearchCommand<NixPackage>.CreateFormatter(OutputFormat.Json);

        // Assert
        result.Should().BeOfType<JsonOutputFormatter<NixPackage>>();
    }

    /// <summary>
    /// CreateFormatter should return YamlOutputFormatter for Yaml format.
    /// </summary>
    [Fact]
    public void CreateFormatter_WithYaml_ShouldReturnYamlFormatter()
    {
        // Act
        IOutputFormatter<NixPackage> result = BaseSearchCommand<NixPackage>.CreateFormatter(OutputFormat.Yaml);

        // Assert
        result.Should().BeOfType<YamlOutputFormatter<NixPackage>>();
    }

    /// <summary>
    /// CreateFormatter should return XmlOutputFormatter for Xml format.
    /// </summary>
    [Fact]
    public void CreateFormatter_WithXml_ShouldReturnXmlFormatter()
    {
        // Act
        IOutputFormatter<NixPackage> result = BaseSearchCommand<NixPackage>.CreateFormatter(OutputFormat.Xml);

        // Assert
        result.Should().BeOfType<XmlOutputFormatter<NixPackage>>();
    }

    /// <summary>
    /// CreateFormatter should return TextOutputFormatter for Text format.
    /// </summary>
    [Fact]
    public void CreateFormatter_WithText_ShouldReturnTextFormatter()
    {
        // Act
        IOutputFormatter<NixPackage> result = BaseSearchCommand<NixPackage>.CreateFormatter(OutputFormat.Text);

        // Assert
        result.Should().BeOfType<TextOutputFormatter<NixPackage>>();
    }

    /// <summary>
    /// CreateFormatter should return TextOutputFormatter for default/unknown format.
    /// </summary>
    [Fact]
    public void CreateFormatter_WithUnknownFormat_ShouldReturnTextFormatter()
    {
        // Act
        IOutputFormatter<NixPackage> result = BaseSearchCommand<NixPackage>.CreateFormatter((OutputFormat)999);

        // Assert
        result.Should().BeOfType<TextOutputFormatter<NixPackage>>();
    }

    /// <summary>
    /// Test implementation of BaseSearchCommand.
    /// </summary>
    private sealed class TestSearchCommand : BaseSearchCommand<NixPackage>
    {
        /// <summary>
        /// Public wrapper for CreateCommand to make it testable.
        /// </summary>
        /// <param name="name">Command name.</param>
        /// <param name="description">Command description.</param>
        /// <returns>The command.</returns>
        public Command CreateTestCommand(string name, string description)
        {
            return this.CreateCommand(name, description);
        }

        /// <inheritdoc/>
        protected override ISearchResponse<NixPackage> ExecuteSearch(
            ParseResult parseResult,
            INixSearchClient client,
            string query,
            NixChannel channel,
            int from,
            int size,
            SortOrder? sortOrder)
        {
            throw new NotImplementedException();
        }
    }
}