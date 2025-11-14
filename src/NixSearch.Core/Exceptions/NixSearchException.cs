// SPDX-License-Identifier: MIT

using System;

namespace NixSearch.Core.Exceptions;

/// <summary>
/// Exception thrown when a NixSearch operation fails.
/// </summary>
public class NixSearchException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NixSearchException"/> class.
    /// </summary>
    public NixSearchException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NixSearchException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NixSearchException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NixSearchException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NixSearchException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}