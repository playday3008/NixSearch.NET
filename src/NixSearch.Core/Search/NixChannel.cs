// SPDX-License-Identifier: MIT

namespace NixSearch.Core.Search;

/// <summary>
/// Represents a NixOS channel.
/// </summary>
public struct NixChannel
{
    private const string UnstableValue = "nixos-unstable";
    private const string StableValue = "nixos-25.05";
    private const string FlakesValue = "group-manual";

    private NixChannel(string value) => this.Value = value;

    /// <summary>
    /// Gets the unstable NixOS channel.
    /// </summary>
    public static NixChannel Unstable => new(UnstableValue);

    /// <summary>
    /// Gets the stable NixOS channel.
    /// </summary>
    public static NixChannel Stable => new(StableValue);

    /// <summary>
    /// Gets the flakes NixOS channel.
    /// </summary>
    public static NixChannel Flakes => new(FlakesValue);

    private string Value { get; init; }

    /// <summary>
    /// Returns the string representation of the channel.
    /// </summary>
    /// <returns>The string representation of the channel.</returns>
    public override readonly string ToString() => this.Value;
}