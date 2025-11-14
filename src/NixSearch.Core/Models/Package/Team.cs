// SPDX-License-Identifier: MIT

using Nest;

namespace NixSearch.Core.Models.Package;

/// <summary>
/// Represents a package team.
/// </summary>
public sealed record Team
{
    /// <summary>
    /// Gets the team members.
    /// </summary>
    [PropertyName("members")]
    public required Maintainer[] Members { get; init; }

    /// <summary>
    /// Gets the team scope.
    /// </summary>
    [PropertyName("scope")]
    public string? Scope { get; init; }

    /// <summary>
    /// Gets the short name of the team.
    /// </summary>
    [PropertyName("shortName")]
    public string? ShortName { get; init; }

    /// <summary>
    /// Gets the GitHub team identifiers.
    /// </summary>
    [PropertyName("githubTeams")]
    public required string[] GitHubTeams { get; init; }
}