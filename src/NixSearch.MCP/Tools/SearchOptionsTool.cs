// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ModelContextProtocol.Server;

using NixSearch.Core.Models;
using NixSearch.Core.Search;
using NixSearch.Core.Search.Builders;
using NixSearch.MCP.Models;

namespace NixSearch.MCP.Tools;

/// <summary>
/// MCP tool for searching NixOS options.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SearchOptionsTool"/> class.
/// </remarks>
/// <param name="client">The NixSearch client.</param>
/// <param name="logger">The logger.</param>
[McpServerToolType]
public partial class SearchOptionsTool(
    INixSearchClient client,
    ILogger<SearchOptionsTool> logger)
{
    /// <summary>
    /// Searches for NixOS configuration options.
    /// </summary>
    /// <param name="query">Search query (option name, description keywords, etc.).</param>
    /// <param name="channel">NixOS channel to search in (unstable, stable, beta, flakes).</param>
    /// <param name="page">Page number (0-indexed).</param>
    /// <param name="size">Number of results per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Search results with option details including name, type, default value, example, description, and source location.</returns>
    [McpServerTool]
    public async partial Task<SearchResponse<NixOption>> SearchOptions(
        string query,
#pragma warning disable CS1066 // ModelContextProtocol WILL use those default values.
        string? channel = "unstable",
        int? page = 0,
        int? size = 50,
        CancellationToken cancellationToken = default)
#pragma warning restore CS1066
    {
        this.LogSearchingOptions(query, channel, page, size);

        IReadOnlyList<NixChannel> availableChannels = await client.GetChannelsAsync(cancellationToken);
        NixChannel nixChannel = NixChannel.Parse(channel ?? "unstable", availableChannels);

        OptionSearchBuilder builder = client.Options()
            .WithQuery(query)
            .ForChannel(nixChannel)
            .Page(page ?? 0, size ?? 50);

        Nest.ISearchResponse<NixOption> searchResponse = await builder.ExecuteAsync(cancellationToken);

        this.LogSearchCompleted(searchResponse.Total, searchResponse.Documents.Count);

        int currentPage = page ?? 0;
        int currentSize = size ?? 50;
        long total = searchResponse.Total;
        bool hasMore = (currentPage + 1) * currentSize < total;

        return new SearchResponse<NixOption>
        {
            Total = total,
            Page = currentPage,
            Size = currentSize,
            HasMore = hasMore,
            Results = [.. searchResponse.Documents],
        };
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Searching options: query='{Query}', channel='{Channel}', page={Page}, size={Size}")]
    private partial void LogSearchingOptions(string query, string? channel, int? page, int? size);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Search completed: found {Total} options, returning {Count} results")]
    private partial void LogSearchCompleted(long total, int count);
}
