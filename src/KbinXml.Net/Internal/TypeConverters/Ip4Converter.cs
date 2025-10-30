using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

internal sealed class Ip4Converter : ITypeConverter
{
    private Ip4Converter()
    {
    }

    public static Ip4Converter Instance { get; } = new();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(Span<byte> span, ReadOnlySpan<char> str)
    {
        var bytes = IPAddress.Parse(str
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ).GetAddressBytes();

        bytes.CopyTo(span);
        return bytes.Length; // 返回 4（IPv4 地址为 4 个字节）
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // todo: loop here
    public int WriteString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var bytes = IPAddress.Parse(str
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ).GetAddressBytes();

        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i]);
        }

        return bytes.Length; // 返回 4（IPv4 地址为 4 个字节）
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(ReadOnlySpan<byte> span)
    {
        var privateAddress = MemoryMarshal.Read<uint>(span);
        Span<char> dst = stackalloc char[15];
        int charsWritten = IPv4AddressToStringHelper(privateAddress, dst);
        unsafe
        {
            fixed (char* p = dst)
            {
                return new string(p, 0, charsWritten);
            }
        }
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendString(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        var privateAddress = MemoryMarshal.Read<uint>(span);
        Span<char> dst = stackalloc char[15];
        int charsWritten = IPv4AddressToStringHelper(privateAddress, dst);
        stringBuilder.Append(dst.Slice(0, charsWritten));
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IPv4AddressToStringHelper(uint address, Span<char> dst)
    {
        int offset = 0;
        address = (uint)IPAddress.NetworkToHostOrder(unchecked((int)address));

        FormatIPv4AddressNumber((int)((address >> 24) & 0xFF), dst, ref offset);
        dst[offset++] = '.';
        FormatIPv4AddressNumber((int)((address >> 16) & 0xFF), dst, ref offset);
        dst[offset++] = '.';
        FormatIPv4AddressNumber((int)((address >> 8) & 0xFF), dst, ref offset);
        dst[offset++] = '.';
        FormatIPv4AddressNumber((int)(address & 0xFF), dst, ref offset);

        return offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void FormatIPv4AddressNumber(int number, Span<char> dst, ref int offset)
    {
        offset += number > 99 ? 3 : number > 9 ? 2 : 1;

        int i = offset;
        do
        {
            number = Math.DivRem(number, 10, out int rem);
            dst[--i] = (char)('0' + rem);
        } while (number != 0);
    }
}