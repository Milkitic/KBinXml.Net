using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KbinXml.Net.Internal;

#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace KbinXml.Net.Utils;

public static class SixbitHelper
{
    private const string Charset = "0123456789:ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";

    private static readonly IReadOnlyDictionary<char, byte> CharsetMapping = Charset
            .Select((k, i) => (i, k))
#if NET8_0_OR_GREATER
            .ToFrozenDictionary(k => k.k, k => (byte)k.i)
#else
            .ToDictionary(k => k.k, k => (byte)k.i)
#endif
        ;

    public static byte[] Encode(string input)
    {
        using var ms = new MemoryStream();
        EncodeAndWrite(ms, input);
        return ms.ToArray();
    }

    public static void EncodeAndWrite(Stream stream, string input)
    {
        bool useStack = input.Length <= Constants.MaxStackLength;
        if (useStack)
        {
            Span<byte> buffer = stackalloc byte[input.Length];
            EncodeFillInput(input, ref buffer);

            var length = (int)Math.Ceiling(buffer.Length * 6 / 8d);

            Span<byte> output = stackalloc byte[length];
            EncodeFillOutput(buffer, ref output);

            stream.WriteSpan(output);
        }
        else
        {
            byte[]? arrIn = null;
            byte[]? arrOut = null;

            try
            {
                arrIn = ArrayPool<byte>.Shared.Rent(input.Length);
                var buffer = arrIn.AsSpan(0, input.Length);
                EncodeFillInput(input, ref buffer);

                var length = (int)Math.Ceiling(buffer.Length * 6 / 8d);

                arrOut = ArrayPool<byte>.Shared.Rent(length);
                var output = arrOut.AsSpan(0, length);
                EncodeFillOutput(buffer, ref output);

                stream.WriteSpan(output);
            }
            finally
            {
                if (arrIn != null) ArrayPool<byte>.Shared.Return(arrIn);
                if (arrOut != null) ArrayPool<byte>.Shared.Return(arrOut);
            }
        }
    }

    public static string Decode(ReadOnlySpan<byte> buffer, int length)
    {
        bool useStack = length <= Constants.MaxStackLength;
        if (useStack)
        {
            Span<byte> input = stackalloc byte[length];
            DecodeFillInput(buffer, ref input);

            Span<char> result = stackalloc char[input.Length];
            return DecodeGetString(input, result);
        }

        byte[]? arrOutput = null;
        char[]? arrResult = null;
        try
        {
            arrOutput = ArrayPool<byte>.Shared.Rent(length);
            var input = arrOutput.AsSpan(0, length);
            DecodeFillInput(buffer, ref input);

            arrResult = ArrayPool<char>.Shared.Rent(input.Length);
            var result = arrResult.AsSpan(0, input.Length);
            return DecodeGetString(input, result);
        }
        finally
        {
            if (arrOutput != null) ArrayPool<byte>.Shared.Return(arrOutput);
            if (arrResult != null) ArrayPool<char>.Shared.Return(arrResult);
        }
    }

    [InlineMethod.Inline]
    private static void EncodeFillInput(string content, ref Span<byte> input)
    {
        for (var i = 0; i < content.Length; i++)
        {
            var c = content[i];
            input[i] = CharsetMapping[c];
        }
    }

    [InlineMethod.Inline]
    public static void EncodeFillOutput(ReadOnlySpan<byte> buffer, ref Span<byte> output)
    {
        for (var i = 0; i < buffer.Length * 6; i++)
            output[i >> 3] = (byte)(output[i >> 3] |
                                    ((buffer[i / 6] >> (5 - (i % 6)) & 1) << (7 - (i & 7))));
    }

    [InlineMethod.Inline]
    public static void DecodeFillInput(ReadOnlySpan<byte> buffer, ref Span<byte> input)
    {
        for (var i = 0; i < input.Length * 6; i++)
            input[i / 6] = (byte)(input[i / 6] |
                                  (((buffer[i >> 3] >> (7 - (i & 7))) & 1) << (5 - (i % 6))));
    }

    [InlineMethod.Inline]
    private static string DecodeGetString(Span<byte> input, Span<char> result)
    {
        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            result[i] = Charset[c];
        }
#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        return new string(result);
#else
        unsafe
        {
            fixed (char* p = result)
                return new string(p, 0, result.Length);
        }
#endif
    }
}