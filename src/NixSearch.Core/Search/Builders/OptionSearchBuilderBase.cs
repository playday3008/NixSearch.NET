// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Options;

using Nest;

using NixSearch.Core.Configuration;
using NixSearch.Core.Models;

namespace NixSearch.Core.Search.Builders;

/// <summary>
/// Interface for option search builder with fluent API.
/// </summary>
public abstract class OptionSearchBuilderBase(
    IElasticClient client,
    IOptions<NixSearchOptions> options)
    : SearchBuilderBase<NixOption, OptionSearchBuilderBase>(
        client,
        options)
{
}