// SPDX-License-Identifier: MIT

using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ModelContextProtocol.Server;

using NixSearch.Core.Models;
using NixSearch.Core.Search;
using NixSearch.Core.Search.Builders;
using NixSearch.MCP.Helpers;
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
    /// <param name="channel">NixOS channel to search in (unstable, stable, flakes).</param>
    /// <param name="page">Page number (0-indexed).</param>
    /// <param name="size">Number of results per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Search results with option details.</returns>
    [McpServerTool]
    [Description("Search for NixOS configuration options. Returns option details including name, type, default value, example, description, and source location.")]
    public async Task<SearchResponse<NixOption>> SearchOptions(
        [Description("Search query (option name, description keywords, etc.)")]
        string query,
        [Description("NixOS channel to search in (unstable, stable, flakes)")]
        string? channel = "unstable",
        [Description("Page number (0-indexed)")]
        int? page = 0,
        [Description("Number of results per page")]
        int? size = 50,
        CancellationToken cancellationToken = default)
    {
        this.LogSearchingOptions(query, channel, page, size);

        NixChannel nixChannel = ChannelParser.Parse(channel ?? "unstable");

        OptionSearchBuilderBase builder = client.Options()
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