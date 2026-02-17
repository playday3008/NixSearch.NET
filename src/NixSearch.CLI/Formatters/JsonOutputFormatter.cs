// SPDX-License-Identifier: MIT

using System.Text.Json;
using System.Text.Json.Serialization;

using Nest;

namespace NixSearch.CLI.Formatters;

/// <summary>
/// JSON output formatter.
/// </summary>
/// <typeparam name="T">The type of results to format.</typeparam>
public class JsonOutputFormatter<T> : IOutputFormatter<T>
    where T : class
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <inheritdoc/>
    public string Format(ISearchResponse<T> response, bool detailed)
    {
        var result = new
        {
            Total = response.Total,
            Results = response.Documents,
        };

        return JsonSerializer.Serialize(result, Options);
    }
}
