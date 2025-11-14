// SPDX-License-Identifier: MIT

using NixSearch.Core.Search.Builders;

namespace NixSearch.Core.Search;

/// <summary>
/// Main interface for NixSearch client.
/// </summary>
public interface INixSearchClient
{
    /// <summary>
    /// Creates a package search builder.
    /// </summary>
    /// <returns>A package search builder instance.</returns>
    PackageSearchBuilderBase Packages();

    /// <summary>
    /// Creates an option search builder.
    /// </summary>
    /// <returns>An option search builder instance.</returns>
    OptionSearchBuilderBase Options();
}