// SPDX-License-Identifier: MIT

using System.Collections.Generic;

using Microsoft.Extensions.Options;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Models;

namespace NixSearch.Core.Search.Builders;

/// <summary>
/// Interface for package search builder with fluent API.
/// </summary>
public abstract class PackageSearchBuilderBase(
    IElasticClient client,
    IOptions<NixSearchOptions> options)
    : SearchBuilderBase<NixPackage, PackageSearchBuilderBase>(
        client,
        options)
{
    /// <summary>
    /// The size of aggregations.
    /// </summary>
    protected const int AggregationSize = 100;

    /// <summary>
    /// Gets the attributes to filter by.
    /// </summary>
    protected List<string> Attributes { get; } = [];

    /// <summary>
    /// Gets the licenses to filter by.
    /// </summary>
    protected List<string> Licenses { get; } = [];

    /// <summary>
    /// Gets the maintainers to filter by.
    /// </summary>
    protected List<string> Maintainers { get; } = [];

    /// <summary>
    /// Gets the teams to filter by.
    /// </summary>
    protected List<string> Teams { get; } = [];

    /// <summary>
    /// Gets the platforms to filter by.
    /// </summary>
    protected List<string> Platforms { get; } = [];

    /// <summary>
    /// Filters packages by package set (e.g., "python3Packages", "haskellPackages").
    /// </summary>
    /// <param name="sets">The package sets to filter by (null-safe).</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public PackageSearchBuilderBase WithPackageSet(params string[] sets)
    {
        if (sets is null)
        {
            return this;
        }

        this.Attributes.AddRange(sets);
        return this;
    }

    /// <summary>
    /// Filters packages by license.
    /// </summary>
    /// <param name="licenses">The licenses to filter by (null-safe).</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public PackageSearchBuilderBase WithLicense(params string[] licenses)
    {
        if (licenses is null)
        {
            return this;
        }

        this.Licenses.AddRange(licenses);
        return this;
    }

    /// <summary>
    /// Filters packages by maintainer.
    /// </summary>
    /// <param name="maintainers">The maintainers to filter by (null-safe).</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public PackageSearchBuilderBase WithMaintainer(params string[] maintainers)
    {
        if (maintainers is null)
        {
            return this;
        }

        this.Maintainers.AddRange(maintainers);
        return this;
    }

    /// <summary>
    /// Filters packages by team.
    /// </summary>
    /// <param name="teams">The teams to filter by (null-safe).</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public PackageSearchBuilderBase WithTeam(params string[] teams)
    {
        if (teams is null)
        {
            return this;
        }

        this.Teams.AddRange(teams);
        return this;
    }

    /// <summary>
    /// Filters packages by platform.
    /// </summary>
    /// <param name="platforms">The platforms to filter by (null-safe).</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public PackageSearchBuilderBase WithPlatform(params string[] platforms)
    {
        if (platforms is null)
        {
            return this;
        }

        this.Platforms.AddRange(platforms);
        return this;
    }

    /// <summary>
    /// Get aggregations for package search.
    /// </summary>
    /// <returns>The aggregation descriptor.</returns>
    protected abstract AggregationContainerDescriptor<NixPackage> GetAggregations();
}
