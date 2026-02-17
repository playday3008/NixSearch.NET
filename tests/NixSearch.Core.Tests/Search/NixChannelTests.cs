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
    private static readonly List<NixChannel> AllChannels =
    [
        NixChannel.Unstable,
        NixChannel.FromValue("nixos-24.11"),
        NixChannel.FromValue("nixos-25.05"),
        NixChannel.Flakes,
    ];

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
    [InlineData("nixos-25.05-beta")]
    public void IsStable_WithNonStablePattern_ShouldReturnFalse(string value)
    {
        // Act
        NixChannel channel = NixChannel.FromValue(value);

        // Assert
        channel.IsStable.Should().BeFalse();
    }

    /// <summary>
    /// Tests that IsBeta returns true for beta channel patterns.
    /// </summary>
    /// <param name="value">The channel value.</param>
    [Theory]
    [InlineData("nixos-25.05-beta")]
    [InlineData("nixos-24.11-beta")]
    public void IsBeta_WithBetaPattern_ShouldReturnTrue(string value)
    {
        // Act
        NixChannel channel = NixChannel.FromValue(value);

        // Assert
        channel.IsBeta.Should().BeTrue();
    }

    /// <summary>
    /// Tests that IsBeta returns false for non-beta channel patterns.
    /// </summary>
    /// <param name="value">The channel value.</param>
    [Theory]
    [InlineData("nixos-unstable")]
    [InlineData("nixos-24.11")]
    [InlineData("group-manual")]
    public void IsBeta_WithNonBetaPattern_ShouldReturnFalse(string value)
    {
        // Act
        NixChannel channel = NixChannel.FromValue(value);

        // Assert
        channel.IsBeta.Should().BeFalse();
    }

    /// <summary>
    /// Tests that IsUnstable returns true for the unstable channel.
    /// </summary>
    [Fact]
    public void IsUnstable_WithUnstableChannel_ShouldReturnTrue()
    {
        // Act
        NixChannel channel = NixChannel.Unstable;

        // Assert
        channel.IsUnstable.Should().BeTrue();
    }

    /// <summary>
    /// Tests that IsUnstable returns false for non-unstable channels.
    /// </summary>
    /// <param name="value">The channel value.</param>
    [Theory]
    [InlineData("nixos-24.11")]
    [InlineData("group-manual")]
    public void IsUnstable_WithNonUnstableChannel_ShouldReturnFalse(string value)
    {
        // Act
        NixChannel channel = NixChannel.FromValue(value);

        // Assert
        channel.IsUnstable.Should().BeFalse();
    }

    /// <summary>
    /// Tests that IsFlakes returns true for the flakes channel.
    /// </summary>
    [Fact]
    public void IsFlakes_WithFlakesChannel_ShouldReturnTrue()
    {
        // Act
        NixChannel channel = NixChannel.Flakes;

        // Assert
        channel.IsFlakes.Should().BeTrue();
    }

    /// <summary>
    /// Tests that IsFlakes returns false for non-flakes channels.
    /// </summary>
    /// <param name="value">The channel value.</param>
    [Theory]
    [InlineData("nixos-unstable")]
    [InlineData("nixos-24.11")]
    public void IsFlakes_WithNonFlakesChannel_ShouldReturnFalse(string value)
    {
        // Act
        NixChannel channel = NixChannel.FromValue(value);

        // Assert
        channel.IsFlakes.Should().BeFalse();
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
        NixChannel result = NixChannel.Parse("unstable", AllChannels);

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
        NixChannel result = NixChannel.Parse("flakes", AllChannels);

        // Assert
        result.Should().Be(NixChannel.Flakes);
    }

    /// <summary>
    /// Tests that Parse is case-insensitive for unstable.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    [Theory]
    [InlineData("UNSTABLE")]
    [InlineData("Unstable")]
    [InlineData("UnStAbLe")]
    public void Parse_WithMixedCaseUnstable_ShouldReturnUnstable(string channel)
    {
        // Act
        NixChannel result = NixChannel.Parse(channel, AllChannels);

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
        NixChannel result = NixChannel.Parse(channel, AllChannels);

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
    public void Parse_WithInvalidChannel_ShouldThrowArgumentException(string channel)
    {
        // Act
        Action act = () => NixChannel.Parse(channel, AllChannels);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"Invalid channel '{channel}'. Valid values: unstable, stable, beta, flakes*")
            .WithParameterName(nameof(channel));
    }

    /// <summary>
    /// Tests that Parse with null available channels throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void Parse_WithNullAvailableChannels_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => NixChannel.Parse("unstable", null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that Parse with "unstable" and no unstable channel throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void Parse_WithUnstableAndNoUnstableChannel_ShouldThrowInvalidOperationException()
    {
        // Arrange
        List<NixChannel> channels =
        [
            NixChannel.FromValue("nixos-24.11"),
            NixChannel.Flakes,
        ];

        // Act
        Action act = () => NixChannel.Parse("unstable", channels);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that Parse with "flakes" and no flakes channel throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void Parse_WithFlakesAndNoFlakesChannel_ShouldThrowInvalidOperationException()
    {
        // Arrange
        List<NixChannel> channels =
        [
            NixChannel.Unstable,
            NixChannel.FromValue("nixos-24.11"),
        ];

        // Act
        Action act = () => NixChannel.Parse("flakes", channels);

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

    /// <summary>
    /// Tests that Parse with "beta" resolves to the latest beta channel.
    /// </summary>
    [Fact]
    public void Parse_WithBeta_ShouldReturnLatestBeta()
    {
        // Arrange
        List<NixChannel> channels =
        [
            NixChannel.Unstable,
            NixChannel.FromValue("nixos-24.11"),
            NixChannel.FromValue("nixos-25.05-beta"),
            NixChannel.FromValue("nixos-25.11-beta"),
            NixChannel.Flakes,
        ];

        // Act
        NixChannel result = NixChannel.Parse("beta", channels);

        // Assert
        result.Value.Should().Be("nixos-25.11-beta");
        result.IsBeta.Should().BeTrue();
    }

    /// <summary>
    /// Tests that Parse with "beta" selects the highest beta version.
    /// </summary>
    [Fact]
    public void Parse_WithBeta_ShouldSelectHighestVersion()
    {
        // Arrange
        List<NixChannel> channels =
        [
            NixChannel.FromValue("nixos-24.11-beta"),
            NixChannel.FromValue("nixos-25.11-beta"),
            NixChannel.FromValue("nixos-25.05-beta"),
        ];

        // Act
        NixChannel result = NixChannel.Parse("beta", channels);

        // Assert
        result.Value.Should().Be("nixos-25.11-beta");
    }

    /// <summary>
    /// Tests that Parse with "beta" and no beta channels throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void Parse_WithBetaAndNoBetaChannels_ShouldThrowInvalidOperationException()
    {
        // Arrange
        List<NixChannel> channels =
        [
            NixChannel.Unstable,
            NixChannel.FromValue("nixos-24.11"),
            NixChannel.Flakes,
        ];

        // Act
        Action act = () => NixChannel.Parse("beta", channels);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that Parse with mixed case "beta" works.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    [Theory]
    [InlineData("BETA")]
    [InlineData("Beta")]
    [InlineData("BeTa")]
    public void Parse_WithMixedCaseBeta_ShouldResolveBeta(string channel)
    {
        // Arrange
        List<NixChannel> channels =
        [
            NixChannel.FromValue("nixos-25.05-beta"),
        ];

        // Act
        NixChannel result = NixChannel.Parse(channel, channels);

        // Assert
        result.Value.Should().Be("nixos-25.05-beta");
    }
}
