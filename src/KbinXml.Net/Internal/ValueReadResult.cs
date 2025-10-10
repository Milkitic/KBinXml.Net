using System.Diagnostics;

namespace KbinXml.Net.Internal;

[DebuggerDisplay("{ToDebuggerDisplay}")]
internal readonly ref struct ValueReadResult<T>
{
    public readonly T Value;
#if USELOG
    public readonly ReadStatus ReadStatus;
#endif

    public ValueReadResult(T value
#if USELOG
        , ReadStatus readStatus
#endif
        )
    {
        Value = value;
#if USELOG
        ReadStatus = readStatus;
#endif
    }

    public string ToDebuggerDisplay
    {
        get
        {
            if (Value is byte b)
                return b.ToString("X2");
            if (Value is short @short)
            {
                return @short.ToString("X4");
            }
            if (Value is int @int)
            {
                return @int.ToString("X8");
            }
            if (Value is long @long)
            {
                return @long.ToString("X16");
            }
            if (Value is sbyte sb)
            {
                return sb.ToString("X2");
            }
            if (Value is ushort us)
            {
                return us.ToString("X4");
            }
            if (Value is uint ui)
            {
                return ui.ToString("X8");
            }
            if (Value is ulong ul)
            {
                return ul.ToString("X16");
            }
            if (Value is float f)
            {
                return f.ToString("F");
            }
            if (Value is double d)
            {
                return d.ToString("F");
            }

            return Value.ToString();
        }
    }
}