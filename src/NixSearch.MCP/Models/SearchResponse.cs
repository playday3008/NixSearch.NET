// SPDX-License-Identifier: MIT

using System.Collections.Generic;

namespace NixSearch.MCP.Models;

/// <summary>
/// Represents a search response with metadata and results.
/// </summary>
/// <typeparam name="T">The type of result items.</typeparam>
public record SearchResponse<T>
{
    /// <summary>
    /// Gets the total number of results available.
    /// </summary>
    public required long Total { get; init; }

    /// <summary>
    /// Gets the current page number (0-indexed).
    /// </summary>
    public required int Page { get; init; }

    /// <summary>
    /// Gets the number of results per page.
    /// </summary>
    public required int Size { get; init; }

    /// <summary>
    /// Gets a value indicating whether there are more results available.
    /// </summary>
    public required bool HasMore { get; init; }

    /// <summary>
    /// Gets the list of results.
    /// </summary>
    public required IReadOnlyList<T> Results { get; init; }

    /// <summary>
    /// Gets the list of warnings, if any.
    /// </summary>
    public IReadOnlyList<Warning>? Warnings { get; init; }
}
