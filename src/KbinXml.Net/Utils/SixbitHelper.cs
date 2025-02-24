using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using KbinXml.Net.Internal;
namespace KbinXml.Net.Utils;

public static class SixbitHelper
{
    private const string Charset = "0123456789:ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";
    private static readonly byte[] CharsetMapping = new byte[128];
    private static readonly char[] CharsetArray = Charset.ToCharArray();

    static SixbitHelper()
    {
        for (var i = 0; i < Charset.Length; i++)
            CharsetMapping[Charset[i]] = (byte)i;
    }

    public static byte[] Encode(string input)
    {
        using var ms = new MemoryStream();
        EncodeCore(input, ms);
        return ms.ToArray();
    }

    public static void EncodeAndWrite(Stream stream, string input) => EncodeCore(input, stream);

    private static void EncodeCore(string input, Stream stream)
    {
        var inputLength = input.Length;
        var outputLength = (inputLength * 6 + 7) / 8;

        if (inputLength <= Constants.MaxStackLength)
        {
            Span<byte> inputBuffer = stackalloc byte[inputLength];
            Span<byte> outputBuffer = stackalloc byte[outputLength];
            FillInput(input, inputBuffer);
            SixbitHelperOptimized.Encode(inputBuffer, outputBuffer);
            stream.WriteSpan(outputBuffer);
        }
        else
        {
            using var rentedInput = new RentedArray<byte>(ArrayPool<byte>.Shared, inputLength);
            using var rentedOutput = new RentedArray<byte>(ArrayPool<byte>.Shared, outputLength);
            var inputSpan = rentedInput.Array.AsSpan(0, inputLength);
            var outputSpan = rentedOutput.Array.AsSpan(0, outputLength);
            FillInput(input, inputSpan);
            SixbitHelperOptimized.Encode(inputSpan, outputSpan);
            stream.WriteSpan(outputSpan);
        }
    }

    public static string Decode(ReadOnlySpan<byte> buffer, int length)
    {
        if (length <= Constants.MaxStackLength)
        {
            Span<byte> input = stackalloc byte[length];
            SixbitHelperOptimized.Decode(buffer, input);
            return GetString(input);
        }

        using var rentedInput = new RentedArray<byte>(ArrayPool<byte>.Shared, length);
        var inputSpan = rentedInput.Array.AsSpan(0, length);
        SixbitHelperOptimized.Decode(buffer, inputSpan);
        return GetString(inputSpan);
    }

    [InlineMethod.Inline]
    private static void FillInput(string content, Span<byte> buffer)
    {
        ref var contentRef = ref MemoryMarshal.GetReference(content.AsSpan());
        ref var bufferRef = ref MemoryMarshal.GetReference(buffer);

        for (var i = 0; i < buffer.Length; i++)
            Unsafe.Add(ref bufferRef, i) = CharsetMapping[Unsafe.Add(ref contentRef, i)];
    }

    [InlineMethod.Inline]
    private static string GetString(Span<byte> input)
    {
        Span<char> chars = stackalloc char[input.Length];
        ref var inputRef = ref MemoryMarshal.GetReference(input);
        ref var charsRef = ref MemoryMarshal.GetReference(chars);

        for (var i = 0; i < input.Length; i++)
            Unsafe.Add(ref charsRef, i) = CharsetArray[Unsafe.Add(ref inputRef, i)];

#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        return new string(chars);
#else
        unsafe
        {
            fixed (char* p = chars)
                return new string(p, 0, chars.Length);
        }
#endif
    }
}