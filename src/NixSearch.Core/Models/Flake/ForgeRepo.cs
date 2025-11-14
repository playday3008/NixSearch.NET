// SPDX-License-Identifier: MIT

namespace NixSearch.Core.Models.Flake;

/// <summary>
/// Represents a repository hosted on a forge (e.g., GitHub, GitLab, SourceHut).
/// </summary>
public record ForgeRepo : Repo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForgeRepo"/> class.
    /// </summary>
    public ForgeRepo()
    {
    }

    /// <summary>
    /// Gets the repository owner.
    /// </summary>
    public new required string Owner { get; init; }

    /// <summary>
    /// Gets the repository name.
    /// </summary>
    public new required string RepoName { get; init; }
}