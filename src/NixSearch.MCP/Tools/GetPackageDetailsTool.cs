// SPDX-License-Identifier: MIT

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ModelContextProtocol.Server;

using NixSearch.Core.Models;
using NixSearch.Core.Search;
using NixSearch.Core.Search.Builders;
using NixSearch.MCP.Helpers;

namespace NixSearch.MCP.Tools;

/// <summary>
/// MCP tool for getting package details.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GetPackageDetailsTool"/> class.
/// </remarks>
/// <param name="client">The NixSearch client.</param>
/// <param name="logger">The logger.</param>
[McpServerToolType]
public partial class GetPackageDetailsTool(
    INixSearchClient client,
    ILogger<GetPackageDetailsTool> logger)
{
    /// <summary>
    /// Gets comprehensive details about a specific NixOS package.
    /// </summary>
    /// <param name="attrName">Package attribute name (e.g., 'firefox', 'python3Packages.numpy').</param>
    /// <param name="channel">NixOS channel (unstable, stable, flakes).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Package details, or null if not found.</returns>
    [McpServerTool]
    [Description("Get comprehensive details about a specific NixOS package by its attribute name.")]
    public async Task<NixPackage?> GetPackageDetails(
        [Description("Package attribute name (e.g., 'firefox', 'python3Packages.numpy')")]
        string attrName,
        [Description("NixOS channel (unstable, stable, flakes)")]
        string? channel = "unstable",
        CancellationToken cancellationToken = default)
    {
        this.LogGettingPackageDetails(attrName, channel);

        NixChannel nixChannel = ChannelParser.Parse(channel ?? "unstable");

        PackageSearchBuilderBase builder = client.Packages()
            .WithQuery(attrName)
            .ForChannel(nixChannel)
            .Page(0, 10);

        Nest.ISearchResponse<NixPackage> searchResponse = await builder.ExecuteAsync(cancellationToken);

        // Find exact match by attribute name
        NixPackage? package = searchResponse.Documents
            .FirstOrDefault(p => p.AttrName.Equals(attrName, StringComparison.OrdinalIgnoreCase));

        if (package == null)
        {
            this.LogPackageNotFound(attrName, channel);
            return null;
        }

        this.LogPackageFound(package.AttrName, package.Version);

        return package;
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Getting package details: attrName='{AttrName}', channel='{Channel}'")]
    private partial void LogGettingPackageDetails(string attrName, string? channel);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Package not found: attrName='{AttrName}', channel='{Channel}'")]
    private partial void LogPackageNotFound(string attrName, string? channel);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Package found: {AttrName} v{Version}")]
    private partial void LogPackageFound(string attrName, string version);
}