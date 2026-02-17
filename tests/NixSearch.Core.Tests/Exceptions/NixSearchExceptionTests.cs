// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;

using FluentAssertions;

using NixSearch.Core.Exceptions;

namespace NixSearch.Core.Tests.Exceptions;

/// <summary>
/// Tests for <see cref="NixSearchException"/>.
/// </summary>
public class NixSearchExceptionTests
{
    /// <summary>
    /// Default constructor should create exception.
    /// </summary>
    [Fact]
    public void Constructor_Default_ShouldCreateException()
    {
        // Act
        NixSearchException exception = new();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNullOrEmpty();
        exception.InnerException.Should().BeNull();
    }

    /// <summary>
    /// Constructor with message should set message.
    /// </summary>
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        const string message = "Test error message";

        // Act
        NixSearchException exception = new(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    /// <summary>
    /// Constructor with message and inner exception should set both.
    /// </summary>
    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
    {
        // Arrange
        const string message = "Test error message";
        InvalidOperationException innerException = new("Inner exception message");

        // Act
        NixSearchException exception = new(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(innerException);
    }

    /// <summary>
    /// Constructor with message and null inner exception should set message only.
    /// </summary>
    [Fact]
    public void Constructor_WithMessageAndNullInnerException_ShouldSetMessageOnly()
    {
        // Arrange
        const string message = "Test error message";

        // Act
        NixSearchException exception = new(message, null);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }
}
