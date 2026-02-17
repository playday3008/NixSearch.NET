// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using FluentAssertions;

using NixSearch.MCP.Models;

namespace NixSearch.MCP.Tests.Models;

/// <summary>
/// Unit tests for <see cref="Warning"/>.
/// </summary>
public class WarningTests
{
    /// <summary>
    /// Tests that Warning can be created with all required properties.
    /// </summary>
    [Fact]
    public void Warning_WithRequiredProperties_ShouldCreate()
    {
        // Arrange & Act
        Warning warning = new()
        {
            Code = "WARN001",
            Message = "This is a warning message",
        };

        // Assert
        warning.Should().NotBeNull();
        warning.Code.Should().Be("WARN001");
        warning.Message.Should().Be("This is a warning message");
        warning.Parameter.Should().BeNull();
    }

    /// <summary>
    /// Tests that Warning can be created with optional Parameter property.
    /// </summary>
    [Fact]
    public void Warning_WithParameter_ShouldCreate()
    {
        // Arrange & Act
        Warning warning = new()
        {
            Code = "WARN002",
            Message = "Invalid parameter value",
            Parameter = "size",
        };

        // Assert
        warning.Should().NotBeNull();
        warning.Code.Should().Be("WARN002");
        warning.Message.Should().Be("Invalid parameter value");
        warning.Parameter.Should().Be("size");
    }

    /// <summary>
    /// Tests that Warning is a record and supports value equality.
    /// </summary>
    [Fact]
    public void Warning_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        Warning warning1 = new()
        {
            Code = "WARN001",
            Message = "Test message",
            Parameter = "test",
        };

        Warning warning2 = new()
        {
            Code = "WARN001",
            Message = "Test message",
            Parameter = "test",
        };

        // Act & Assert
        warning1.Should().Be(warning2);
        (warning1 == warning2).Should().BeTrue();
    }

    /// <summary>
    /// Tests that Warning with different values are not equal.
    /// </summary>
    [Fact]
    public void Warning_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        Warning warning1 = new()
        {
            Code = "WARN001",
            Message = "Test message",
        };

        Warning warning2 = new()
        {
            Code = "WARN002",
            Message = "Different message",
        };

        // Act & Assert
        warning1.Should().NotBe(warning2);
        (warning1 == warning2).Should().BeFalse();
    }

    /// <summary>
    /// Tests that Warning with null Parameter is not equal to one with a Parameter value.
    /// </summary>
    [Fact]
    public void Warning_WithNullParameter_ShouldNotEqualWarningWithParameter()
    {
        // Arrange
        Warning warning1 = new()
        {
            Code = "WARN001",
            Message = "Test message",
            Parameter = null,
        };

        Warning warning2 = new()
        {
            Code = "WARN001",
            Message = "Test message",
            Parameter = "test",
        };

        // Act & Assert
        warning1.Should().NotBe(warning2);
    }

    /// <summary>
    /// Tests that Warning supports record with expression.
    /// </summary>
    [Fact]
    public void Warning_WithExpression_ShouldCreateNewInstance()
    {
        // Arrange
        Warning original = new()
        {
            Code = "WARN001",
            Message = "Original message",
        };

        // Act
        Warning modified = original with { Message = "Modified message" };

        // Assert
        original.Code.Should().Be("WARN001");
        original.Message.Should().Be("Original message");
        modified.Code.Should().Be("WARN001");
        modified.Message.Should().Be("Modified message");
        modified.Should().NotBe(original);
    }
}
