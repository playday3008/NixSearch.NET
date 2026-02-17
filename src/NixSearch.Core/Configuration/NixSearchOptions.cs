// SPDX-License-Identifier: MIT

using System;

namespace NixSearch.Core.Configuration;

/// <summary>
/// Configuration options for the NixSearch client.
/// </summary>
public record NixSearchOptions
{
    /// <summary>
    /// Gets the Elasticsearch backend URL.
    /// </summary>
    public string Url { get; init; } = "https://search.nixos.org/backend";

    /// <summary>
    /// Gets the username for basic authentication.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Gets the password for basic authentication.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Gets the Elasticsearch mapping schema version.
    /// </summary>
    public int MappingSchemaVersion { get; init; } = 44;

    /// <summary>
    /// Gets the request timeout.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets the maximum number of retries for failed requests.
    /// </summary>
    public int MaxRetries { get; init; } = 5;

    /// <summary>
    /// Gets the maximum total time to spend on retries.
    /// </summary>
    public TimeSpan MaxRetryTimeout { get; init; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Gets a value indicating whether to enable debugging.
    /// </summary>
    public bool EnableDebugMode { get; init; }
}
