// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
    private IReadOnlyList<NixChannel>? cachedChannels;

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
    public OptionSearchBuilder Options()
    {
        return new OptionSearchBuilder(this.client, this.options);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<NixChannel>> GetChannelsAsync(CancellationToken cancellationToken = default)
    {
        if (this.cachedChannels is not null)
        {
            return this.cachedChannels;
        }

        int schemaVersion = this.options.Value.MappingSchemaVersion;
        string aliasPrefix = $"latest-{schemaVersion}-";

        CatResponse<CatAliasesRecord> catResponse = await this.client.Cat.AliasesAsync(
            c => c.Name($"{aliasPrefix}*"),
            cancellationToken);

        List<NixChannel> channels = catResponse.Records
            .Select(r => r.Alias)
            .Where(a => a.StartsWith(aliasPrefix, StringComparison.Ordinal))
            .Select(a => NixChannel.FromValue(a[aliasPrefix.Length..]))
            .Distinct()
            .ToList();

        this.cachedChannels = channels.AsReadOnly();
        return this.cachedChannels;
    }
}
