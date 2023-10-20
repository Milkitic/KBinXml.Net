using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Utils;

public static class StreamExtensions
{
    public static byte[] ToArray(this Stream stream)
    {
        if (stream is MemoryStream ms)
            return ms.ToArray();

        var pos = stream.Position;
        stream.Position = 0;
        byte[] buffer = new byte[16 * 1024];
        using var copyMs = new MemoryStream();
        int read;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            copyMs.Write(buffer, 0, read);
        }

        stream.Position = pos;
        return copyMs.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSpan(this Stream builder, ReadOnlySpan<byte> buffer)
    {
#if NETCOREAPP3_1_OR_GREATER
        builder.Write(buffer);
#else
        foreach (var b in buffer) builder.WriteByte(b);
#endif
    }
}