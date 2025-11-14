// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;

using FluentAssertions;

using NixSearch.Core.Search;
using NixSearch.MCP.Helpers;

namespace NixSearch.MCP.Tests.Helpers;

/// <summary>
/// Tests for <see cref="ChannelParser"/>.
/// </summary>
public class ChannelParserTests
{
    /// <summary>
    /// Tests that Parse with "unstable" returns NixChannel.Unstable.
    /// </summary>
    [Fact]
    public void Parse_WithUnstableChannel_ShouldReturnUnstable()
    {
        // Act
        NixChannel result = ChannelParser.Parse("unstable");

        // Assert
        result.Should().Be(NixChannel.Unstable);
    }

    /// <summary>
    /// Tests that Parse with "stable" returns NixChannel.Stable.
    /// </summary>
    [Fact]
    public void Parse_WithStableChannel_ShouldReturnStable()
    {
        // Act
        NixChannel result = ChannelParser.Parse("stable");

        // Assert
        result.Should().Be(NixChannel.Stable);
    }

    /// <summary>
    /// Tests that Parse with "flakes" returns NixChannel.Flakes.
    /// </summary>
    [Fact]
    public void Parse_WithFlakesChannel_ShouldReturnFlakes()
    {
        // Act
        NixChannel result = ChannelParser.Parse("flakes");

        // Assert
        result.Should().Be(NixChannel.Flakes);
    }

    /// <summary>
    /// Tests that Parse with uppercase channel names works correctly.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    [Theory]
    [InlineData("UNSTABLE")]
    [InlineData("Unstable")]
    [InlineData("UnStAbLe")]
    public void Parse_WithMixedCaseUnstable_ShouldReturnUnstable(string channel)
    {
        // Act
        NixChannel result = ChannelParser.Parse(channel);

        // Assert
        result.Should().Be(NixChannel.Unstable);
    }

    /// <summary>
    /// Tests that Parse with stable channel in different cases works correctly.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    [Theory]
    [InlineData("STABLE")]
    [InlineData("Stable")]
    [InlineData("StAbLe")]
    public void Parse_WithMixedCaseStable_ShouldReturnStable(string channel)
    {
        // Act
        NixChannel result = ChannelParser.Parse(channel);

        // Assert
        result.Should().Be(NixChannel.Stable);
    }

    /// <summary>
    /// Tests that Parse with flakes channel in different cases works correctly.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    [Theory]
    [InlineData("FLAKES")]
    [InlineData("Flakes")]
    [InlineData("FlAkEs")]
    public void Parse_WithMixedCaseFlakes_ShouldReturnFlakes(string channel)
    {
        // Act
        NixChannel result = ChannelParser.Parse(channel);

        // Assert
        result.Should().Be(NixChannel.Flakes);
    }

    /// <summary>
    /// Tests that Parse with invalid channel throws ArgumentException.
    /// </summary>
    /// <param name="channel">The invalid channel name.</param>
    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData("nixos-23.11")]
    [InlineData("testing")]
    [InlineData("beta")]
    public void Parse_WithInvalidChannel_ShouldThrowArgumentException(string channel)
    {
        // Act
        Action act = () => ChannelParser.Parse(channel);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"Invalid channel '{channel}'. Valid values: unstable, stable, flakes*")
            .WithParameterName(nameof(channel));
    }

    /// <summary>
    /// Tests that Parse with all valid channels returns correct enum values.
    /// </summary>
    [Fact]
    public void Parse_WithAllValidChannels_ShouldReturnCorrectEnums()
    {
        // Arrange
        string[] validChannels = ["unstable", "stable", "flakes"];
        NixChannel[] expectedResults = [NixChannel.Unstable, NixChannel.Stable, NixChannel.Flakes];

        // Act & Assert
        for (int i = 0; i < validChannels.Length; i++)
        {
            NixChannel result = ChannelParser.Parse(validChannels[i]);
            result.Should().Be(expectedResults[i]);
        }
    }
}