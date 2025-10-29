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
    [Obsolete("Use EncodeAndWrite() instead", true)]
    public static byte[] Encode(string input)
    {
        using var ms = KbinConverter.RecyclableMemoryStreamManager.GetStream("byte[] returning methods");
        EncodeCore(input, ms);
        return ms.ToArray();
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

    /// <summary>
    /// Internal encoding dispatcher for the optimized <see cref="RecyclableMemoryStream"/> path.
    /// </summary>
    private static void EncodeCore(string input, RecyclableMemoryStream stream)
    {
        var inputLength = input.Length;
        var outputLength = (inputLength * 6 + 7) >> 3;

        if (inputLength <= Constants.MaxStackLength)
        {
            Span<byte> inputBuffer = stackalloc byte[inputLength];
            FillInputSmall(input, inputBuffer);
            EncodeCoreRecyclable(inputBuffer, stream, outputLength);
        }
        else
        {
            using var rentedInput = new RentedArray<byte>(ArrayPool<byte>.Shared, inputLength);
            var inputSpan = rentedInput.Array.AsSpan(0, inputLength);
            FillInputLarge(input, inputSpan);
            EncodeCoreRecyclable(inputSpan, stream, outputLength);
        }
    }

    /// <summary>
    /// Internal encoding dispatcher for the generic <see cref="Stream"/> path.
    /// </summary>
    private static void EncodeCore(string input, Stream stream)
    {
        if (stream is RecyclableMemoryStream rms)
        {
            EncodeCore(input, rms);
            return;
        }

        var inputLength = input.Length;
        var outputLength = (inputLength * 6 + 7) >> 3;

        if (inputLength <= Constants.MaxStackLength)
        {
            Span<byte> inputBuffer = stackalloc byte[inputLength];
            Span<byte> outputBuffer = stackalloc byte[outputLength];
            FillInputSmall(input, inputBuffer);
            EncodeCoreGeneric(inputBuffer, outputBuffer, stream);
        }
        else
        {
            using var rentedInput = new RentedArray<byte>(ArrayPool<byte>.Shared, inputLength);
            using var rentedOutput = new RentedArray<byte>(ArrayPool<byte>.Shared, outputLength);
            var inputSpan = rentedInput.Array.AsSpan(0, inputLength);
            var outputSpan = rentedOutput.Array.AsSpan(0, outputLength);
            FillInputLarge(input, inputSpan);
            EncodeCoreGeneric(inputSpan, outputSpan, stream);
        }
    }

    /// <summary>
    /// Encodes to the stream using the high-performance GetSpan/Advance pattern.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EncodeCoreRecyclable(Span<byte> inputBuffer, RecyclableMemoryStream stream, int outputLength)
    {
        var outputSpan = stream.GetSpan(outputLength);
        outputSpan.Slice(0, outputLength).Clear(); // Clear() Must be required
        SixbitHelperEncImpl.Encode(inputBuffer, outputSpan);
        stream.Advance(outputLength);
    }

    /// <summary>
    /// Encodes to an intermediate buffer and writes it to the generic stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EncodeCoreGeneric(Span<byte> inputBuffer, Span<byte> outputBuffer, Stream stream)
    {
        SixbitHelperEncImpl.Encode(inputBuffer, outputBuffer);
        stream.WriteSpan(outputBuffer);
    }

    /// <summary>
    /// Fills the destination buffer from the source string (for small inputs).
    /// </summary>
    private static void FillInputSmall(string content, Span<byte> buffer)
    {
        ref var contentRef = ref MemoryMarshal.GetReference(content.AsSpan());
        ref var bufferRef = ref MemoryMarshal.GetReference(buffer);

        for (var i = 0; i < buffer.Length; i++)
            Unsafe.Add(ref bufferRef, i) = CharsetMapping[Unsafe.Add(ref contentRef, i)];
    }

    /// <summary>
    /// Fills the destination buffer from the source string (for large inputs, with loop unrolling).
    /// </summary>
    private static void FillInputLarge(string content, Span<byte> buffer)
    {
        ref var contentRef = ref MemoryMarshal.GetReference(content.AsSpan());
        ref var bufferRef = ref MemoryMarshal.GetReference(buffer);

        var length = buffer.Length;
        var i = 0;
        var unrollLimit = length - 3;
        for (; i < unrollLimit; i += 4)
        {
            Unsafe.Add(ref bufferRef, i) = CharsetMapping[Unsafe.Add(ref contentRef, i)];
            Unsafe.Add(ref bufferRef, i + 1) = CharsetMapping[Unsafe.Add(ref contentRef, i + 1)];
            Unsafe.Add(ref bufferRef, i + 2) = CharsetMapping[Unsafe.Add(ref contentRef, i + 2)];
            Unsafe.Add(ref bufferRef, i + 3) = CharsetMapping[Unsafe.Add(ref contentRef, i + 3)];
        }

        for (; i < length; i++)
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
                ref var charsRef = ref MemoryMarshal.GetReference(chars);

                for (var i = 0; i < chars.Length; i++)
                {
                    var index = ptr[i];
                    var value = CharsetArray[index];
                    Unsafe.Add(ref charsRef, i) = value;
                }
            });
        }
#else
        if (input.Length <= Constants.MaxStackLength)
        {
            Span<char> chars = stackalloc char[input.Length];
            FillChars(input, chars);
            fixed (char* p = chars)
                return new string(p, 0, chars.Length);
        }

        using var rentedChars = new RentedArray<char>(ArrayPool<char>.Shared, input.Length);
        var charSpan = rentedChars.Array.AsSpan(0, input.Length);
        FillChars(input, charSpan);
        fixed (char* p = charSpan)
            return new string(p, 0, charSpan.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void FillChars(ReadOnlySpan<byte> input, Span<char> chars)
        {
            ref var inputRef = ref MemoryMarshal.GetReference(input);
            ref var charsRef = ref MemoryMarshal.GetReference(chars);

            for (var i = 0; i < input.Length; i++)
                Unsafe.Add(ref charsRef, i) = CharsetArray[Unsafe.Add(ref inputRef, i)];
        }
#endif
    }
}