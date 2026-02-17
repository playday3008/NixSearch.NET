// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NixSearch.Core.Search;

/// <summary>
/// Represents a NixOS channel.
/// </summary>
public partial struct NixChannel : IEquatable<NixChannel>
{
    private const string UnstableValue = "nixos-unstable";
    private const string FlakesValue = "group-manual";

    private NixChannel(string value) => this.Value = value;

    /// <summary>
    /// Gets the unstable NixOS channel.
    /// </summary>
    public static NixChannel Unstable => new(UnstableValue);

    /// <summary>
    /// Gets the flakes NixOS channel.
    /// </summary>
    public static NixChannel Flakes => new(FlakesValue);

    /// <summary>
    /// Gets the channel value string.
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    /// Gets a value indicating whether this channel is a stable release channel (e.g., nixos-24.11).
    /// </summary>
    public readonly bool IsStable => this.Value is not null && StablePattern().IsMatch(this.Value);

    /// <summary>
    /// Gets a value indicating whether this channel is a beta release channel (e.g., nixos-25.11-beta).
    /// </summary>
    public readonly bool IsBeta => this.Value is not null && BetaPattern().IsMatch(this.Value);

    /// <summary>
    /// Gets a value indicating whether this channel is the unstable channel.
    /// </summary>
    public readonly bool IsUnstable => string.Equals(this.Value, UnstableValue, StringComparison.Ordinal);

    /// <summary>
    /// Gets a value indicating whether this channel is the flakes channel.
    /// </summary>
    public readonly bool IsFlakes => string.Equals(this.Value, FlakesValue, StringComparison.Ordinal);

    /// <summary>
    /// Equality operator.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if equal.</returns>
    public static bool operator ==(NixChannel left, NixChannel right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if not equal.</returns>
    public static bool operator !=(NixChannel left, NixChannel right) => !left.Equals(right);

    /// <summary>
    /// Creates a NixChannel from an arbitrary channel value string.
    /// </summary>
    /// <param name="value">The channel value (e.g., "nixos-24.11").</param>
    /// <returns>A NixChannel with the given value.</returns>
    /// <exception cref="ArgumentException">Thrown if value is null or whitespace.</exception>
    public static NixChannel FromValue(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new NixChannel(value);
    }

    /// <summary>
    /// Parses a user-friendly channel name into a NixChannel.
    /// All named channels are resolved dynamically from available channels.
    /// </summary>
    /// <param name="channel">The channel name (unstable, stable, beta, flakes).</param>
    /// <param name="availableChannels">
    /// List of available channels from discovery, required for resolving all named channels.
    /// </param>
    /// <returns>The corresponding NixChannel.</returns>
    /// <exception cref="ArgumentException">Thrown when the channel name is invalid.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when available channels are not provided or the requested channel is not found.
    /// </exception>
    public static NixChannel Parse(string channel, IReadOnlyList<NixChannel> availableChannels)
    {
        ArgumentNullException.ThrowIfNull(availableChannels);

        return channel.ToLowerInvariant() switch
        {
            "unstable" => ResolveByPredicate(availableChannels, c => c.IsUnstable, "unstable"),
            "flakes" => ResolveByPredicate(availableChannels, c => c.IsFlakes, "flakes"),
            "stable" => ResolveLatestVersioned(availableChannels, c => c.IsStable, "stable"),
            "beta" => ResolveLatestVersioned(availableChannels, c => c.IsBeta, "beta"),
            _ => throw new ArgumentException(
                $"Invalid channel '{channel}'. Valid values: unstable, stable, beta, flakes",
                nameof(channel)),
        };
    }

    /// <summary>
    /// Returns the string representation of the channel.
    /// </summary>
    /// <returns>The string representation of the channel.</returns>
    public override readonly string ToString() => this.Value;

    /// <inheritdoc/>
    public readonly bool Equals(NixChannel other) => string.Equals(this.Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj) => obj is NixChannel other && this.Equals(other);

    /// <inheritdoc/>
    public override readonly int GetHashCode() => this.Value is not null ? StringComparer.Ordinal.GetHashCode(this.Value) : 0;

    [GeneratedRegex(@"^nixos-\d+\.\d+$")]
    private static partial Regex StablePattern();

    [GeneratedRegex(@"^nixos-\d+\.\d+-beta$")]
    private static partial Regex BetaPattern();

    private static NixChannel ResolveByPredicate(
        IReadOnlyList<NixChannel> availableChannels,
        Func<NixChannel, bool> predicate,
        string channelName)
    {
        foreach (NixChannel channel in availableChannels)
        {
            if (predicate(channel))
            {
                return channel;
            }
        }

        throw new InvalidOperationException(
            $"No '{channelName}' channel found in available channels.");
    }

    private static NixChannel ResolveLatestVersioned(
        IReadOnlyList<NixChannel> availableChannels,
        Func<NixChannel, bool> predicate,
        string channelName)
    {
        NixChannel[] matching = availableChannels
            .Where(predicate)
            .OrderByDescending(c => c.Value, StringComparer.Ordinal)
            .ToArray();

        if (matching.Length == 0)
        {
            throw new InvalidOperationException(
                $"No '{channelName}' channel found in available channels.");
        }

        return matching[0];
    }
}
