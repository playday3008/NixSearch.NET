// SPDX-License-Identifier: MIT

namespace NixSearch.MCP.Models;

/// <summary>
/// Represents a simplified package result for MCP responses.
/// </summary>
public record PackageResult
{
    /// <summary>
    /// Gets the package attribute name.
    /// </summary>
    public required string AttrName { get; init; }

    /// <summary>
    /// Gets the package attribute set.
    /// </summary>
    public required string AttrSet { get; init; }

    /// <summary>
    /// Gets the package name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the package version.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Gets the package description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the long description.
    /// </summary>
    public string? LongDescription { get; init; }

    /// <summary>
    /// Gets the supported platforms.
    /// </summary>
    public required string[] Platforms { get; init; }

    /// <summary>
    /// Gets the license names.
    /// </summary>
    public required string[] LicenseNames { get; init; }

    /// <summary>
    /// Gets the maintainer names.
    /// </summary>
    public required string[] MaintainerNames { get; init; }

    /// <summary>
    /// Gets the team names.
    /// </summary>
    public required string[] TeamNames { get; init; }

    /// <summary>
    /// Gets the package homepage URLs.
    /// </summary>
    public string[]? Homepage { get; init; }

    /// <summary>
    /// Gets the programs provided by this package.
    /// </summary>
    public required string[] Programs { get; init; }

    /// <summary>
    /// Gets the main program.
    /// </summary>
    public string? MainProgram { get; init; }

    /// <summary>
    /// Gets the system (e.g., "x86_64-linux").
    /// </summary>
    public required string System { get; init; }

    /// <summary>
    /// Gets the package position in the nixpkgs repository.
    /// </summary>
    public string? Position { get; init; }

    /// <summary>
    /// Gets the flake name, if applicable.
    /// </summary>
    public string? FlakeName { get; init; }

    /// <summary>
    /// Gets the flake description, if applicable.
    /// </summary>
    public string? FlakeDescription { get; init; }
}