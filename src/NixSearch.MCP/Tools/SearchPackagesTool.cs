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
/// MCP tool for searching NixOS packages.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SearchPackagesTool"/> class.
/// </remarks>
/// <param name="client">The NixSearch client.</param>
/// <param name="logger">The logger.</param>
[McpServerToolType]
public partial class SearchPackagesTool(
    INixSearchClient client,
    ILogger<SearchPackagesTool> logger)
{
    /// <summary>
    /// Searches for NixOS packages in nixpkgs.
    /// </summary>
    /// <param name="query">Search query (package name, description keywords, etc.).</param>
    /// <param name="channel">NixOS channel to search in (unstable, stable, flakes).</param>
    /// <param name="platform">Filter by platforms (e.g., x86_64-linux, aarch64-darwin).</param>
    /// <param name="packageSet">Filter by package sets (e.g., python3Packages, haskellPackages).</param>
    /// <param name="license">Filter by license names.</param>
    /// <param name="maintainer">Filter by maintainer usernames.</param>
    /// <param name="team">Filter by team names.</param>
    /// <param name="page">Page number (0-indexed).</param>
    /// <param name="size">Number of results per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Search results with package details.</returns>
    [McpServerTool]
    [Description("Search for NixOS packages in nixpkgs. Returns package details including name, version, description, maintainers, licenses, platforms, and more.")]
    public async Task<SearchResponse<NixPackage>> SearchPackages(
        string query,
        string? channel = "unstable",
        string[]? platform = null,
        string[]? packageSet = null,
        string[]? license = null,
        string[]? maintainer = null,
        string[]? team = null,
        int? page = 0,
        int? size = 50,
        CancellationToken cancellationToken = default)
    {
        this.LogSearchingPackages(query, channel, page, size);

        NixChannel nixChannel = ChannelParser.Parse(channel ?? "unstable");

        PackageSearchBuilderBase builder = client.Packages()
            .WithQuery(query)
            .ForChannel(nixChannel)
            .Page(page ?? 0, size ?? 50);

        if (platform?.Length > 0)
        {
            this.LogFilteringBy("platforms", string.Join(", ", platform));
            builder.WithPlatform(platform);
        }

        if (packageSet?.Length > 0)
        {
            this.LogFilteringBy("package", string.Join(", ", packageSet));
            builder.WithPackageSet(packageSet);
        }

        if (license?.Length > 0)
        {
            this.LogFilteringBy("licenses", string.Join(", ", license));
            builder.WithLicense(license);
        }

        if (maintainer?.Length > 0)
        {
            this.LogFilteringBy("maintainers", string.Join(", ", maintainer));
            builder.WithMaintainer(maintainer);
        }

        if (team?.Length > 0)
        {
            this.LogFilteringBy("teams", string.Join(", ", team));
            builder.WithTeam(team);
        }

        Nest.ISearchResponse<NixPackage> searchResponse = await builder.ExecuteAsync(cancellationToken);

        this.LogSearchCompleted(searchResponse.Total, searchResponse.Documents.Count);

        int currentPage = page ?? 0;
        int currentSize = size ?? 50;
        long total = searchResponse.Total;
        bool hasMore = (currentPage + 1) * currentSize < total;

        return new SearchResponse<NixPackage>
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
        Message = "Searching packages: query='{Query}', channel='{Channel}', page={Page}, size={Size}")]
    private partial void LogSearchingPackages(string query, string? channel, int? page, int? size);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Search completed: found {Total} packages, returning {Count} results")]
    private partial void LogSearchCompleted(long total, int count);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Filtering by {what}: {values}")]
    private partial void LogFilteringBy(string what, string values);
}