// SPDX-License-Identifier: MIT

using NixSearch.Core.Models;
using NixSearch.Core.Models.Package;

namespace NixSearch.CLI.Tests.TestHelpers;

/// <summary>
/// Factory for creating test data objects.
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Creates a minimal NixPackage for testing.
    /// </summary>
    /// <param name="attrName">The attribute name.</param>
    /// <param name="name">The package name.</param>
    /// <param name="version">The version.</param>
    /// <param name="description">The description.</param>
    /// <returns>A NixPackage instance.</returns>
    public static NixPackage CreatePackage(
        string attrName = "test",
        string name = "test",
        string version = "1.0",
        string? description = null)
    {
        return new NixPackage
        {
            AttrName = attrName,
            AttrSet = "packages",
            Name = name,
            Version = version,
            Description = description,
            Platforms = [],
            Outputs = [],
            Programs = [],
            License = [],
            LicenseSet = [],
            Maintainers = [],
            MaintainersSet = [],
            Teams = [],
            TeamsSet = [],
            System = "x86_64-linux",
        };
    }

    /// <summary>
    /// Creates a detailed NixPackage for testing.
    /// </summary>
    /// <returns>A NixPackage instance with all fields populated.</returns>
    public static NixPackage CreateDetailedPackage()
    {
        return new NixPackage
        {
            AttrName = "vim",
            AttrSet = "packages",
            Name = "vim",
            Version = "9.0",
            Description = "Vi IMproved text editor",
            Platforms = ["x86_64-linux", "aarch64-darwin"],
            Outputs = ["out", "doc"],
            Programs = ["vim", "vi"],
            MainProgram = "vim",
            License = [new NixSearch.Core.Models.Package.License { FullName = "Vim" }],
            LicenseSet = ["vim"],
            Maintainers = [new Maintainer { Name = "John Doe", Email = "john@example.com" }],
            MaintainersSet = ["johndoe"],
            Teams = [new Team { Members = [], GitHubTeams = [], ShortName = "vim-team" }],
            TeamsSet = ["vim-team"],
            Homepage = ["https://www.vim.org/"],
            LongDescription = "Vim is a highly configurable text editor.",
            Position = "pkgs/vim/default.nix:10",
            System = "x86_64-linux",
        };
    }

    /// <summary>
    /// Creates a minimal NixOption for testing.
    /// </summary>
    /// <param name="name">The option name.</param>
    /// <param name="description">The description.</param>
    /// <returns>A NixOption instance.</returns>
    public static NixOption CreateOption(
        string name = "services.test.enable",
        string? description = null)
    {
        return new NixOption
        {
            Name = name,
            Description = description,
        };
    }

    /// <summary>
    /// Creates a detailed NixOption for testing.
    /// </summary>
    /// <returns>A NixOption instance with all fields populated.</returns>
    public static NixOption CreateDetailedOption()
    {
        return new NixOption
        {
            Name = "services.nginx.enable",
            Description = "Whether to enable nginx.",
            Type = "boolean",
            Default = "false",
            Example = "true",
            Source = "nixos/modules/services/web-servers/nginx/default.nix",
        };
    }
}
