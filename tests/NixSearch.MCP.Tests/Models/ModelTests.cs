// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.Collections.Generic;

using FluentAssertions;

using NixSearch.MCP.Models;

namespace NixSearch.MCP.Tests.Models;

/// <summary>
/// Tests for MCP model classes.
/// </summary>
public class ModelTests
{
    /// <summary>
    /// Tests that SearchResponse can be created with all required fields.
    /// </summary>
    [Fact]
    public void SearchResponse_WithRequiredFields_ShouldBeCreated()
    {
        // Arrange & Act
        SearchResponse<string> response = new()
        {
            Total = 100,
            Page = 0,
            Size = 50,
            HasMore = true,
            Results = ["result1", "result2"],
        };

        // Assert
        response.Should().NotBeNull();
        response.Total.Should().Be(100);
        response.Page.Should().Be(0);
        response.Size.Should().Be(50);
        response.HasMore.Should().BeTrue();
        response.Results.Should().HaveCount(2);
    }

    /// <summary>
    /// Tests that SearchResponse with warnings works correctly.
    /// </summary>
    [Fact]
    public void SearchResponse_WithWarnings_ShouldIncludeWarnings()
    {
        // Arrange
        List<Warning> warnings =
        [
            new() { Code = "W001", Message = "Deprecated parameter" },
            new() { Code = "W002", Message = "Slow query" },
        ];

        // Act
        SearchResponse<string> response = new()
        {
            Total = 10,
            Page = 0,
            Size = 10,
            HasMore = false,
            Results = ["result"],
            Warnings = warnings,
        };

        // Assert
        response.Warnings.Should().NotBeNull();
        response.Warnings.Should().HaveCount(2);
        response.Warnings![0].Code.Should().Be("W001");
        response.Warnings[1].Message.Should().Be("Slow query");
    }

    /// <summary>
    /// Tests that SearchResponse with empty results works correctly.
    /// </summary>
    [Fact]
    public void SearchResponse_WithEmptyResults_ShouldWork()
    {
        // Arrange & Act
        SearchResponse<PackageResult> response = new()
        {
            Total = 0,
            Page = 0,
            Size = 50,
            HasMore = false,
            Results = [],
        };

        // Assert
        response.Total.Should().Be(0);
        response.Results.Should().BeEmpty();
        response.HasMore.Should().BeFalse();
    }

    /// <summary>
    /// Tests that PackageResult can be created with all required fields.
    /// </summary>
    [Fact]
    public void PackageResult_WithRequiredFields_ShouldBeCreated()
    {
        // Arrange & Act
        PackageResult package = new()
        {
            AttrName = "firefox",
            AttrSet = "nixpkgs",
            Name = "firefox",
            Version = "120.0",
            Platforms = ["x86_64-linux"],
            LicenseNames = ["mpl20"],
            MaintainerNames = ["john"],
            TeamNames = [],
            Programs = ["firefox"],
            System = "x86_64-linux",
        };

        // Assert
        package.Should().NotBeNull();
        package.AttrName.Should().Be("firefox");
        package.Name.Should().Be("firefox");
        package.Version.Should().Be("120.0");
    }

    /// <summary>
    /// Tests that PackageResult with all optional fields works correctly.
    /// </summary>
    [Fact]
    public void PackageResult_WithAllFields_ShouldIncludeOptionalFields()
    {
        // Arrange & Act
        PackageResult package = new()
        {
            AttrName = "nginx",
            AttrSet = "nixpkgs",
            Name = "nginx",
            Version = "1.24.0",
            Description = "Web server",
            LongDescription = "A high-performance web server",
            Platforms = ["x86_64-linux", "aarch64-linux"],
            LicenseNames = ["bsd2"],
            MaintainerNames = ["alice", "bob"],
            TeamNames = ["web-servers"],
            Homepage = ["https://nginx.org"],
            Programs = ["nginx"],
            MainProgram = "nginx",
            System = "x86_64-linux",
            Position = "pkgs/servers/http/nginx/default.nix:42",
            FlakeName = "nixpkgs",
            FlakeDescription = "NixOS package collection",
        };

        // Assert
        package.Description.Should().Be("Web server");
        package.LongDescription.Should().Be("A high-performance web server");
        package.Homepage.Should().ContainSingle("https://nginx.org");
        package.MainProgram.Should().Be("nginx");
        package.Position.Should().Be("pkgs/servers/http/nginx/default.nix:42");
        package.FlakeName.Should().Be("nixpkgs");
    }

    /// <summary>
    /// Tests that OptionResult can be created with required fields.
    /// </summary>
    [Fact]
    public void OptionResult_WithRequiredFields_ShouldBeCreated()
    {
        // Arrange & Act
        OptionResult option = new()
        {
            Name = "services.nginx.enable",
        };

        // Assert
        option.Should().NotBeNull();
        option.Name.Should().Be("services.nginx.enable");
    }

