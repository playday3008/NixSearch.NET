// SPDX-License-Identifier: MIT

namespace NixSearch.MCP.Models;

/// <summary>
/// Represents a warning in the response.
/// </summary>
public record Warning
{
    /// <summary>
    /// Gets the warning code.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets the warning message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Gets the parameter that caused the warning, if applicable.
    /// </summary>
    public string? Parameter { get; init; }
}
