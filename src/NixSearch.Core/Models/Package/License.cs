// SPDX-License-Identifier: MIT

using Nest;

namespace NixSearch.Core.Models.Package;

/// <summary>
/// Represents a package license.
/// </summary>
public sealed record License
{
    /// <summary>
    /// Initializes a new instance of the <see cref="License"/> class.
    /// </summary>
    public License()
    {
    }

    /// <summary>
    /// Gets the license URL.
    /// </summary>
    [PropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets the full name of the license.
    /// </summary>
    [PropertyName("fullName")]
    public string? FullName { get; init; }
}