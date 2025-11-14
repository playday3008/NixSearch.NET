// SPDX-License-Identifier: MIT

using Nest;

namespace NixSearch.Core.Models.Package;

/// <summary>
/// Represents Hydra build information for a package.
/// </summary>
public sealed record Hydra
{
    /// <summary>
    /// Gets the build ID.
    /// </summary>
    [PropertyName("build_id")]
    public required int BuildId { get; init; }

    /// <summary>
    /// Gets the build status.
    /// </summary>
    [PropertyName("build_status")]
    public required int BuildStatus { get; init; }

    /// <summary>
    /// Gets the platform.
    /// </summary>
    [PropertyName("platform")]
    public required string Platform { get; init; }

    /// <summary>
    /// Gets the Hydra project.
    /// </summary>
    [PropertyName("project")]
    public required string Project { get; init; }

    /// <summary>
    /// Gets the jobset.
    /// </summary>
    [PropertyName("jobset")]
    public required string Jobset { get; init; }

    /// <summary>
    /// Gets the job name.
    /// </summary>
    [PropertyName("job")]
    public required string Job { get; init; }

    /// <summary>
    /// Gets the output paths.
    /// </summary>
    [PropertyName("path")]
    public required HydraPath[] Path { get; init; }

    /// <summary>
    /// Gets the derivation path.
    /// </summary>
    [PropertyName("drv_path")]
    public required string DrvPath { get; init; }
}