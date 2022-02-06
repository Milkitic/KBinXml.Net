using System;

namespace KbinXml.Net;

public class KbinException : Exception
{
    public KbinException() : base()
    {

    }

    public KbinException(string message) : base(message)
    {

    }
}