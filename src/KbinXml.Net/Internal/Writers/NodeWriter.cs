using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using KbinXml.Net.Utils;
using Microsoft.IO;

namespace KbinXml.Net.Internal.Writers;

internal readonly ref partial struct NodeWriter
{
    public readonly RecyclableMemoryStream Stream;
    public readonly bool Compressed;

    private readonly Encoding _encoding;

    public NodeWriter(bool compressed, Encoding encoding, int capacity = 0)
    {
        Stream = KbinConverter.RecyclableMemoryStreamManager.GetStream("wn", capacity);
        Compressed = compressed;
        _encoding = encoding;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(string value)
    {
        if (Compressed)
        {
            WriteU8((byte)value.Length);
            SixbitHelper.EncodeAndWrite(Stream, value);
        }
        else
        {
            WriteU8((byte)((value.Length - 1) | (1 << 6)));

            // https://stackoverflow.com/questions/9533258/what-is-the-maximum-number-of-bytes-for-a-utf-8-encoded-character
            //int byteCount = _encoding.GetByteCount(value);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            var span = Stream.GetSpan(value.Length * 4);
            int bytesWritten = _encoding.GetBytes(value.AsSpan(), span);
            Stream.Advance(bytesWritten);
#else
            using (var rentedArray = new RentedArray<byte>(ArrayPool<byte>.Shared, value.Length * 4))
            {
                int bytesEncoded = _encoding.GetBytes(value, 0, value.Length, rentedArray.Array, 0);
                Stream.Write(rentedArray.Array, 0, bytesEncoded);
            }
#endif
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("This method has degraded performance and should be avoided.")]
    public byte[] ToArray()
    {
        return Stream.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        Stream.Dispose();
    }
}