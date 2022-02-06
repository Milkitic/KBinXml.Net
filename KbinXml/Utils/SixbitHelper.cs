using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KbinXml.Internal;

namespace KbinXml.Utils;

public static class SixbitHelper
{
    private const string Charset = "0123456789:ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";

    private static readonly Dictionary<char, byte> CharsetMapping = Charset
        .Select((k, i) => (i, k))
        .ToDictionary(k => k.k, k => (byte)k.i);

    public static byte[] Encode(string input)
    {
        using var ms = new MemoryStream();
        EncodeAndWrite(ms, input);
        return ms.ToArray();
    }

    public static void EncodeAndWrite(Stream stream, string input)
    {
        byte[]? arrIn = null;
        var buffer = input.Length <= Constants.MaxStackLength
            ? stackalloc byte[input.Length]
            : arrIn = ArrayPool<byte>.Shared.Rent(input.Length);
        try
        {
            if (arrIn != null) buffer = buffer.Slice(0, input.Length);
            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];
                buffer[i] = CharsetMapping[c];
            }

            var length = (int)Math.Ceiling(buffer.Length * 6.0 / 8);

            byte[]? arrOut = null;
            var output = length <= Constants.MaxStackLength
                ? stackalloc byte[length]
                : arrOut = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                if (arrOut != null) output = output.Slice(0, length);

                for (var i = 0; i < buffer.Length * 6; i++)
                    output[i / 8] = (byte)(output[i / 8] |
                                           ((buffer[i / 6] >> (5 - (i % 6)) & 1) << (7 - (i % 8))));

                var encode = output.Slice(0, output.Length);
                stream.WriteSpan(encode);
            }
            finally
            {
                if (arrOut != null) ArrayPool<byte>.Shared.Return(arrOut);
            }
        }
        finally
        {
            if (arrIn != null) ArrayPool<byte>.Shared.Return(arrIn);
        }
    }

    public static string Decode(ReadOnlySpan<byte> buffer, int length)
    {
        byte[]? arrOutput = null;
        var output = length <= Constants.MaxStackLength
            ? stackalloc byte[length]
            : arrOutput = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            if (arrOutput != null) output = output.Slice(0, length);

            for (var i = 0; i < length * 6; i++)
                output[i / 6] = (byte)(output[i / 6] |
                                       (((buffer[i / 8] >> (7 - (i % 8))) & 1) << (5 - (i % 6))));
            char[]? arrResult = null;
            var result = output.Length <= Constants.MaxStackLength
                ? stackalloc char[output.Length]
                : arrResult = ArrayPool<char>.Shared.Rent(output.Length);
            try
            {
                if (arrResult != null) result = result.Slice(0, output.Length);

                for (var i = 0; i < output.Length; i++)
                {
                    var c = output[i];
                    result[i] = Charset[c];
                }

#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
                return new string(result);
#elif NETSTANDARD2_0
                unsafe
                {
                    fixed (char* p = result)
                        return new string(p, 0, result.Length);
                }
#endif
            }
            finally
            {
                if (arrResult != null) ArrayPool<char>.Shared.Return(arrResult);
            }
        }
        finally
        {
            if (arrOutput != null) ArrayPool<byte>.Shared.Return(arrOutput);
        }
    }
}