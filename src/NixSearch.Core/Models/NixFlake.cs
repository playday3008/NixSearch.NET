// SPDX-License-Identifier: MIT

using Nest;

using NixSearch.Core.Models.Flake;

namespace NixSearch.Core.Models;

/// <summary>
/// Represents a Nix flake package.
/// </summary>
public abstract record NixFlake : BaseModel
{
    /// <summary>
    /// Gets the flake description.
    /// </summary>
    [PropertyName("flake_description")]
    public string? FlakeDescription { get; init; }

    /// <summary>
    /// Gets the resolved flake information.
    /// </summary>
    [PropertyName("flake_resolved")]
    public Repo? FlakeResolved { get; init; }

    /// <summary>
    /// Gets the flake name.
    /// </summary>
    [PropertyName("flake_name")]
    public string? FlakeName { get; init; }

    /// <summary>
    /// Gets the flake revision.
    /// </summary>
    [PropertyName("revision")]
    public string? FlakeRevision { get; init; }
}