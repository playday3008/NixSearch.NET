// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using FluentAssertions;

using NixSearch.Core.Models;
using NixSearch.Core.Models.Package;
using NixSearch.MCP.Mappers;
using NixSearch.MCP.Models;

namespace NixSearch.MCP.Tests.Mappers;

/// <summary>
/// Tests for <see cref="ModelMappers"/>.
/// </summary>
public class ModelMappersTests
{
    /// <summary>
    /// Tests that ToPackageResult maps all required fields correctly.
    /// </summary>
    [Fact]
    public void ToPackageResult_WithValidPackage_ShouldMapAllFields()
    {
        // Arrange
        NixPackage package = new()
        {
            AttrName = "firefox",
            AttrSet = "nixpkgs",
            Name = "firefox",
            Version = "120.0",
            Description = "A web browser",
            LongDescription = "Mozilla Firefox web browser",
            Platforms = ["x86_64-linux", "aarch64-linux"],
            LicenseSet = ["mpl20"],
            MaintainersSet = ["john", "jane"],
            TeamsSet = ["mozilla"],
            Homepage = ["https://firefox.com"],
            Programs = ["firefox"],
            MainProgram = "firefox",
            System = "x86_64-linux",
            Position = "pkgs/applications/networking/browsers/firefox/default.nix:42",
            FlakeName = "nixpkgs",
            FlakeDescription = "NixOS package collection",
            Outputs = ["out"],
            License = [],
            Maintainers = [],
            Teams = [],
        };

        // Act
        PackageResult result = package.ToPackageResult();

        // Assert
        result.Should().NotBeNull();
        result.AttrName.Should().Be(package.AttrName);
        result.AttrSet.Should().Be(package.AttrSet);
        result.Name.Should().Be(package.Name);
        result.Version.Should().Be(package.Version);
        result.Description.Should().Be(package.Description);
        result.LongDescription.Should().Be(package.LongDescription);
        result.Platforms.Should().BeEquivalentTo(package.Platforms);
        result.LicenseNames.Should().BeEquivalentTo(package.LicenseSet);
        result.MaintainerNames.Should().BeEquivalentTo(package.MaintainersSet);
        result.TeamNames.Should().BeEquivalentTo(package.TeamsSet);
        result.Homepage.Should().BeEquivalentTo(package.Homepage);
        result.Programs.Should().BeEquivalentTo(package.Programs);
        result.MainProgram.Should().Be(package.MainProgram);
        result.System.Should().Be(package.System);
        result.Position.Should().Be(package.Position);
        result.FlakeName.Should().Be(package.FlakeName);
        result.FlakeDescription.Should().Be(package.FlakeDescription);
    }

    /// <summary>
    /// Tests that ToPackageResult maps optional fields correctly when null.
    /// </summary>
    [Fact]
    public void ToPackageResult_WithNullOptionalFields_ShouldMapCorrectly()
    {
        // Arrange
        NixPackage package = new()
        {
            AttrName = "testpkg",
            AttrSet = "nixpkgs",
            Name = "testpkg",
            Version = "1.0.0",
            Description = null,
            LongDescription = null,
            Platforms = [],
            LicenseSet = [],
            MaintainersSet = [],
            TeamsSet = [],
            Homepage = null,
            Programs = [],
            MainProgram = null,
            System = "x86_64-linux",
            Position = null,
            FlakeName = null,
            FlakeDescription = null,
            Outputs = [],
            License = [],
            Maintainers = [],
            Teams = [],
        };

        // Act
        PackageResult result = package.ToPackageResult();

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().BeNull();
        result.LongDescription.Should().BeNull();
        result.Homepage.Should().BeNull();
        result.MainProgram.Should().BeNull();
        result.Position.Should().BeNull();
        result.FlakeName.Should().BeNull();
        result.FlakeDescription.Should().BeNull();
    }

