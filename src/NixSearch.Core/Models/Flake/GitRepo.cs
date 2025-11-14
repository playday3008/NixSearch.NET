// SPDX-License-Identifier: MIT

namespace NixSearch.Core.Models.Flake;

/// <summary>
/// Represents a Git repository.
/// </summary>
public record GitRepo : Repo
{
    /// <summary>
    /// Gets the repository URL.
    /// </summary>
    public new required string Url { get; init; }
}