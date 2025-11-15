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
/// MCP tool for getting option details.
/// </summary>
/// <param name="client">The NixSearch client.</param>
/// <param name="logger">The logger.</param>
[McpServerToolType]
public partial class GetOptionDetailsTool(
    INixSearchClient client,
    ILogger<GetOptionDetailsTool> logger)
{
    /// <summary>
    /// Gets comprehensive details about a specific NixOS configuration option.
    /// </summary>
    /// <param name="optionName">Full option name (e.g., 'services.nginx.enable').</param>
    /// <param name="channel">NixOS channel (unstable, stable, flakes).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Option details, or null if not found.</returns>
    [McpServerTool]
    [Description("Get comprehensive details about a specific NixOS configuration option.")]
    public async Task<NixOption?> GetOptionDetails(
        string optionName,
        string? channel = "unstable",
        CancellationToken cancellationToken = default)
    {
        this.LogGettingOptionDetails(optionName, channel);

        NixChannel nixChannel = ChannelParser.Parse(channel ?? "unstable");

        OptionSearchBuilderBase builder = client.Options()
            .WithQuery(optionName)
            .ForChannel(nixChannel)
            .Page(0, 10);

        Nest.ISearchResponse<NixOption> searchResponse = await builder.ExecuteAsync(cancellationToken);

        // Find exact match by option name
        NixOption? option = searchResponse.Documents
            .FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));

        if (option == null)
        {
            this.LogOptionNotFound(optionName, channel);
            return null;
        }

        this.LogOptionFound(optionName);

        return option;
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Getting option details: optionName='{OptionName}', channel='{Channel}'")]
    private partial void LogGettingOptionDetails(string optionName, string? channel);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Option not found: optionName='{OptionName}', channel='{Channel}'")]
    private partial void LogOptionNotFound(string optionName, string? channel);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Option found: {OptionName}")]
    private partial void LogOptionFound(string optionName);
}