    /// <summary>
    /// Tests that ToPackageResult maps empty arrays correctly.
    /// </summary>
    [Fact]
    public void ToPackageResult_WithEmptyArrays_ShouldMapCorrectly()
    {
        // Arrange
        NixPackage package = new()
        {
            AttrName = "testpkg",
            AttrSet = "nixpkgs",
            Name = "testpkg",
            Version = "1.0.0",
            Platforms = [],
            LicenseSet = [],
            MaintainersSet = [],
            TeamsSet = [],
            Programs = [],
            System = "x86_64-linux",
            Outputs = [],
            License = [],
            Maintainers = [],
            Teams = [],
        };

        // Act
        PackageResult result = package.ToPackageResult();

        // Assert
        result.Platforms.Should().BeEmpty();
        result.LicenseNames.Should().BeEmpty();
        result.MaintainerNames.Should().BeEmpty();
        result.TeamNames.Should().BeEmpty();
        result.Programs.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that ToOptionResult maps all required fields correctly.
    /// </summary>
    [Fact]
    public void ToOptionResult_WithValidOption_ShouldMapAllFields()
    {
        // Arrange
        NixOption option = new()
        {
            Name = "services.nginx.enable",
            Description = "Enable nginx service",
            Type = "boolean",
            Default = "false",
            Example = "true",
            Source = "nixos/modules/services/web-servers/nginx/default.nix:42",
            FlakeName = "nixpkgs",
            FlakeDescription = "NixOS package collection",
        };

        // Act
        OptionResult result = option.ToOptionResult();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(option.Name);
        result.Description.Should().Be(option.Description);
        result.Type.Should().Be(option.Type);
        result.Default.Should().Be(option.Default);
        result.Example.Should().Be(option.Example);
        result.Source.Should().Be(option.Source);
        result.FlakeName.Should().Be(option.FlakeName);
        result.FlakeDescription.Should().Be(option.FlakeDescription);
    }

    /// <summary>
    /// Tests that ToOptionResult maps optional fields correctly when null.
    /// </summary>
    [Fact]
    public void ToOptionResult_WithNullOptionalFields_ShouldMapCorrectly()
    {
        // Arrange
        NixOption option = new()
        {
            Name = "test.option",
            Description = null,
            Type = null,
            Default = null,
            Example = null,
            Source = null,
            FlakeName = null,
            FlakeDescription = null,
        };

        // Act
        OptionResult result = option.ToOptionResult();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(option.Name);
        result.Description.Should().BeNull();
        result.Type.Should().BeNull();
        result.Default.Should().BeNull();
        result.Example.Should().BeNull();
        result.Source.Should().BeNull();
        result.FlakeName.Should().BeNull();
        result.FlakeDescription.Should().BeNull();
    }

    /// <summary>
    /// Tests that ToOptionResult with minimal fields works correctly.
    /// </summary>
    [Fact]
    public void ToOptionResult_WithMinimalFields_ShouldMapCorrectly()
    {
        // Arrange
        NixOption option = new()
        {
            Name = "minimal.option",
        };

        // Act
        OptionResult result = option.ToOptionResult();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("minimal.option");
    }

    /// <summary>
    /// Tests that ToPackageResult preserves special characters in fields.
    /// </summary>
    [Fact]
    public void ToPackageResult_WithSpecialCharacters_ShouldPreserveCharacters()
    {
        // Arrange
        NixPackage package = new()
        {
            AttrName = "python3Packages.requests",
            AttrSet = "python3Packages",
            Name = "requests",
            Version = "2.31.0",
            Description = "HTTP library for Python - supports HTTP/1.1, HTTP/2.0, and more!",
            Platforms = ["x86_64-linux", "aarch64-darwin", "i686-linux"],
            LicenseSet = ["apache-2.0"],
            MaintainersSet = ["user@example.com"],
            TeamsSet = [],
            Programs = [],
            System = "x86_64-linux",
            Outputs = [],
            License = [],
            Maintainers = [],
            Teams = [],
        };

        // Act
        PackageResult result = package.ToPackageResult();

        // Assert
        result.AttrName.Should().Be("python3Packages.requests");
        result.Description.Should().Contain("HTTP/1.1");
    }

    /// <summary>
    /// Tests that ToOptionResult preserves special characters in fields.
    /// </summary>
    [Fact]
    public void ToOptionResult_WithSpecialCharacters_ShouldPreserveCharacters()
    {
        // Arrange
        NixOption option = new()
        {
            Name = "services.web-server.nginx.virtualHosts.<name>.enable",
            Description = "Enable virtual host <name> with SSL/TLS support",
            Type = "attrsOf (submodule { ... })",
            Example = "{ \"example.com\" = { enableACME = true; }; }",
        };

        // Act
        OptionResult result = option.ToOptionResult();

        // Assert
        result.Name.Should().Contain("<name>");
        result.Description.Should().Contain("SSL/TLS");
        result.Type.Should().Contain("attrsOf");
    }
}