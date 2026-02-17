// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;

using FluentAssertions;

using NixSearch.Core.Configuration;

namespace NixSearch.Core.Tests.Configuration;

/// <summary>
/// Tests for <see cref="NixSearchOptions"/>.
/// </summary>
public class NixSearchOptionsTests
{
    /// <summary>
    /// Tests that the constructor sets default values correctly.
    /// </summary>
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Act
        NixSearchOptions options = new()
        {
            Username = "test",
            Password = "test",
        };

        // Assert
        options.Url.Should().Be("https://search.nixos.org/backend");
        options.MappingSchemaVersion.Should().Be(44);
        options.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        options.EnableDebugMode.Should().BeFalse();
    }

    /// <summary>
    /// Tests that all properties are initializable.
    /// </summary>
    [Fact]
    public void Everything_ShouldBeInitializable()
    {
        // Act
        NixSearchOptions options = new()
        {
            Url = "https://custom.url/backend",
            Username = "test",
            Password = "test",
            MappingSchemaVersion = 45,
            Timeout = TimeSpan.FromSeconds(60),
            EnableDebugMode = true,
        };

        // Assert
        options.Url.Should().Be("https://custom.url/backend");
        options.MappingSchemaVersion.Should().Be(45);
        options.Timeout.Should().Be(TimeSpan.FromSeconds(60));
        options.EnableDebugMode.Should().BeTrue();
    }

    /// <summary>
    /// Tests that the record supports equality comparison.
    /// </summary>
    [Fact]
    public void Options_ShouldSupportRecordEquality()
    {
        // Arrange
        NixSearchOptions lhs = new()
        {
            Url = "https://search.nixos.org/backend",
            Username = "test",
            Password = "test",
            MappingSchemaVersion = 44,
            Timeout = TimeSpan.FromSeconds(30),
            EnableDebugMode = false,
        };

        NixSearchOptions rhs = new()
        {
            Url = "https://search.nixos.org/backend",
            Username = "test",
            Password = "test",
            MappingSchemaVersion = 44,
            Timeout = TimeSpan.FromSeconds(30),
            EnableDebugMode = false,
        };

        // Assert
        lhs.Should().Be(rhs);
    }

    /// <summary>
    /// Tests that options with different values are not equal.
    /// </summary>
    [Fact]
    public void Options_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        NixSearchOptions options1 = new()
        {
            Url = "https://search.nixos.org/backend",
            Username = "test1",
            Password = "test1",
            MappingSchemaVersion = 44,
            Timeout = TimeSpan.FromSeconds(30),
            EnableDebugMode = false,
        };

        NixSearchOptions options2 = new()
        {
            Url = "https://search.nixos.org/backend",
            Username = "test2",
            Password = "test2",
            MappingSchemaVersion = 44,
            Timeout = TimeSpan.FromSeconds(30),
            EnableDebugMode = false,
        };

        // Assert
        options1.Should().NotBe(options2);
    }

    /// <summary>
    /// Tests that the record supports the 'with' expression.
    /// </summary>
    [Fact]
    public void Options_ShouldSupportWith()
    {
        // Arrange
        NixSearchOptions original = new()
        {
            Url = "https://search.nixos.org/backend",
            Username = "test",
            Password = "test",
            MappingSchemaVersion = 44,
            Timeout = TimeSpan.FromSeconds(30),
            EnableDebugMode = false,
        };

        // Act
        NixSearchOptions modified = original with { MappingSchemaVersion = 45 };

        // Assert
        modified.MappingSchemaVersion.Should().Be(45);
        modified.Url.Should().Be(original.Url);
        modified.Username.Should().Be(original.Username);
        original.MappingSchemaVersion.Should().Be(44);
    }

    /// <summary>
    /// Tests that options can be created with empty credentials.
    /// </summary>
    [Fact]
    public void Options_CanSetEmptyCredentials()
    {
        // Act
        NixSearchOptions options = new()
        {
            Username = string.Empty,
            Password = string.Empty,
        };

        // Assert
        options.Username.Should().BeEmpty();
        options.Password.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that all properties are accessible and settable.
    /// </summary>
    [Fact]
    public void Options_AllProperties_ShouldBeAccessible()
    {
        // Arrange
        NixSearchOptions options = new()
        {
            Url = "https://test.url",
            Username = "user",
            Password = "pass",
            MappingSchemaVersion = 50,
            Timeout = TimeSpan.FromMinutes(1),
            EnableDebugMode = true,
        };

        // Assert
        options.Url.Should().Be("https://test.url");
        options.Username.Should().Be("user");
        options.Password.Should().Be("pass");
        options.MappingSchemaVersion.Should().Be(50);
        options.Timeout.Should().Be(TimeSpan.FromMinutes(1));
        options.EnableDebugMode.Should().BeTrue();
    }
}