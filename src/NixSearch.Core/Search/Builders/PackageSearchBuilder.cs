// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Options;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Models;

namespace NixSearch.Core.Search.Builders;

/// <summary>
/// Implementation of package search builder.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PackageSearchBuilder"/> class.
/// </remarks>
/// <param name="client">The Elasticsearch client.</param>
/// <param name="options">The NixSearch options.</param>
internal sealed class PackageSearchBuilder(
    IElasticClient client,
    IOptions<NixSearchOptions> options)
    : PackageSearchBuilderBase(
        client,
        options)
{
    private const string TypeValue = "package";
    private const string FilterName = "filter_packages";

    // Aggregation size is taken from official Nixpkgs search frontend
    private new const int AggregationSize = 20;

    /// <inheritdoc/>
    protected override string[] GetMatchFields()
    {
        string attrName = BaseModel.GetPropertyName<NixPackage>(p => p.AttrName);
        string progs = BaseModel.GetPropertyName<NixPackage>(p => p.Programs);
        string name = BaseModel.GetPropertyName<NixPackage>(p => p.Name);
        string desc = BaseModel.GetPropertyName<NixPackage>(p => p.Description);
        string longDesc = BaseModel.GetPropertyName<NixPackage>(p => p.LongDescription);
        string version = BaseModel.GetPropertyName<NixPackage>(p => p.Version);
        string flake = BaseModel.GetPropertyName<NixOption>(o => o.FlakeName);

        // Weights are taken from official Nixpkgs search frontend
        return [
            $"{attrName}^9",
            $"{attrName}.*^5.4",
            $"{progs}^9",
            $"{progs}.*^5.4",
            $"{name}^6",
            $"{name}.*^3.6",
            $"{desc}^1.3",
            $"{desc}.*^0.78",
            $"{longDesc}^1",
            $"{longDesc}.*^0.6",
            $"{flake}^0.5",
            $"{flake}.*^0.3",
        ];
    }

    /// <inheritdoc/>
    protected override SortDescriptor<NixPackage> GetSortDescriptor()
    {
        string attrName = BaseModel.GetPropertyName<NixPackage>(p => p.AttrName);
        string version = BaseModel.GetPropertyName<NixPackage>(p => p.Version);

        return this.Order switch
        {
            null => new SortDescriptor<NixPackage>()
                .Descending(SortSpecialField.Score)
                .Field(f => f
                    .Field(attrName)
                    .Order(SortOrder.Descending))
                .Field(f => f
                    .Field(version)
                    .Order(SortOrder.Descending)),
            _ => new SortDescriptor<NixPackage>()
                .Field(f => f
                    .Field(attrName)
                    .Order(this.Order!))
                .Field(f => f
                    .Field(version)
                    .Order(this.Order!)),
        };
    }

    /// <inheritdoc/>
    protected override AggregationContainerDescriptor<NixPackage> GetAggregations()
    {
        string attrSet = BaseModel.GetPropertyName<NixPackage>(p => p.AttrSet);
        string licenseSet = BaseModel.GetPropertyName<NixPackage>(p => p.LicenseSet);
        string maintainersSet = BaseModel.GetPropertyName<NixPackage>(p => p.MaintainersSet);
        string teamsSet = BaseModel.GetPropertyName<NixPackage>(p => p.TeamsSet);
        string platforms = BaseModel.GetPropertyName<NixPackage>(p => p.Platforms);

        return new AggregationContainerDescriptor<NixPackage>()
                .Terms(attrSet, t => t
                    .Field(attrSet)
                    .Size(AggregationSize))
                .Terms(licenseSet, t => t
                    .Field(licenseSet)
                    .Size(AggregationSize))
                .Terms(maintainersSet, t => t
                    .Field(maintainersSet)
                    .Size(AggregationSize))
                .Terms(teamsSet, t => t
                    .Field(teamsSet)
                    .Size(AggregationSize))
                .Terms(platforms, t => t
                    .Field(platforms)
                    .Size(AggregationSize));
    }

    /// <inheritdoc/>
    protected override SearchDescriptor<NixPackage> GetSearchDescriptor()
    {
        string index = this.GetIndexName();
        string[] matchFields = this.GetMatchFields();
        SortDescriptor<NixPackage> sortDescriptor = this.GetSortDescriptor();

        string attrName = BaseModel.GetPropertyName<NixPackage>(p => p.AttrName);

        string attrSet = BaseModel.GetPropertyName<NixPackage>(p => p.AttrSet);
        string licenseSet = BaseModel.GetPropertyName<NixPackage>(p => p.LicenseSet);
        string maintainersSet = BaseModel.GetPropertyName<NixPackage>(p => p.MaintainersSet);
        string teamsSet = BaseModel.GetPropertyName<NixPackage>(p => p.TeamsSet);
        string platformsSet = BaseModel.GetPropertyName<NixPackage>(p => p.Platforms);

        return new SearchDescriptor<NixPackage>()
            .Index(index)
            .Query(q => q
                .Bool(b => b
                    .Filter(
                        f => f
                            .Term(t => t
                                .Field(TypeField)
                                .Name(FilterName)
                                .Value(TypeValue)),
                        f => f
                            .Bool(bb => bb
                                .Must(
                                    m => m
                                        .Bool(bbb => bbb
                                            .Should(GetShouldQueries(attrSet, this.Attributes))),
                                    m => m
                                        .Bool(bbb => bbb
                                            .Should(GetShouldQueries(licenseSet, this.Licenses))),
                                    m => m
                                        .Bool(bbb => bbb
                                            .Should(GetShouldQueries(maintainersSet, this.Maintainers))),
                                    m => m
                                        .Bool(bbb => bbb
                                            .Should(GetShouldQueries(teamsSet, this.Teams))),
                                    m => m
                                        .Bool(bbb => bbb
                                            .Should(GetShouldQueries(platformsSet, this.Platforms))))))
                    .Must(m => m
                        .DisMax(dm => dm
                            .TieBreaker(0.7)
                            .Queries(
                                qq => qq
                                    .MultiMatch(mm => mm
                                        .Name($"multi_match_{this.Query}")
                                        .Type(TextQueryType.CrossFields)
                                        .Query(this.Query)
                                        .Analyzer("whitespace")
                                        .AutoGenerateSynonymsPhraseQuery(false)
                                        .Operator(Operator.And)
                                        .Fields(fs => fs
                                            .Fields(matchFields))),
                                qq => qq
                                    .Wildcard(wc => wc
                                        .Field(attrName)
                                        .Value($"*{this.Query}*")
                                        .CaseInsensitive(true)))))))
            .Aggregations(aggs => this.GetAggregations())
            .From(this.From)
            .Size(this.Size)
            .Sort(s => sortDescriptor);
    }

    private static QueryContainer[] GetShouldQueries(string name, in List<string> values)
    {
        return [..
            values.Select(value =>
                new QueryContainerDescriptor<NixPackage>()
                    .Term(t => t
                        .Field(name)
                        .Name($"filter_bucket_{name}")
                        .Value(value)))
        ];
    }
}
