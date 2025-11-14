// SPDX-License-Identifier: MIT

using System;

using Nest;

namespace NixSearch.Core.Models.Flake;

/// <summary>
/// Represents a flake repository.
/// </summary>
public record Repo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Repo"/> class.
    /// </summary>
    public Repo()
    {
    }

    /// <summary>
    /// Gets the type of the repository.
    /// </summary>
    [PropertyName("type")]
    public required RepoType Type { get; init; }

    /// <summary>
    /// Gets the repository URL.
    /// </summary>
    [PropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets the repository owner.
    /// </summary>
    [PropertyName("owner")]
    public string? Owner { get; init; }

    /// <summary>
    /// Gets the repository name.
    /// </summary>
    [PropertyName("repo")]
    public string? RepoName { get; init; }

    /// <summary>
    /// Returns the repository cast to its specific type.
    /// </summary>
    /// <typeparam name="T">The type of the repository.</typeparam>
    /// <returns>>The repository cast to its specific type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the cast fails.</exception>
    /// <exception cref="NotSupportedException">>Thrown when the repository type is not supported.</exception>
    public T WithType<T>()
        where T : Repo
    {
        // Check if already correct type
        if (this is T result)
        {
            return result;
        }

        // Create typed instance based on RepoType
        Repo typedRepo = this.Type switch
        {
            RepoType.Git =>
                new GitRepo()
                {
                    Type = this.Type,
                    Url = this.Url!,
                },
            RepoType.GitHub or RepoType.GitLab or RepoType.SourceHut =>
                new ForgeRepo()
                {
                    Type = this.Type,
                    Owner = this.Owner!,
                    RepoName = this.RepoName!,
                },
            _ => throw new NotSupportedException($"Repository type '{this.Type}' is not supported."),
        };

        // Cast to requested type
        if (typedRepo is not T castResult)
        {
            throw new InvalidOperationException($"Cannot cast repository of type '{this.Type}' to '{typeof(T).Name}'.");
        }

        return castResult;
    }
}