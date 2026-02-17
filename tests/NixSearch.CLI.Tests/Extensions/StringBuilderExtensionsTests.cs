// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Text;

using FluentAssertions;

using NixSearch.CLI.Extensions;

namespace NixSearch.CLI.Tests.Extensions;

/// <summary>
/// Tests for <see cref="StringBuilderExtensions"/>.
/// </summary>
public class StringBuilderExtensionsTests
{
    /// <summary>
    /// AppendInvariantLine should append text using invariant culture.
    /// </summary>
    [Fact]
    public void AppendInvariantLine_WithInteger_ShouldUseInvariantCulture()
    {
        // Arrange
        StringBuilder sb = new();

        // Act
        sb.AppendInvariantLine($"Count: {1000}");

        // Assert
        string result = sb.ToString();
        result.Should().Contain("Count: 1000");
    }

    /// <summary>
    /// AppendInvariantLine should append text with formatting.
    /// </summary>
    [Fact]
    public void AppendInvariantLine_WithFormattedDouble_ShouldUseInvariantCulture()
    {
        // Arrange
        StringBuilder sb = new();

        // Act
        sb.AppendInvariantLine($"Value: {3.14159:F2}");

        // Assert
        string result = sb.ToString();
        result.Should().Contain("Value: 3.14");
    }

    /// <summary>
    /// AppendInvariantLine should append text with string interpolation.
    /// </summary>
    [Fact]
    public void AppendInvariantLine_WithString_ShouldAppendCorrectly()
    {
        // Arrange
        StringBuilder sb = new();
        string name = "vim";

        // Act
        sb.AppendInvariantLine($"Package: {name}");

        // Assert
        string result = sb.ToString();
        result.Should().Contain("Package: vim");
        result.Should().EndWith(Environment.NewLine);
    }

    /// <summary>
    /// AppendInvariantLine should append multiple lines correctly.
    /// </summary>
    [Fact]
    public void AppendInvariantLine_MultipleLines_ShouldAppendCorrectly()
    {
        // Arrange
        StringBuilder sb = new();

        // Act
        sb.AppendInvariantLine($"Line 1: {1}");
        sb.AppendInvariantLine($"Line 2: {2}");
        sb.AppendInvariantLine($"Line 3: {3}");

        // Assert
        string result = sb.ToString();
        result.Should().Contain("Line 1: 1");
        result.Should().Contain("Line 2: 2");
        result.Should().Contain("Line 3: 3");
    }

    /// <summary>
    /// AppendInvariantLine should handle empty interpolation.
    /// </summary>
    [Fact]
    public void AppendInvariantLine_WithEmptyString_ShouldAppendNewline()
    {
        // Arrange
        StringBuilder sb = new();

        // Act
        sb.AppendInvariantLine($"");

        // Assert
        string result = sb.ToString();
        result.Should().Be(Environment.NewLine);
    }

    /// <summary>
    /// AppendInvariantLine should return same StringBuilder instance.
    /// </summary>
    [Fact]
    public void AppendInvariantLine_ShouldReturnSameInstance()
    {
        // Arrange
        StringBuilder sb = new();

        // Act
        StringBuilder result = sb.AppendInvariantLine($"Test");

        // Assert
        result.Should().BeSameAs(sb);
    }
}
