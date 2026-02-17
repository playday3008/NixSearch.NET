// SPDX-License-Identifier: MIT

namespace NixSearch.MCP.Logging;

/// <summary>
/// A deferred string-join formatter: <see cref="ToString"/> is only called
/// by the <see cref="Microsoft.Extensions.Logging.LoggerMessageAttribute"/> source generator
/// when the log level is enabled.
/// </summary>
internal readonly struct CommaSeparatedValues(string[] items)
{
    /// <summary>
    /// Implicitly converts a string array to <see cref="CommaSeparatedValues"/>.
    /// </summary>
    /// <param name="items">The items to join.</param>
    public static implicit operator CommaSeparatedValues(string[] items) => new(items);

    /// <inheritdoc/>
    public override string ToString() => string.Join(", ", items);
}
