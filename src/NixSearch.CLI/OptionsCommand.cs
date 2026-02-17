// SPDX-License-Identifier: MIT

using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

using Nest;

using NixSearch.Core.Models;
using NixSearch.Core.Search;
using NixSearch.Core.Search.Builders;

namespace NixSearch.CLI;

/// <summary>
/// Command for searching NixOS options.
/// </summary>
public class OptionsCommand : BaseSearchCommand<NixOption>
{
    /// <summary>
    /// Creates the options command.
    /// </summary>
    /// <returns>The options command.</returns>
    public static Command Create()
    {
        return new OptionsCommand().CreateCommand("options", "Search for NixOS options");
    }

    /// <inheritdoc/>
    protected override async Task<ISearchResponse<NixOption>> ExecuteSearchAsync(
        ParseResult parseResult,
        INixSearchClient client,
        string query,
        NixChannel channel,
        int from,
        int size,
        SortOrder? sortOrder,
        CancellationToken cancellationToken)
    {
        OptionSearchBuilder builder = client.Options()
            .WithQuery(query)
            .ForChannel(channel)
            .Page(from, size)
            .SortBy(sortOrder);

        return await builder.ExecuteAsync(cancellationToken);
    }
}
