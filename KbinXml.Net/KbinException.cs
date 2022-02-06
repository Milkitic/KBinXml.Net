using System;

namespace KbinXml.Net;

/// <summary>
/// Represents error that occurs in KBin operations.
/// </summary>
public class KbinException : Exception
{
    public KbinException() : base()
    {

    }

    public KbinException(string message) : base(message)
    {

    }
}