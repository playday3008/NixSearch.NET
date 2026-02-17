// SPDX-License-Identifier: MIT

using System.Text.Json.Serialization;
using System.Xml.Serialization;

using Nest;

using YamlDotNet.Serialization;

namespace NixSearch.Core.Models;

/// <summary>
/// Represents a NixOS configuration option.
/// </summary>
public record NixOption : NixFlake
{
    /// <summary>
    /// Gets the option name.
    /// </summary>
    [PropertyName("option_name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the option description.
    /// </summary>
    [PropertyName("option_description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the option type.
    /// </summary>
    [PropertyName("option_type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    [PropertyName("option_default")]
    public string? Default { get; init; }

    /// <summary>
    /// Gets an example value.
    /// </summary>
    [PropertyName("option_example")]
    public string? Example { get; init; }

    /// <summary>
    /// Gets the source file location.
    /// </summary>
    [PropertyName("option_source")]
    public string? Source { get; init; }

    /// <summary>
    /// Gets the flake information (flake name and module path).
    /// </summary>
    /// <remarks>
    /// This property is ignored during serialization.
    /// </remarks>
    [PropertyName("option_flake")]
    [XmlIgnore]
    [YamlIgnore]
    [JsonIgnore]
    public Union<string?, string[]>? Flake { get; init; }
}
