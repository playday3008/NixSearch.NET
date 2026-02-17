// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.CommandLine;
using System.Linq;

using FluentAssertions;

namespace NixSearch.CLI.Tests.Commands;

/// <summary>
/// Tests for <see cref="PackagesCommand"/>.
/// </summary>
public class PackagesCommandTests
{
    /// <summary>
    /// Create should return a valid command.
    /// </summary>
    [Fact]
    public void Create_ShouldReturnValidCommand()
    {
        // Act
        Command command = PackagesCommand.Create();

        // Assert
        command.Should().NotBeNull();
        command.Name.Should().Be("packages");
        command.Description.Should().Be("Search for NixOS packages");
    }

    /// <summary>
    /// Create should include package-specific options.
    /// </summary>
    [Fact]
    public void Create_ShouldIncludePackageSpecificOptions()
    {
        // Act
        Command command = PackagesCommand.Create();

        // Assert
        command.Options.Should().Contain(o => o.Name == "--platform");
        command.Options.Should().Contain(o => o.Name == "--package-set");
        command.Options.Should().Contain(o => o.Name == "--license");
        command.Options.Should().Contain(o => o.Name == "--maintainer");
        command.Options.Should().Contain(o => o.Name == "--team");
    }

    /// <summary>
    /// Create should include common search options.
    /// </summary>
    [Fact]
    public void Create_ShouldIncludeCommonSearchOptions()
    {
        // Act
        Command command = PackagesCommand.Create();

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
        Command command = PackagesCommand.Create();

        // Assert
        command.Arguments.Should().HaveCount(1);
        command.Arguments.First().Name.Should().Be("query");
    }

    /// <summary>
    /// Package-specific options should allow multiple values.
    /// </summary>
    [Fact]
    public void Create_PackageSpecificOptions_ShouldAllowMultipleValues()
    {
        // Act
        Command command = PackagesCommand.Create();

        // Assert
        Option platformOption = command.Options.First(o => o.Name == "--platform");
        platformOption.Should().BeOfType<Option<string[]?>>();

        Option packageSetOption = command.Options.First(o => o.Name == "--package-set");
        packageSetOption.Should().BeOfType<Option<string[]?>>();

        Option licenseOption = command.Options.First(o => o.Name == "--license");
        licenseOption.Should().BeOfType<Option<string[]?>>();

        Option maintainerOption = command.Options.First(o => o.Name == "--maintainer");
        maintainerOption.Should().BeOfType<Option<string[]?>>();

        Option teamOption = command.Options.First(o => o.Name == "--team");
        teamOption.Should().BeOfType<Option<string[]?>>();
    }

    /// <summary>
    /// Package-specific options should have descriptions.
    /// </summary>
    [Fact]
    public void Create_PackageSpecificOptions_ShouldHaveDescriptions()
    {
        // Act
        Command command = PackagesCommand.Create();

        // Assert
        Option platformOption = command.Options.First(o => o.Name == "--platform");
        platformOption.Description.Should().Contain("platform");

        Option packageSetOption = command.Options.First(o => o.Name == "--package-set");
        packageSetOption.Description.Should().Contain("package set");

        Option licenseOption = command.Options.First(o => o.Name == "--license");
        licenseOption.Description.Should().Contain("license");

        Option maintainerOption = command.Options.First(o => o.Name == "--maintainer");
        maintainerOption.Description.Should().Contain("maintainer");

        Option teamOption = command.Options.First(o => o.Name == "--team");
        teamOption.Description.Should().Contain("team");
    }
}
