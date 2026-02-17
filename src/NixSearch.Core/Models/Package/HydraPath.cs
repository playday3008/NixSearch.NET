// SPDX-License-Identifier: MIT

using Nest;

namespace NixSearch.Core.Models.Package;

/// <summary>
/// Represents a Hydra output path.
/// </summary>
public sealed record HydraPath
{
    /// <summary>
    /// Gets the output name.
    /// </summary>
    [PropertyName("output")]
    public required string Output { get; init; }

    /// <summary>
    /// Gets the store path.
    /// </summary>
    [PropertyName("path")]
    public required string Path { get; init; }
}
