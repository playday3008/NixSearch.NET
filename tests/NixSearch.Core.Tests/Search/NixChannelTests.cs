// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Collections.Generic;

using FluentAssertions;

using NixSearch.Core.Search;

namespace NixSearch.Core.Tests.Search;

/// <summary>
/// Tests for <see cref="NixChannel"/>.
/// </summary>
public class NixChannelTests
{
    /// <summary>
    /// Tests that Unstable returns the correct value.
    /// </summary>
    [Fact]
    public void Unstable_ShouldReturnCorrectValue()
    {
        // Act
        NixChannel channel = NixChannel.Unstable;

        // Assert
        channel.Value.Should().Be("nixos-unstable");
        channel.ToString().Should().Be("nixos-unstable");
    }

    /// <summary>
    /// Tests that Flakes returns the correct value.
    /// </summary>
    [Fact]
    public void Flakes_ShouldReturnCorrectValue()
    {
        // Act
        NixChannel channel = NixChannel.Flakes;

        // Assert
        channel.Value.Should().Be("group-manual");
        channel.ToString().Should().Be("group-manual");
    }

    /// <summary>
    /// Tests that FromValue creates a channel with the given value.
    /// </summary>
    [Fact]
    public void FromValue_WithValidValue_ShouldCreateChannel()
    {
        // Act
        NixChannel channel = NixChannel.FromValue("nixos-24.11");

        // Assert
        channel.Value.Should().Be("nixos-24.11");
        channel.ToString().Should().Be("nixos-24.11");
    }

