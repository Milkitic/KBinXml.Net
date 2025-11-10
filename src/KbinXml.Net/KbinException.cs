using System;

namespace KbinXml.Net;

/// <summary>
/// Thrown when the specified KBin node type is not found.
/// </summary>
public class KbinTypeNotFoundException : KbinException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KbinTypeNotFoundException"/> class.
    /// </summary>
    /// <param name="type">The missing node type name.</param>
    public KbinTypeNotFoundException(string type) : base($"The node type '{type}' was not found.")
    {

    }
}

/// <summary>
/// Thrown when an array length does not match the value of the '__count' attribute.
/// </summary>
public class KbinArrayCountMissMatchException : KbinException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KbinArrayCountMissMatchException"/> class.
    /// </summary>
    /// <param name="expectedValue">The expected array length (may be <see langword="null"/>).</param>
    /// <param name="actualValue">The actual array length.</param>
    public KbinArrayCountMissMatchException(object? expectedValue, object actualValue) : base(
        $"Array length does not match the '__count' attribute. Expected: {expectedValue}, Actual: {actualValue}.")
    {

    }
}

/// <summary>
/// Represents errors that occur during KBin API operations.
/// </summary>
/// <remarks>
/// This exception is thrown when KBin-related operations fail, typically containing 
/// specific error information about the failure cause.
/// </remarks>
public class KbinException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KbinException"/> class.
    /// </summary>
    public KbinException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KbinException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public KbinException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KbinException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, 
    /// or <see langword="null"/> if no inner exception is specified.</param>
    public KbinException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}