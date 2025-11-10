using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Utils;

/// <summary>
/// Extension methods for <see cref="Stream"/> to simplify common operations
/// with minimal allocations.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Returns the contents of the stream as a byte array.
    /// </summary>
    /// <remarks>
    /// If the stream is a <see cref="MemoryStream"/>, its internal buffer is returned directly.
    /// Otherwise, the method reads from the beginning using a pooled buffer and restores the
    /// original position before returning.
    /// </remarks>
    /// <param name="stream">The source stream.</param>
    /// <returns>A new byte array containing the stream content.</returns>
    public static byte[] ToArray(this Stream stream)
    {
        if (stream is MemoryStream ms)
            return ms.ToArray();

        var pos = stream.Position;
        stream.Position = 0;
        using var rentedArray = new RentedArray<byte>(ArrayPool<byte>.Shared, 16 * 1024);
        var buffer = rentedArray.Array;
        using var copyMs = KbinConverter.RecyclableMemoryStreamManager.GetStream("byte[] returning methods");
        int read;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            copyMs.Write(buffer, 0, read);
        }

        stream.Position = pos;
        return copyMs.ToArray();
    }

    /// <summary>
    /// Writes the provided read-only span of bytes to the stream.
    /// </summary>
    /// <param name="builder">The target stream.</param>
    /// <param name="buffer">The bytes to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSpan(this Stream builder, ReadOnlySpan<byte> buffer)
    {
#if NET8_0_OR_GREATER
        builder.Write(buffer);
#else
        foreach (var b in buffer) builder.WriteByte(b);
#endif
    }
}