    /// <summary>
    /// Tests that FromValue throws for null or whitespace.
    /// </summary>
    /// <param name="value">The invalid value.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FromValue_WithNullOrWhitespace_ShouldThrow(string? value)
    {
        // Act
        Action act = () => NixChannel.FromValue(value!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Tests that IsStable returns true for stable channel patterns.
    /// </summary>
    /// <param name="value">The channel value.</param>
    [Theory]
    [InlineData("nixos-24.11")]
    [InlineData("nixos-25.05")]
    [InlineData("nixos-23.11")]
    public void IsStable_WithStablePattern_ShouldReturnTrue(string value)
    {
        // Act
        NixChannel channel = NixChannel.FromValue(value);

        // Assert
        channel.IsStable.Should().BeTrue();
    }

    /// <summary>
    /// Tests that IsStable returns false for non-stable channel patterns.
    /// </summary>
    /// <param name="value">The channel value.</param>
    [Theory]
    [InlineData("nixos-unstable")]
    [InlineData("group-manual")]
    [InlineData("nixos-unstable-small")]
    [InlineData("nixpkgs-24.11")]
    public void IsStable_WithNonStablePattern_ShouldReturnFalse(string value)
    {
        // Act
        NixChannel channel = NixChannel.FromValue(value);

        // Assert
        channel.IsStable.Should().BeFalse();
    }

    /// <summary>
    /// Tests that Equals works correctly for equal channels.
    /// </summary>
    [Fact]
    public void Equals_WithEqualChannels_ShouldReturnTrue()
    {
        // Arrange
        NixChannel a = NixChannel.Unstable;
        NixChannel b = NixChannel.Unstable;

        // Assert
        a.Equals(b).Should().BeTrue();
        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    /// <summary>
    /// Tests that Equals works correctly for different channels.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentChannels_ShouldReturnFalse()
    {
        // Arrange
        NixChannel a = NixChannel.Unstable;
        NixChannel b = NixChannel.Flakes;

        // Assert
        a.Equals(b).Should().BeFalse();
        (a == b).Should().BeFalse();
        (a != b).Should().BeTrue();
    }

    /// <summary>
    /// Tests that Equals handles object boxing correctly.
    /// </summary>
    [Fact]
    public void Equals_WithBoxedObject_ShouldWork()
    {
        // Arrange
        NixChannel a = NixChannel.Unstable;
        object b = NixChannel.Unstable;
        object c = "not a channel";

        // Assert
        a.Equals(b).Should().BeTrue();
        a.Equals(c).Should().BeFalse();
        a.Equals(null).Should().BeFalse();
    }

    /// <summary>
    /// Tests that Parse with "unstable" returns NixChannel.Unstable.
    /// </summary>
    [Fact]
    public void Parse_WithUnstable_ShouldReturnUnstable()
    {
        // Act
        NixChannel result = NixChannel.Parse("unstable");

        // Assert
        result.Should().Be(NixChannel.Unstable);
    }

    /// <summary>
    /// Tests that Parse with "flakes" returns NixChannel.Flakes.
    /// </summary>
    [Fact]
    public void Parse_WithFlakes_ShouldReturnFlakes()
    {
        // Act
        NixChannel result = NixChannel.Parse("flakes");

        // Assert
        result.Should().Be(NixChannel.Flakes);
    }

    /// <summary>
    /// Tests that Parse is case-insensitive.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    [Theory]
    [InlineData("UNSTABLE")]
    [InlineData("Unstable")]
    [InlineData("UnStAbLe")]
    public void Parse_WithMixedCaseUnstable_ShouldReturnUnstable(string channel)
    {
        // Act
        NixChannel result = NixChannel.Parse(channel);

        // Assert
        result.Should().Be(NixChannel.Unstable);
    }

    /// <summary>
    /// Tests that Parse with mixed case flakes works correctly.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    [Theory]
    [InlineData("FLAKES")]
    [InlineData("Flakes")]
    [InlineData("FlAkEs")]
    public void Parse_WithMixedCaseFlakes_ShouldReturnFlakes(string channel)
    {
        // Act
        NixChannel result = NixChannel.Parse(channel);

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
        Action act = () => NixChannel.Parse(channel);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"Invalid channel '{channel}'. Valid values: unstable, stable, flakes*")
            .WithParameterName(nameof(channel));
    }

    /// <summary>
    /// Tests that Parse with "stable" without available channels throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void Parse_WithStableWithoutChannels_ShouldThrowInvalidOperationException()
    {
        // Act
        Action act = () => NixChannel.Parse("stable");

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that Parse with "stable" and empty channels throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void Parse_WithStableAndEmptyChannels_ShouldThrowInvalidOperationException()
    {
        // Act
        Action act = () => NixChannel.Parse("stable", []);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that Parse with "stable" and no stable channels throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void Parse_WithStableAndNoStableChannels_ShouldThrowInvalidOperationException()
    {
        // Arrange
        List<NixChannel> channels =
        [
            NixChannel.Unstable,
            NixChannel.Flakes,
        ];

        // Act
        Action act = () => NixChannel.Parse("stable", channels);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that Parse with "stable" and available channels resolves to the latest stable.
    /// </summary>
    [Fact]
    public void Parse_WithStableAndAvailableChannels_ShouldReturnLatestStable()
    {
        // Arrange
        List<NixChannel> channels =
        [
            NixChannel.Unstable,
            NixChannel.FromValue("nixos-24.05"),
            NixChannel.FromValue("nixos-24.11"),
            NixChannel.Flakes,
        ];

        // Act
        NixChannel result = NixChannel.Parse("stable", channels);

        // Assert
        result.Value.Should().Be("nixos-24.11");
        result.IsStable.Should().BeTrue();
    }

    /// <summary>
    /// Tests that Parse with "stable" selects the highest version.
    /// </summary>
    [Fact]
    public void Parse_WithStable_ShouldSelectHighestVersion()
    {
        // Arrange
        List<NixChannel> channels =
        [
            NixChannel.FromValue("nixos-23.11"),
            NixChannel.FromValue("nixos-25.05"),
            NixChannel.FromValue("nixos-24.11"),
        ];

        // Act
        NixChannel result = NixChannel.Parse("stable", channels);

        // Assert
        result.Value.Should().Be("nixos-25.05");
    }

    /// <summary>
    /// Tests that Parse with mixed case "stable" works.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    [Theory]
    [InlineData("STABLE")]
    [InlineData("Stable")]
    [InlineData("StAbLe")]
    public void Parse_WithMixedCaseStable_ShouldResolveStable(string channel)
    {
        // Arrange
        List<NixChannel> channels =
        [
            NixChannel.FromValue("nixos-24.11"),
        ];

        // Act
        NixChannel result = NixChannel.Parse(channel, channels);

        // Assert
        result.Value.Should().Be("nixos-24.11");
    }
}
