// SPDX-License-Identifier: MIT

namespace NixSearch.CLI.Formatters;

/// <summary>
/// Represents the output format for the CLI.
/// </summary>
public enum OutputFormat
{
    /// <summary>
    /// Human-readable text format.
    /// </summary>
    Text,

    /// <summary>
    /// JSON format.
    /// </summary>
    Json,

    /// <summary>
    /// YAML format.
    /// </summary>
    Yaml,

    /// <summary>
    /// XML format.
    /// </summary>
    Xml,
}
