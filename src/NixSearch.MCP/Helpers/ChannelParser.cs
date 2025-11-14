// SPDX-License-Identifier: MIT

using System;

using NixSearch.Core.Search;

namespace NixSearch.MCP.Helpers;

/// <summary>
/// Helper class for parsing channel strings.
/// </summary>
public static class ChannelParser
{
    /// <summary>
    /// Parses a channel string to a NixChannel.
    /// </summary>
    /// <param name="channel">The channel string (unstable, stable, flakes).</param>
    /// <returns>The corresponding NixChannel.</returns>
    /// <exception cref="ArgumentException">Thrown when the channel is invalid.</exception>
    public static NixChannel Parse(string channel)
    {
        return channel.ToLowerInvariant() switch
        {
            "unstable" => NixChannel.Unstable,
            "stable" => NixChannel.Stable,
            "flakes" => NixChannel.Flakes,
            _ => throw new ArgumentException(
                $"Invalid channel '{channel}'. Valid values: unstable, stable, flakes",
                nameof(channel)),
        };
    }
}