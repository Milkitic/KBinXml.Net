using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using KbinXml.Net.Internal;
using KbinXml.Net.Internal.Debugging;
using Microsoft.IO;

namespace KbinXml.Net;

/// <summary>
/// Provides methods for converting between KBin binary format and XML representations.
/// </summary>
public static partial class KbinConverter
{
#if USELOG
    internal static ConsoleLogger Logger { get; } = new ConsoleLogger();
#else
    internal static NullLogger Logger { get; } = new NullLogger();
#endif

    internal static readonly RecyclableMemoryStreamManager RecyclableMemoryStreamManager = new()
    {
        Settings =
        {
            //BlockSize = 1024,
            AggressiveBufferReturn = true,
            //LargeBufferMultiple = 1024 * 128,
        }
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string GetActualName(string name, string? repairedPrefix)
    {
        if (string.IsNullOrEmpty(repairedPrefix))
        {
            return name;
        }

        if (name.Length < repairedPrefix!.Length)
        {
            return name;
        }

        if (name.StartsWith(repairedPrefix, StringComparison.Ordinal))
        {
            return name.Substring(repairedPrefix.Length);
        }

        return name;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string GetRepairedName(string name, string? repairedPrefix)
    {
        if (repairedPrefix is null)
        {
            return name;
        }

        if (name.Length == 0 || !IsDigit(name[0]))
        {
            return name;
        }

        return repairedPrefix + name;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsControlType(byte nodeType)
    {
        return nodeType is
            (byte)ControlType.NodeStart or
            (byte)ControlType.Attribute or
            (byte)ControlType.NodeEnd or
            (byte)ControlType.FileEnd;
    }

    /// <summary>
    /// Determines whether the specified byte array represents a valid KBin format.
    /// </summary>
    /// <param name="buffer">The byte array to check.</param>
    /// <returns>
    /// <c>true</c> if the buffer contains valid KBin format data; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method checks the KBin file signature and header structure to determine validity.
    /// It verifies:
    /// <list type="bullet">
    /// <item><description>The signature byte (0xA0)</description></item>
    /// <item><description>The encoding flag and its inverse relationship</description></item>
    /// <item><description>Minimum required buffer length</description></item>
    /// </list>
    /// </remarks>
    public static bool IsKbinFormat(byte[] buffer)
    {
        if (buffer == null)
            return false;

        return IsKbinFormat(buffer.AsSpan());
    }

    /// <summary>
    /// Determines whether the specified span represents a valid KBin format.
    /// </summary>
    /// <param name="buffer">The span to check.</param>
    /// <returns>
    /// <c>true</c> if the buffer contains valid KBin format data; otherwise, <c>false</c>.
    /// </returns>
    /// <inheritdoc cref="IsKbinFormat(byte[])"/>
    public static bool IsKbinFormat(ReadOnlySpan<byte> buffer)
    {
        // kbin format requires at least 4 bytes for the header
        if (buffer.Length < 4)
            return false;

        // Check signature (first byte must be 0xA0)
        if (buffer[0] != 0xA0)
            return false;

        // Get encoding flag and its inverse
        var encodingFlag = buffer[2];
        var encodingFlagNot = buffer[3];

        // Encoding flag should be an inverse of the fourth byte
        if ((byte)~encodingFlag != encodingFlagNot)
            return false;

        return true;
    }

    /// <summary>
    /// Determines whether the specified stream contains valid KBin format data.
    /// </summary>
    /// <param name="stream">The stream to check. The stream position will be restored after checking.</param>
    /// <returns>
    /// <c>true</c> if the stream contains valid KBin format data; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
    /// <exception cref="NotSupportedException">The stream does not support seeking.</exception>
    /// <inheritdoc cref="IsKbinFormat(byte[])"/>
    public static bool IsKbinFormat(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanSeek)
            throw new NotSupportedException("Stream must support seeking to check KBin format.");

        var originalPosition = stream.Position;
        var buffer = ArrayPool<byte>.Shared.Rent(4);
        try
        {
            // Read the first 4 bytes for header validation
            var bytesRead = stream.Read(buffer, 0, 4);

            if (bytesRead < 4)
                return false;

            return IsKbinFormat(buffer.AsSpan(0, 4));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            // Restore original position
            stream.Position = originalPosition;
        }
    }
}
