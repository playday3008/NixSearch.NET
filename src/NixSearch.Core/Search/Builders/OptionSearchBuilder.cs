// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Options;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Models;

namespace NixSearch.Core.Search.Builders;

/// <summary>
/// Implementation of option search builder.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OptionSearchBuilder"/> class.
/// </remarks>
/// <param name="client">The Elasticsearch client.</param>
/// <param name="options">The NixSearch options.</param>
public sealed class OptionSearchBuilder(
    IElasticClient client,
    IOptions<NixSearchOptions> options)
    : SearchBuilderBase<NixOption, OptionSearchBuilder>(
        client,
        options)
{
    private const string TypeValue = "option";
    private const string FilterName = "filter_options";

    /// <inheritdoc/>
    protected override string[] GetMatchFields()
    {
        string name = BaseModel.GetPropertyName<NixOption>(o => o.Name);
        string desc = BaseModel.GetPropertyName<NixOption>(o => o.Description);
        string flake = BaseModel.GetPropertyName<NixOption>(o => o.FlakeName);

        // Weights are taken from official Nixpkgs search frontend
        return [
            $"{name}^6",
            $"{name}.*^3.6",
            $"{desc}^1",
            $"{desc}.*^0.6",
            $"{flake}^0.5",
            $"{flake}.*^0.3",
        ];
    }

    /// <inheritdoc/>
    protected override SortDescriptor<NixOption> GetSortDescriptor()
    {
        string name = BaseModel.GetPropertyName<NixOption>(o => o.Name);

        return this.Order switch
        {
            null => new SortDescriptor<NixOption>()
                .Descending(SortSpecialField.Score)
                .Field(f => f
                    .Field(name)
                    .Order(SortOrder.Descending)),
            _ => new SortDescriptor<NixOption>()
                .Field(name, (SortOrder)this.Order),
        };
    }

    /// <inheritdoc/>
    protected override SearchDescriptor<NixOption> GetSearchDescriptor()
    {
        string index = this.GetIndexName();
        string[] matchFields = this.GetMatchFields();
        SortDescriptor<NixOption> sortDescriptor = this.GetSortDescriptor();

        string name = BaseModel.GetPropertyName<NixOption>(o => o.Name);

        return new SearchDescriptor<NixOption>()
            .Index(index)
            .Query(q => q
                .Bool(b => b
                    .Filter(f => f
                        .Term(t => t
                            .Field(TypeField)
                            .Name(FilterName)
                            .Value(TypeValue)))
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
                                        .Field(name)
                                        .Value($"*{this.Query}*")
                                        .CaseInsensitive(true)))))))
            .From(this.From)
            .Size(this.Size)
            .Sort(s => sortDescriptor);
    }
}