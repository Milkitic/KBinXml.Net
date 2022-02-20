using System;

namespace StableKbin
{
    public sealed class KbinException : Exception
    {
        public KbinException()
            : base()
        {

        }

        public KbinException(string message)
            : base(message)
        {

        }
    }
}
