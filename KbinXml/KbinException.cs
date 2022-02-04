using System;

namespace KbinXml;

public class KbinException : Exception
{
    public KbinException() : base()
    {

    }

    public KbinException(string message) : base(message)
    {

    }
}