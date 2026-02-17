// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
    OptionSearchBuilder Options();

    /// <summary>
    /// Gets the available NixOS channels by querying Elasticsearch indices.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only list of available channels.</returns>
    Task<IReadOnlyList<NixChannel>> GetChannelsAsync(CancellationToken cancellationToken = default);
}
