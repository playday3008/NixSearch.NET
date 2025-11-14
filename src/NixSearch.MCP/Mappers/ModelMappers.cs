// SPDX-License-Identifier: MIT

using NixSearch.Core.Models;
using NixSearch.MCP.Models;

namespace NixSearch.MCP.Mappers;

/// <summary>
/// Extension methods for mapping Core models to MCP response models.
/// </summary>
public static class ModelMappers
{
    /// <summary>
    /// Maps a NixPackage to a PackageResult.
    /// </summary>
    /// <param name="package">The package to map.</param>
    /// <returns>A simplified PackageResult.</returns>
    public static PackageResult ToPackageResult(this NixPackage package)
    {
        return new PackageResult
        {
            AttrName = package.AttrName,
            AttrSet = package.AttrSet,
            Name = package.Name,
            Version = package.Version,
            Description = package.Description,
            LongDescription = package.LongDescription,
            Platforms = package.Platforms,
            LicenseNames = package.LicenseSet,
            MaintainerNames = package.MaintainersSet,
            TeamNames = package.TeamsSet,
            Homepage = package.Homepage,
            Programs = package.Programs,
            MainProgram = package.MainProgram,
            System = package.System,
            Position = package.Position,
            FlakeName = package.FlakeName,
            FlakeDescription = package.FlakeDescription,
        };
    }

    /// <summary>
    /// Maps a NixOption to an OptionResult.
    /// </summary>
    /// <param name="option">The option to map.</param>
    /// <returns>A simplified OptionResult.</returns>
    public static OptionResult ToOptionResult(this NixOption option)
    {
        return new OptionResult
        {
            Name = option.Name,
            Description = option.Description,
            Type = option.Type,
            Default = option.Default,
            Example = option.Example,
            Source = option.Source,
            FlakeName = option.FlakeName,
            FlakeDescription = option.FlakeDescription,
        };
    }
}