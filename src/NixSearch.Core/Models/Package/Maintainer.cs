// SPDX-License-Identifier: MIT

using Nest;

namespace NixSearch.Core.Models.Package;

/// <summary>
/// Represents a package maintainer.
/// </summary>
public sealed record Maintainer
{
    /// <summary>
    /// Gets the maintainer's name.
    /// </summary>
    [PropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the maintainer's email address.
    /// </summary>
    [PropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Gets the maintainer's GitHub username.
    /// </summary>
    [PropertyName("github")]
    public string? GitHub { get; init; }
}
