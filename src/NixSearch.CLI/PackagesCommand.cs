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
/// Command for searching NixOS packages.
/// </summary>
public class PackagesCommand : BaseSearchCommand<NixPackage>
{
    private readonly Option<string[]?> platformOption;
    private readonly Option<string[]?> packageSetOption;
    private readonly Option<string[]?> licenseOption;
    private readonly Option<string[]?> maintainerOption;
    private readonly Option<string[]?> teamOption;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackagesCommand"/> class.
    /// </summary>
    public PackagesCommand()
    {
        this.platformOption = new("--platform")
        {
            Description = "Filter by platform (e.g., x86_64-linux)",
            AllowMultipleArgumentsPerToken = true,
        };

        this.packageSetOption = new("--package-set")
        {
            Description = "Filter by package set (e.g., python3Packages)",
            AllowMultipleArgumentsPerToken = true,
        };

        this.licenseOption = new("--license")
        {
            Description = "Filter by license",
            AllowMultipleArgumentsPerToken = true,
        };

        this.maintainerOption = new("--maintainer")
        {
            Description = "Filter by maintainer",
            AllowMultipleArgumentsPerToken = true,
        };

        this.teamOption = new("--team")
        {
            Description = "Filter by team",
            AllowMultipleArgumentsPerToken = true,
        };
    }

    /// <summary>
    /// Creates the packages command.
    /// </summary>
    /// <returns>The packages command.</returns>
    public static Command Create()
    {
        return new PackagesCommand().CreateCommand("packages", "Search for NixOS packages");
    }

    /// <inheritdoc/>
    protected override void AddSpecificOptions(Command command)
    {
        command.Add(this.platformOption);
        command.Add(this.packageSetOption);
        command.Add(this.licenseOption);
        command.Add(this.maintainerOption);
        command.Add(this.teamOption);
    }

    /// <inheritdoc/>
    protected override async Task<ISearchResponse<NixPackage>> ExecuteSearchAsync(
        ParseResult parseResult,
        INixSearchClient client,
        string query,
        NixChannel channel,
        int from,
        int size,
        SortOrder? sortOrder,
        CancellationToken cancellationToken)
    {
        PackageSearchBuilderBase builder = client.Packages()
            .WithQuery(query)
            .ForChannel(channel)
            .Page(from, size)
            .SortBy(sortOrder);

        string[]? platforms = parseResult.GetValue(this.platformOption);
        if (platforms?.Length > 0)
        {
            builder.WithPlatform(platforms);
        }

        string[]? packageSets = parseResult.GetValue(this.packageSetOption);
        if (packageSets?.Length > 0)
        {
            builder.WithPackageSet(packageSets);
        }

        string[]? licenses = parseResult.GetValue(this.licenseOption);
        if (licenses?.Length > 0)
        {
            builder.WithLicense(licenses);
        }

        string[]? maintainers = parseResult.GetValue(this.maintainerOption);
        if (maintainers?.Length > 0)
        {
            builder.WithMaintainer(maintainers);
        }

        string[]? teams = parseResult.GetValue(this.teamOption);
        if (teams?.Length > 0)
        {
            builder.WithTeam(teams);
        }

        return await builder.ExecuteAsync(cancellationToken);
    }
}