    /// <summary>
    /// Tests that OptionResult with all fields works correctly.
    /// </summary>
    [Fact]
    public void OptionResult_WithAllFields_ShouldIncludeAllFields()
    {
        // Arrange & Act
        OptionResult option = new()
        {
            Name = "services.postgresql.enable",
            Description = "Enable PostgreSQL",
            Type = "boolean",
            Default = "false",
            Example = "true",
            Source = "nixos/modules/services/databases/postgresql.nix:100",
            FlakeName = "nixpkgs",
            FlakeDescription = "NixOS package collection",
        };

        // Assert
        option.Name.Should().Be("services.postgresql.enable");
        option.Description.Should().Be("Enable PostgreSQL");
        option.Type.Should().Be("boolean");
        option.Default.Should().Be("false");
        option.Example.Should().Be("true");
        option.Source.Should().Be("nixos/modules/services/databases/postgresql.nix:100");
        option.FlakeName.Should().Be("nixpkgs");
        option.FlakeDescription.Should().Be("NixOS package collection");
    }

    /// <summary>
    /// Tests that Warning can be created with all required fields.
    /// </summary>
    [Fact]
    public void Warning_WithRequiredFields_ShouldBeCreated()
    {
        // Arrange & Act
        Warning warning = new()
        {
            Code = "W001",
            Message = "Deprecated parameter",
        };

        // Assert
        warning.Should().NotBeNull();
        warning.Code.Should().Be("W001");
        warning.Message.Should().Be("Deprecated parameter");
        warning.Parameter.Should().BeNull();
    }

    /// <summary>
    /// Tests that Warning with parameter field works correctly.
    /// </summary>
    [Fact]
    public void Warning_WithParameter_ShouldIncludeParameter()
    {
        // Arrange & Act
        Warning warning = new()
        {
            Code = "W002",
            Message = "Invalid parameter value",
            Parameter = "channel",
        };

        // Assert
        warning.Parameter.Should().Be("channel");
    }

    /// <summary>
    /// Tests that record equality works for PackageResult.
    /// </summary>
    [Fact]
    public void PackageResult_RecordEquality_ShouldWork()
    {
        // Arrange
        PackageResult package1 = new()
        {
            AttrName = "test",
            AttrSet = "nixpkgs",
            Name = "test",
            Version = "1.0",
            Platforms = [],
            LicenseNames = [],
            MaintainerNames = [],
            TeamNames = [],
            Programs = [],
            System = "x86_64-linux",
        };

        PackageResult package2 = new()
        {
            AttrName = "test",
            AttrSet = "nixpkgs",
            Name = "test",
            Version = "1.0",
            Platforms = [],
            LicenseNames = [],
            MaintainerNames = [],
            TeamNames = [],
            Programs = [],
            System = "x86_64-linux",
        };

        // Act & Assert
        package1.Should().Be(package2);
    }

    /// <summary>
    /// Tests that record equality works for OptionResult.
    /// </summary>
    [Fact]
    public void OptionResult_RecordEquality_ShouldWork()
    {
        // Arrange
        OptionResult option1 = new()
        {
            Name = "test.option",
            Type = "boolean",
        };

        OptionResult option2 = new()
        {
            Name = "test.option",
            Type = "boolean",
        };

        // Act & Assert
        option1.Should().Be(option2);
    }

    /// <summary>
    /// Tests that record equality works for Warning.
    /// </summary>
    [Fact]
    public void Warning_RecordEquality_ShouldWork()
    {
        // Arrange
        Warning warning1 = new()
        {
            Code = "W001",
            Message = "Test warning",
        };

        Warning warning2 = new()
        {
            Code = "W001",
            Message = "Test warning",
        };

        // Act & Assert
        warning1.Should().Be(warning2);
    }

    /// <summary>
    /// Tests that SearchResponse with different types works correctly.
    /// </summary>
    [Fact]
    public void SearchResponse_WithDifferentTypes_ShouldWork()
    {
        // Arrange & Act
        SearchResponse<PackageResult> packageResponse = new()
        {
            Total = 1,
            Page = 0,
            Size = 10,
            HasMore = false,
            Results =
            [
                new()
                {
                    AttrName = "test",
                    AttrSet = "nixpkgs",
                    Name = "test",
                    Version = "1.0",
                    Platforms = [],
                    LicenseNames = [],
                    MaintainerNames = [],
                    TeamNames = [],
                    Programs = [],
                    System = "x86_64-linux",
                },
            ],
        };

        SearchResponse<OptionResult> optionResponse = new()
        {
            Total = 1,
            Page = 0,
            Size = 10,
            HasMore = false,
            Results = [new() { Name = "test.option" }],
        };

        // Assert
        packageResponse.Results.Should().HaveCount(1);
        packageResponse.Results[0].Should().BeOfType<PackageResult>();
        optionResponse.Results.Should().HaveCount(1);
        optionResponse.Results[0].Should().BeOfType<OptionResult>();
    }
}