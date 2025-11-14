// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.CommandLine;
using System.Linq;

using FluentAssertions;

using Nest;

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