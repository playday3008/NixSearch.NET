// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nest;

using NixSearch.CLI.Extensions;
using NixSearch.Core.Models;

namespace NixSearch.CLI.Formatters;

/// <summary>
/// Text output formatter.
/// </summary>
/// <typeparam name="T">The type of results to format.</typeparam>
public class TextOutputFormatter<T> : IOutputFormatter<T>
    where T : class
{
    /// <inheritdoc/>
    public string Format(ISearchResponse<T> response, bool detailed)
    {
        StringBuilder sb = new();

        sb.AppendInvariantLine($"Found {response.Total} results");
        sb.AppendLine();

        foreach (T doc in response.Documents)
        {
            if (doc is NixPackage package)
            {
                TextOutputFormatter<T>.FormatPackage(sb, package, detailed);
            }
            else if (doc is NixOption option)
            {
                TextOutputFormatter<T>.FormatOption(sb, option, detailed);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static void FormatPackage(StringBuilder sb, NixPackage package, bool detailed)
    {
        sb.AppendInvariantLine($"Package: {package.AttrName}");
        sb.AppendInvariantLine($"  Name: {package.Name}");
        sb.AppendInvariantLine($"  Version: {package.Version}");

        if (!string.IsNullOrEmpty(package.Description))
        {
            sb.AppendInvariantLine($"  Description: {package.Description}");
        }

        if (detailed)
        {
            if (package.Platforms?.Length > 0)
            {
                sb.AppendInvariantLine($"  Platforms: {string.Join(", ", package.Platforms)}");
            }

            if (package.Programs?.Length > 0)
            {
                sb.AppendInvariantLine($"  Programs: {string.Join(", ", package.Programs)}");
            }

            if (!string.IsNullOrEmpty(package.MainProgram))
            {
                sb.AppendInvariantLine($"  Main Program: {package.MainProgram}");
            }

            if (package.License?.Length > 0)
            {
                IEnumerable<string?> licenses = package.License.Select(l => l.FullName).Where(l => l != null);
                sb.AppendInvariantLine($"  License: {string.Join(", ", licenses)}");
            }

            if (package.Maintainers?.Length > 0)
            {
                IEnumerable<string?> maintainers = package.Maintainers.Select(m => m.Name ?? m.Email).Where(m => m != null);
                if (maintainers.Any())
                {
                    sb.AppendInvariantLine($"  Maintainers: {string.Join(", ", maintainers)}");
                }
            }

            if (package.Homepage?.Length > 0)
            {
                sb.AppendInvariantLine($"  Homepage: {string.Join(", ", package.Homepage)}");
            }

            if (!string.IsNullOrEmpty(package.LongDescription))
            {
                sb.AppendInvariantLine($"  Long Description: {package.LongDescription}");
            }

            if (!string.IsNullOrEmpty(package.Position))
            {
                sb.AppendInvariantLine($"  Position: {package.Position}");
            }

            if (!string.IsNullOrEmpty(package.AttrSet))
            {
                sb.AppendInvariantLine($"  Attribute Set: {package.AttrSet}");
            }

            if (!string.IsNullOrEmpty(package.System))
            {
                sb.AppendInvariantLine($"  System: {package.System}");
            }
        }
    }

    private static void FormatOption(StringBuilder sb, NixOption option, bool detailed)
    {
        sb.AppendInvariantLine($"Option: {option.Name}");

        if (!string.IsNullOrEmpty(option.Description))
        {
            sb.AppendInvariantLine($"  Description: {option.Description}");
        }

        if (detailed)
        {
            if (!string.IsNullOrEmpty(option.Type))
            {
                sb.AppendInvariantLine($"  Type: {option.Type}");
            }

            if (!string.IsNullOrEmpty(option.Default))
            {
                sb.AppendInvariantLine($"  Default: {option.Default}");
            }

            if (!string.IsNullOrEmpty(option.Example))
            {
                sb.AppendInvariantLine($"  Example: {option.Example}");
            }

            if (!string.IsNullOrEmpty(option.Source))
            {
                sb.AppendInvariantLine($"  Source: {option.Source}");
            }

            if (option.Flake != null)
            {
                var flakeValue = option.Flake.Match(
                    str => str,
                    arr => arr?.Length > 0 ? string.Join(", ", arr) : null);

                if (!string.IsNullOrEmpty(flakeValue))
                {
                    sb.AppendInvariantLine($"  Flake: {flakeValue}");
                }
            }
        }
    }
}
