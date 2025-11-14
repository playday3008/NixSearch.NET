// SPDX-License-Identifier: MIT

using Nest;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NixSearch.CLI.Formatters;

/// <summary>
/// YAML output formatter.
/// </summary>
/// <typeparam name="T">The type of results to format.</typeparam>
public class YamlOutputFormatter<T> : IOutputFormatter<T>
    where T : class
{
    private static readonly ISerializer Serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
        .Build();

    /// <inheritdoc/>
    public string Format(ISearchResponse<T> response, bool detailed)
    {
        var result = new
        {
            Total = response.Total,
            Results = response.Documents,
        };

        return Serializer.Serialize(result);
    }
}