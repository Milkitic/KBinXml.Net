using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using KbinXml.Net.Utils;
using Microsoft.IO;

//using SixbitHelperImpl = KbinXml.Net.Utils.SixbitHelperOptimized;
#if NET6_0_OR_GREATER
using SixbitHelperEncImpl = KbinXml.Net.Internal.Sixbit.SixbitHelperCoreClrOptimized;
using SixbitHelperDecImpl = KbinXml.Net.Internal.Sixbit.SixbitHelperCoreClrOptimized;
#else
using SixbitHelperEncImpl = KbinXml.Net.Internal.Sixbit.SixbitHelperSuperOptimized;
using SixbitHelperDecImpl = KbinXml.Net.Internal.Sixbit.SixbitHelperSuperOptimized;
#endif

namespace KbinXml.Net.Internal;

/// <summary>
/// Provides methods for converting between strings and 6-bit encoded binary data.
/// </summary>
internal static class SixbitHelper
{
    private const string Charset = "0123456789:ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";
    private static readonly byte[] CharsetMapping = new byte[128];
    private static readonly char[] CharsetArray = Charset.ToCharArray();

    static SixbitHelper()
    {
        for (var i = 0; i < Charset.Length; i++)
            CharsetMapping[Charset[i]] = (byte)i;
    }

    /// <summary>
    /// Encodes a string into 6-bit encoded binary data.
    /// </summary>
    /// <param name="input">The string to encode.</param>
    /// <returns>A byte array containing the 6-bit encoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="input"/> is <see langword="null"/>.</exception>
    public static byte[] Encode(string input)
    {
        using var ms = KbinConverter.RecyclableMemoryStreamManager.GetStream("byte[] returning methods");
        EncodeCore(input, ms);
        return ms.GetBuffer();
    }

    public static void EncodeAndWrite(RecyclableMemoryStream stream, string input)
    {
        EncodeCore(input, stream);
    }

    /// <summary>
    /// Encodes a string and writes the 6-bit encoded data directly to a stream.
    /// </summary>
    /// <param name="stream">The output stream to write to.</param>
    /// <param name="input">The string to encode.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="input"/> is <see langword="null"/>.</exception>
    public static void EncodeAndWrite(Stream stream, string input)
    {
        EncodeCore(input, stream);
    }

    /// <summary>
    /// Decodes 6-bit encoded binary data back to a string.
    /// </summary>
    /// <param name="buffer">The buffer containing the encoded data.</param>
    /// <param name="length">The number of bytes to decode.</param>
    /// <returns>The decoded string.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> exceeds the buffer size.</exception>
    public static string Decode(ReadOnlySpan<byte> buffer, int length)
    {
        if (length <= Constants.MaxStackLength)
        {
            Span<byte> input = stackalloc byte[length];
            SixbitHelperDecImpl.Decode(buffer, input);
            return GetString(input);
        }

        using var rentedInput = new RentedArray<byte>(ArrayPool<byte>.Shared, length);
        var inputSpan = rentedInput.Array.AsSpan(0, length);
        SixbitHelperDecImpl.Decode(buffer, inputSpan);
        return GetString(inputSpan);
    }

    private static void EncodeCore(string input, RecyclableMemoryStream stream)
    {
        var inputLength = input.Length;
        var outputLength = (inputLength * 6 + 7) / 8;

        if (inputLength <= Constants.MaxStackLength)
        {
            Span<byte> inputBuffer = stackalloc byte[inputLength];
            FillInput(input, inputBuffer);
            Span<byte> outputBuffer = stream.GetSpan(outputLength);
            outputBuffer.Slice(0, outputLength).Clear();
            SixbitHelperEncImpl.Encode(inputBuffer, outputBuffer);
            stream.Advance(outputLength);
        }
        else
        {
            using var rentedInput = new RentedArray<byte>(ArrayPool<byte>.Shared, inputLength);
            var inputSpan = rentedInput.Array.AsSpan(0, inputLength);
            FillInput(input, inputSpan);
            Span<byte> outputSpan = stream.GetSpan(outputLength);
            outputSpan.Slice(0, outputLength).Clear();
            SixbitHelperEncImpl.Encode(inputSpan, outputSpan);
            stream.Advance(outputLength);
        }
    }

    private static void EncodeCore(string input, Stream stream)
    {
        var inputLength = input.Length;
        var outputLength = (inputLength * 6 + 7) / 8;

        if (inputLength <= Constants.MaxStackLength)
        {
            Span<byte> inputBuffer = stackalloc byte[inputLength];
            Span<byte> outputBuffer = stackalloc byte[outputLength];
            FillInput(input, inputBuffer);
            SixbitHelperEncImpl.Encode(inputBuffer, outputBuffer);
            stream.WriteSpan(outputBuffer);
        }
        else
        {
            using var rentedInput = new RentedArray<byte>(ArrayPool<byte>.Shared, inputLength);
            using var rentedOutput = new RentedArray<byte>(ArrayPool<byte>.Shared, outputLength);
            var inputSpan = rentedInput.Array.AsSpan(0, inputLength);
            var outputSpan = rentedOutput.Array.AsSpan(0, outputLength);
            FillInput(input, inputSpan);
            SixbitHelperEncImpl.Encode(inputSpan, outputSpan);
            stream.WriteSpan(outputSpan);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void FillInput(string content, Span<byte> buffer)
    {
        ref var contentRef = ref MemoryMarshal.GetReference(content.AsSpan());
        ref var bufferRef = ref MemoryMarshal.GetReference(buffer);

        for (var i = 0; i < buffer.Length; i++)
            Unsafe.Add(ref bufferRef, i) = CharsetMapping[Unsafe.Add(ref contentRef, i)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe string GetString(scoped Span<byte> input)
    {
#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        fixed (byte* inputPtr = input)
        {
            return string.Create(input.Length, (nint)inputPtr, (chars, state) =>
            {
                var ptr = (byte*)state.ToPointer();
                for (var i = 0; i < chars.Length; i++)
                {
                    chars[i] = CharsetArray[ptr[i]];
                }
            });
        }
#else
        Span<char> chars = stackalloc char[input.Length];
        ref var inputRef = ref MemoryMarshal.GetReference(input);
        ref var charsRef = ref MemoryMarshal.GetReference(chars);

        for (var i = 0; i < input.Length; i++)
            Unsafe.Add(ref charsRef, i) = CharsetArray[Unsafe.Add(ref inputRef, i)];

        fixed (char* p = chars)
            return new string(p, 0, chars.Length);
#endif
    }
}