using System;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Utils;

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpaceSplitEnumerator SpanSplit(this string str, char c)
    {
        // SpaceSplitEnumerator is a struct so there is no allocation here
        return new SpaceSplitEnumerator(str.AsSpan(), c);
    }

    // Must be a ref struct as it contains a ReadOnlySpan<char>
    public ref struct SpaceSplitEnumerator
    {
        private ReadOnlySpan<char> _str;
        private readonly char _c;

        public SpaceSplitEnumerator(ReadOnlySpan<char> str, char c)
        {
            _str = str;
            _c = c;
            Current = default;
        }

        // Needed to be compatible with the foreach operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SpaceSplitEnumerator GetEnumerator() => this;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var span = _str;
            if (span.Length == 0) // Reach the end of the string
                return false;

            var index = span.IndexOf(_c);
            if (index == -1) // The string is composed of only one content
            {
                _str = ReadOnlySpan<char>.Empty; // The remaining string is an empty string
                Current = span;
                return true;
            }

            Current = span.Slice(0, index);
            _str = span.Slice(index + 1);
            return true;
        }

        public ReadOnlySpan<char> Current { get; private set; }
    }
}