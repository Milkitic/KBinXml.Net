using System;

namespace KbinXml.Net.Utils;

internal static class SixbitHelperOriginal
{
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
}