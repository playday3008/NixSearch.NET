// SPDX-License-Identifier: MIT

using Nest;

using NixSearch.Core.Models.Package;

namespace NixSearch.Core.Models;

/// <summary>
/// Represents a Nix package.
/// </summary>
public record NixPackage : NixFlake
{
    /// <summary>
    /// Gets the package attribute name.
    /// </summary>
    [PropertyName("package_attr_name")]
    public required string AttrName { get; init; }

    /// <summary>
    /// Gets the package attribute set.
    /// </summary>
    [PropertyName("package_attr_set")]
    public required string AttrSet { get; init; }

    /// <summary>
    /// Gets the package name.
    /// </summary>
    [PropertyName("package_pname")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the package version.
    /// </summary>
    [PropertyName("package_pversion")]
    public required string Version { get; init; }

    /// <summary>
    /// Gets the supported platforms.
    /// </summary>
    [PropertyName("package_platforms")]
    public required string[] Platforms { get; init; }

    /// <summary>
    /// Gets the package outputs.
    /// </summary>
    [PropertyName("package_outputs")]
    public required string[] Outputs { get; init; }

    /// <summary>
    /// Gets the default output.
    /// </summary>
    [PropertyName("package_default_output")]
    public string? DefaultOutput { get; init; }

    /// <summary>
    /// Gets the programs provided by this package.
    /// </summary>
    [PropertyName("package_programs")]
    public required string[] Programs { get; init; }

    /// <summary>
    /// Gets the main program.
    /// </summary>
    [PropertyName("package_mainProgram")]
    public string? MainProgram { get; init; }

    /// <summary>
    /// Gets the package licenses.
    /// </summary>
    [PropertyName("package_license")]
    public required Package.License[] License { get; init; }

    /// <summary>
    /// Gets the package license set.
    /// </summary>
    [PropertyName("package_license_set")]
    public required string[] LicenseSet { get; init; }

    /// <summary>
    /// Gets the package maintainers.
    /// </summary>
    [PropertyName("package_maintainers")]
    public required Maintainer[] Maintainers { get; init; }

    /// <summary>
    /// Gets the package maintainers set.
    /// </summary>
    [PropertyName("package_maintainers_set")]
    public required string[] MaintainersSet { get; init; }

    /// <summary>
    /// Gets the package teams.
    /// </summary>
    [PropertyName("package_teams")]
    public required Team[] Teams { get; init; }

    /// <summary>
    /// Gets the package teams set.
    /// </summary>
    [PropertyName("package_teams_set")]
    public required string[] TeamsSet { get; init; }

    /// <summary>
    /// Gets the package description.
    /// </summary>
    [PropertyName("package_description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the long description.
    /// </summary>
    [PropertyName("package_longDescription")]
    public string? LongDescription { get; init; }

    /// <summary>
    /// Gets the Hydra build information.
    /// </summary>
    [PropertyName("package_hydra")]
    public Hydra[]? Hydra { get; init; }

    /// <summary>
    /// Gets the system (e.g., "x86_64-linux").
    /// </summary>
    [PropertyName("package_system")]
    public required string System { get; init; }

    /// <summary>
    /// Gets the package homepage URLs.
    /// </summary>
    [PropertyName("package_homepage")]
    public string[]? Homepage { get; init; }

    /// <summary>
    /// Gets the package position in the nixpkgs repository.
    /// </summary>
    [PropertyName("package_position")]
    public string? Position { get; init; }
}
