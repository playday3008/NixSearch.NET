// SPDX-License-Identifier: MIT

using System;

using Microsoft.Extensions.Options;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Search.Builders;

namespace NixSearch.Core.Search;

/// <summary>
/// Main implementation of the NixSearch client.
/// </summary>
public sealed class NixSearchClient : INixSearchClient
{
    private readonly IElasticClient client;
    private readonly IOptions<NixSearchOptions> options;

    /// <summary>
    /// Initializes a new instance of the <see cref="NixSearchClient"/> class.
    /// </summary>
    /// <param name="client">The Elasticsearch client.</param>
    /// <param name="options">The NixSearch options.</param>
    /// <exception cref="ArgumentNullException">Thrown if client or options is null.</exception>
    public NixSearchClient(
        IElasticClient client,
        IOptions<NixSearchOptions> options)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(options);

        this.client = client;
        this.options = options;
    }

    /// <inheritdoc/>
    public PackageSearchBuilderBase Packages()
    {
        return new PackageSearchBuilder(this.client, this.options);
    }

    /// <inheritdoc/>
    public OptionSearchBuilderBase Options()
    {
        return new OptionSearchBuilder(this.client, this.options);
    }
}