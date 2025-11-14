// SPDX-License-Identifier: MIT

namespace NixSearch.MCP.Models;

/// <summary>
/// Represents a simplified option result for MCP responses.
/// </summary>
public record OptionResult
{
    /// <summary>
    /// Gets the option name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the option description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the option type.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    public string? Default { get; init; }

    /// <summary>
    /// Gets an example value.
    /// </summary>
    public string? Example { get; init; }

    /// <summary>
    /// Gets the source file location.
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// Gets the flake name, if applicable.
    /// </summary>
    public string? FlakeName { get; init; }

    /// <summary>
    /// Gets the flake description, if applicable.
    /// </summary>
    public string? FlakeDescription { get; init; }
}