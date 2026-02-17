// SPDX-License-Identifier: MIT

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace NixSearch.CLI.Extensions;

/// <summary>
/// Extension methods for configuring NixSearch services.
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Appends a line using invariant culture formatting.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="handler">The interpolated string handler.</param>
    /// <returns>Same <see cref="StringBuilder"/> instance.</returns>
    public static StringBuilder AppendInvariantLine(
        this StringBuilder stringBuilder,
        [InterpolatedStringHandlerArgument(nameof(stringBuilder))]
        ref InvariantCultureAppendInterpolatedStringHandler handler)
    {
        return stringBuilder.AppendLine();
    }

    /// <summary>
    /// An interpolated string handler that appends using invariant culture.
    /// </summary>
    [InterpolatedStringHandler]
    public readonly ref struct InvariantCultureAppendInterpolatedStringHandler
    {
        private readonly StringBuilder stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvariantCultureAppendInterpolatedStringHandler"/> struct.
        /// </summary>
        /// <param name="literalLength">The length of the literal parts of the interpolated string.</param>
        /// <param name="formattedCount">The number of formatted parts of the interpolated string.</param>
        /// <param name="stringBuilder">The string builder to append to.</param>
        public InvariantCultureAppendInterpolatedStringHandler(
            int literalLength,
            int formattedCount,
            StringBuilder stringBuilder)
        {
            _ = literalLength;
            _ = formattedCount;
            this.stringBuilder = stringBuilder;
        }

        /// <summary>
        /// Appends a literal string.
        /// </summary>
        /// <param name="str">The string to append.</param>
        public readonly void AppendLiteral(string str) =>
            this.stringBuilder.Append(str);

        /// <summary>
        /// Appends a formatted IFormattable value using invariant culture.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">The format string.</param>
        public readonly void AppendFormatted(IFormattable value, string format) =>
            this.stringBuilder.Append(value.ToString(format, CultureInfo.InvariantCulture));

        /// <summary>
        /// Appends a formatted value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to append.</param>
        public readonly void AppendFormatted<T>(T value) =>
            this.stringBuilder.Append(value);
    }
}
