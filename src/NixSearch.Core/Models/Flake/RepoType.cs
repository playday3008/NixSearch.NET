// SPDX-License-Identifier: MIT

using System.Runtime.Serialization;

using Elasticsearch.Net;

namespace NixSearch.Core.Models.Flake;

/// <summary>
/// Defines the type of repository.
/// </summary>
[StringEnum]
public enum RepoType
{
    /// <summary>
    /// Git repository.
    /// </summary>
    [EnumMember(Value = "git")]
    Git,

    /// <summary>
    /// GitHub repository.
    /// </summary>
    [EnumMember(Value = "github")]
    GitHub,

    /// <summary>
    /// GitLab repository.
    /// </summary>
    [EnumMember(Value = "gitlab")]
    GitLab,

    /// <summary>
    /// SourceHut repository.
    /// </summary>
    [EnumMember(Value = "sourcehut")]
    SourceHut,
}
