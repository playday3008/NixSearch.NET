// SPDX-License-Identifier: MIT

using Nest;

namespace NixSearch.CLI.Formatters;

/// <summary>
/// Interface for output formatters.
/// </summary>
/// <typeparam name="T">The type of results to format.</typeparam>
public interface IOutputFormatter<T>
    where T : class
{
    /// <summary>
    /// Formats the search results.
    /// </summary>
    /// <param name="response">The search response.</param>
    /// <param name="detailed">Whether to show detailed output.</param>
    /// <returns>The formatted string.</returns>
    string Format(ISearchResponse<T> response, bool detailed);
}