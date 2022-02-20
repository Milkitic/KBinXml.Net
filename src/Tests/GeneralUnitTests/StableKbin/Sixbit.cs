using System;
using System.Collections.Generic;
using System.Linq;

namespace StableKbin
{
    internal static class Sixbit
    {
        private const string Charset = "0123456789:ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";

        private static readonly Dictionary<char, byte> CharsetMapping = Charset
            .Select((k, i) => (i, k))
            .ToDictionary(k => k.k, k => (byte)k.i);

        internal static byte[] Encode(string input)
        {
            var buffer = input.Length <= 128
                ? stackalloc byte[input.Length]
                : new byte[input.Length];
            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];
                buffer[i] = CharsetMapping[c];
            }

            var length = (int)Math.Ceiling(buffer.Length * 6.0 / 8);
            var output = length <= 128
                ? stackalloc byte[length]
                : new byte[length];

            for (var i = 0; i < buffer.Length * 6; i++)
                output[i / 8] = (byte)(output[i / 8] |
                    ((buffer[i / 6] >> (5 - (i % 6)) & 1) << (7 - (i % 8))));

            var encode = output.Slice(0, output.Length);
            return encode.ToArray();
        }

        internal static string Decode(Span<byte> buffer, int length)
        {
            var output = length <= 128
                ? stackalloc byte[length]
                : new byte[length];

            for (var i = 0; i < length * 6; i++)
                output[i / 6] = (byte)(output[i / 6] |
                    (((buffer[i / 8] >> (7 - (i % 8))) & 1) << (5 - (i % 6))));

            var result = output.Length <= 128
                ? stackalloc char[output.Length]
                : new char[output.Length];

            for (var i = 0; i < output.Length; i++)
            {
                var c = output[i];
                result[i] = Charset[c];
            }

#if NETSTANDARD2_1 || NET5_0_OR_GREATER
            return new string(result);
#elif NETSTANDARD2_0
            return new string(result.ToArray());
#endif
        }
    }
}
