// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

using Nest;

using NixSearch.CLI.Formatters;
using NixSearch.Core.Search;

namespace NixSearch.CLI;

/// <summary>
/// Base class for search commands with common options and logic.
/// </summary>
/// <typeparam name="T">The type of search result.</typeparam>
public abstract class BaseSearchCommand<T>
    where T : class
{
    /// <summary>
    /// Parses the channel string into a NixChannel using available channels from discovery.
    /// </summary>
    /// <param name="channel">The channel string to parse.</param>
    /// <param name="availableChannels">The available channels from discovery.</param>
    /// <returns>The parsed NixChannel.</returns>
    internal static NixChannel ParseChannel(string channel, IReadOnlyList<NixChannel> availableChannels)
    {
        return NixChannel.Parse(channel, availableChannels);
    }

    /// <summary>
    /// Parses the sort string into a SortOrder enum.
    /// </summary>
    /// <param name="sort">The sort string to parse.</param>
    /// <returns>The parsed SortOrder, or null for relevance sorting.</returns>
    internal static SortOrder? ParseSortOrder(string? sort)
    {
        return sort?.ToLowerInvariant() switch
        {
            "asc" => SortOrder.Ascending,
            "desc" => SortOrder.Descending,
            null => null,
            _ => throw new ArgumentException($"Invalid sort order: {sort}. Valid values are: asc, desc, or omit for relevance sorting"),
        };
    }

    /// <summary>
    /// Creates the appropriate formatter based on the output format.
    /// </summary>
    /// <param name="format">The output format.</param>
    /// <returns>The formatter instance.</returns>
    internal static IOutputFormatter<T> CreateFormatter(OutputFormat format)
    {
        return format switch
        {
            OutputFormat.Json => new JsonOutputFormatter<T>(),
            OutputFormat.Yaml => new YamlOutputFormatter<T>(),
            OutputFormat.Xml => new XmlOutputFormatter<T>(),
            _ => new TextOutputFormatter<T>(),
        };
    }

    /// <summary>
    /// Creates the command with common and specific options.
    /// </summary>
    /// <param name="name">The command name.</param>
    /// <param name="description">The command description.</param>
    /// <returns>The configured command.</returns>
    protected Command CreateCommand(string name, string description)
    {
        // Required argument
        Argument<string> queryArgument = new("query")
        {
            Description = "The search query",
        };

        // Common options
        Option<string> channelOption = new("--channel")
        {
            Description = "The NixOS channel to search (unstable, stable, beta, flakes)",
            DefaultValueFactory = _ => "unstable",
        };

        Option<int> fromOption = new("--from")
        {
            Description = "Starting offset for pagination (0-based)",
            DefaultValueFactory = _ => 0,
        };

        Option<int> sizeOption = new("--size")
        {
            Description = "Number of results to return",
            DefaultValueFactory = _ => 50,
        };

        Option<string?> sortOption = new("--sort")
        {
            Description = "Sort order (asc or desc, null for relevance)",
        };

        Option<OutputFormat> formatOption = new("--format")
        {
            Description = "Output format (text, json, yaml, xml)",
            DefaultValueFactory = _ => OutputFormat.Text,
        };

        Option<bool> detailedOption = new("--detailed")
        {
            Description = "Show detailed output (for text format)",
            DefaultValueFactory = _ => false,
        };

        Command command = new(name, description)
        {
            queryArgument,
            channelOption,
            fromOption,
            sizeOption,
            sortOption,
            formatOption,
            detailedOption,
        };

        // Add command-specific options
        this.AddSpecificOptions(command);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var query = parseResult.GetValue(queryArgument);
            var channel = parseResult.GetValue(channelOption);
            var from = parseResult.GetValue(fromOption);
            var size = parseResult.GetValue(sizeOption);
            var sort = parseResult.GetValue(sortOption);
            var format = parseResult.GetValue(formatOption);
            var detailed = parseResult.GetValue(detailedOption);

            await this.ExecuteAsync(parseResult, query!, channel!, from, size, sort, format, detailed, cancellationToken);
        });

        return command;
    }

    /// <summary>
    /// Allows derived classes to add command-specific options.
    /// </summary>
    /// <param name="command">The command to add options to.</param>
    protected virtual void AddSpecificOptions(Command command)
    {
    }

    /// <summary>
    /// Executes the specific search logic asynchronously.
    /// </summary>
    /// <param name="parseResult">The parse result for accessing command-specific options.</param>
    /// <param name="client">The NixSearch client.</param>
    /// <param name="query">The search query.</param>
    /// <param name="channel">The NixOS channel.</param>
    /// <param name="from">The starting offset.</param>
    /// <param name="size">The number of results.</param>
    /// <param name="sortOrder">The sort order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The search response.</returns>
    protected abstract Task<ISearchResponse<T>> ExecuteSearchAsync(
        ParseResult parseResult,
        INixSearchClient client,
        string query,
        NixChannel channel,
        int from,
        int size,
        SortOrder? sortOrder,
        CancellationToken cancellationToken);

    /// <summary>
    /// Executes the search command asynchronously.
    /// </summary>
    private async Task ExecuteAsync(
        ParseResult parseResult,
        string query,
        string channel,
        int from,
        int size,
        string? sort,
        OutputFormat format,
        bool detailed,
        CancellationToken cancellationToken)
    {
        try
        {
            INixSearchClient client = NixSearchClientFactory.Create();
            IReadOnlyList<NixChannel> availableChannels = await client.GetChannelsAsync(cancellationToken);

            NixChannel nixChannel = ParseChannel(channel, availableChannels);
            SortOrder? sortOrder = ParseSortOrder(sort);

            ISearchResponse<T> response = await this.ExecuteSearchAsync(parseResult, client, query, nixChannel, from, size, sortOrder, cancellationToken);

            IOutputFormatter<T> formatter = CreateFormatter(format);
            string output = formatter.Format(response, detailed);
            Console.WriteLine(output);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
