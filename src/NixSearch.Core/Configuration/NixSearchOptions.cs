// SPDX-License-Identifier: MIT

using System;

namespace NixSearch.Core.Configuration;

/// <summary>
/// Configuration options for the NixSearch client.
/// </summary>
public record NixSearchOptions
{
    /// <summary>
    /// Gets or sets the Elasticsearch backend URL.
    /// </summary>
    public string Url { get; set; } = "https://search.nixos.org/backend";

    /// <summary>
    /// Gets or sets the username for basic authentication.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the password for basic authentication.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Gets or sets the Elasticsearch mapping schema version.
    /// </summary>
    public int MappingSchemaVersion { get; set; } = 44;

    /// <summary>
    /// Gets or sets the request timeout.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets a value indicating whether to enable debugging.
    /// </summary>
    public bool EnableDebugMode { get; set; }
}