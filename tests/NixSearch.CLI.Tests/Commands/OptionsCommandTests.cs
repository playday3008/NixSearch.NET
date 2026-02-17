// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.CommandLine;
using System.Linq;

using FluentAssertions;

namespace NixSearch.CLI.Tests.Commands;

/// <summary>
/// Tests for <see cref="OptionsCommand"/>.
/// </summary>
public class OptionsCommandTests
{
    /// <summary>
    /// Create should return a valid command.
    /// </summary>
    [Fact]
    public void Create_ShouldReturnValidCommand()
    {
        // Act
        Command command = OptionsCommand.Create();

        // Assert
        command.Should().NotBeNull();
        command.Name.Should().Be("options");
        command.Description.Should().Be("Search for NixOS options");
    }

    /// <summary>
    /// Create should include common search options.
    /// </summary>
    [Fact]
    public void Create_ShouldIncludeCommonSearchOptions()
    {
        // Act
        Command command = OptionsCommand.Create();

        // Assert
        command.Options.Should().Contain(o => o.Name == "--channel");
        command.Options.Should().Contain(o => o.Name == "--from");
        command.Options.Should().Contain(o => o.Name == "--size");
        command.Options.Should().Contain(o => o.Name == "--sort");
        command.Options.Should().Contain(o => o.Name == "--format");
        command.Options.Should().Contain(o => o.Name == "--detailed");
    }

    /// <summary>
    /// Create should include query argument.
    /// </summary>
    [Fact]
    public void Create_ShouldIncludeQueryArgument()
    {
        // Act
        Command command = OptionsCommand.Create();

        // Assert
        command.Arguments.Should().HaveCount(1);
        command.Arguments.First().Name.Should().Be("query");
    }

    /// <summary>
    /// Create should not include package-specific options.
    /// </summary>
    [Fact]
    public void Create_ShouldNotIncludePackageSpecificOptions()
    {
        // Act
        Command command = OptionsCommand.Create();

        // Assert
        command.Options.Should().NotContain(o => o.Name == "platform");
        command.Options.Should().NotContain(o => o.Name == "package-set");
        command.Options.Should().NotContain(o => o.Name == "license");
        command.Options.Should().NotContain(o => o.Name == "maintainer");
        command.Options.Should().NotContain(o => o.Name == "team");
    }

    /// <summary>
    /// Create should have fewer options than PackagesCommand.
    /// </summary>
    [Fact]
    public void Create_ShouldHaveFewerOptionsThanPackagesCommand()
    {
        // Act
        Command optionsCommand = OptionsCommand.Create();
        Command packagesCommand = PackagesCommand.Create();

        // Assert
        optionsCommand.Options.Count.Should().BeLessThan(packagesCommand.Options.Count);
    }
}
