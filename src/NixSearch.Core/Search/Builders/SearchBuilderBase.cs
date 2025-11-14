// SPDX-License-Identifier: MIT

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Elasticsearch.Net;

using Microsoft.Extensions.Options;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Exceptions;

namespace NixSearch.Core.Search.Builders;

/// <summary>
/// Base interface for search builders.
/// </summary>
/// <typeparam name="TSource">The type of the source document.</typeparam>
/// <typeparam name="TBuilder">The type of the builder (for fluent chaining).</typeparam>
public abstract class SearchBuilderBase<TSource, TBuilder>
    where TBuilder : SearchBuilderBase<TSource, TBuilder>
    where TSource : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchBuilderBase{TSource, TBuilder}"/> class.
    /// </summary>
    /// <param name="client">The Elasticsearch client.</param>
    /// <param name="options">The NixSearch options.</param>
    protected SearchBuilderBase(IElasticClient client, IOptions<NixSearchOptions> options)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(options?.Value);

        this.Client = client;
        this.Configuration = options.Value;
    }

    /// <summary>
    /// Gets the Elasticsearch client.
    /// </summary>
    protected IElasticClient Client { get; }

    /// <summary>
    /// Gets the NixSearch configuration options.
    /// </summary>
    protected NixSearchOptions Configuration { get; }

    /// <summary>
    /// Gets or sets the NixOS channel to search in.
    /// </summary>
    protected NixChannel Channel { get; set; } = NixChannel.Unstable;

    /// <summary>
    /// Gets or sets the search query.
    /// </summary>
    protected string Query { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the starting offset (0-based).
    /// </summary>
    protected int From { get; set; }

    /// <summary>
    /// Gets or sets the number of results to return.
    /// </summary>
    protected int Size { get; set; } = 50;

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    protected SortOrder? Order { get; set; }

    /// <summary>
    /// Sets the NixOS channel to search in.
    /// </summary>
    /// <param name="channel">The channel name (e.g., "unstable", "23.11").</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public TBuilder ForChannel(in NixChannel channel)
    {
        this.Channel = channel;
        return (TBuilder)this;
    }

    /// <summary>
    /// Sets the search query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public TBuilder WithQuery(in string query)
    {
        if (query is null)
        {
            return (TBuilder)this;
        }

        this.Query = query;
        return (TBuilder)this;
    }

    /// <summary>
    /// Sets pagination parameters.
    /// </summary>
    /// <param name="from">The starting offset (0-based).</param>
    /// <param name="size">The number of results to return.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if from is less than 0 or size is less than or equal to 0.</exception>
    public TBuilder Page(in int from, in int size)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(from, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(size, 0);

        this.From = from;
        this.Size = size;
        return (TBuilder)this;
    }

    /// <summary>
    /// Sets the sort order.
    /// </summary>
    /// <param name="order">The sort order.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public TBuilder SortBy(in SortOrder? order)
    {
        this.Order = order;
        return (TBuilder)this;
    }

    /// <summary>
    /// Executes the search query synchronously.
    /// </summary>
    /// <returns>The search results.</returns>
    /// <exception cref="NixSearchException">Thrown when the search request fails.</exception>
    public ISearchResponse<TSource> Execute()
    {
        SearchDescriptor<TSource> searchDescriptor = this.GetSearchDescriptor();

        this.InspectSearchDescriptor(searchDescriptor);

        ISearchResponse<TSource> searchResponse = this.Client.Search<TSource>(searchDescriptor);

        this.InspectSearchResponse(searchResponse);

        SearchBuilderBase<TSource, TBuilder>.ValidateSearchResponse(searchResponse);

        return searchResponse;
    }

    /// <summary>
    /// Executes the search query asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The search results.</returns>
    /// <exception cref="NixSearchException">Thrown when the search request fails.</exception>
    public async Task<ISearchResponse<TSource>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        SearchDescriptor<TSource> searchDescriptor = this.GetSearchDescriptor();

        this.InspectSearchDescriptor(searchDescriptor);

        ISearchResponse<TSource> searchResponse = await this.Client.SearchAsync<TSource>(searchDescriptor, cancellationToken);

        this.InspectSearchResponse(searchResponse);

        SearchBuilderBase<TSource, TBuilder>.ValidateSearchResponse(searchResponse);

        return searchResponse;
    }

    /// <summary>
    /// Gets the index name to search in.
    /// </summary>
    /// <returns>The index name.</returns>
    protected virtual string GetIndexName()
    {
        return $"latest-{this.Configuration.MappingSchemaVersion}-{this.Channel}";
    }

    /// <summary>
    /// Gets the fields to match against.
    /// </summary>
    /// <returns>The array of match field names.</returns>
    protected abstract string[] GetMatchFields();

    /// <summary>
    /// Gets the sort descriptor for the search.
    /// </summary>
    /// <returns>The sort descriptor.</returns>
    protected abstract SortDescriptor<TSource> GetSortDescriptor();

    /// <summary>
    /// Gets the search descriptor for the search.
    /// </summary>
    /// <returns>The search descriptor.</returns>
    protected abstract SearchDescriptor<TSource> GetSearchDescriptor();

    private static void ValidateSearchResponse(in ISearchResponse<TSource> searchResponse)
    {
        if (!searchResponse.IsValid)
        {
            string errorMessage = searchResponse.OriginalException != null
                ? $"Search request failed: {searchResponse.OriginalException.Message}"
                : $"Search request failed: {searchResponse.ServerError?.Error?.Reason ?? "Unknown error"}";

            throw new NixSearchException(errorMessage, searchResponse.OriginalException);
        }
    }

    [Conditional("DEBUG")]
    [ExcludeFromCodeCoverage]
    private void InspectSearchDescriptor(in SearchDescriptor<TSource> searchDescriptor)
    {
        if (this.Configuration.EnableDebugMode && Debugger.IsAttached)
        {
            string str = this.Client.RequestResponseSerializer.SerializeToString(searchDescriptor);
            _ = str;
            Debugger.Break();
        }
    }

    [Conditional("DEBUG")]
    [ExcludeFromCodeCoverage]
    private void InspectSearchResponse(in ISearchResponse<TSource> searchResponse)
    {
        if (this.Configuration.EnableDebugMode && Debugger.IsAttached)
        {
            string str = searchResponse.DebugInformation;
            _ = str;
            Debugger.Break();
        }
    }
}