using System;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Utils;

/// <summary>
/// Provides span-friendly string extension methods and a lightweight
/// enumerator for splitting without allocations.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Splits the specified read-only character span by the given delimiter
    /// into an allocation-free enumerator.
    /// </summary>
    /// <param name="str">The source span to split.</param>
    /// <param name="c">The delimiter character.</param>
    /// <returns>A <see cref="SpaceSplitEnumerator"/> for iterating over segments.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpaceSplitEnumerator SpanSplit(this ReadOnlySpan<char> str, char c)
    {
        // SpaceSplitEnumerator is a struct so there is no allocation here
        return new SpaceSplitEnumerator(str, c);
    }

    /// <summary>
    /// Splits the specified string by the given delimiter into an allocation-free enumerator.
    /// </summary>
    /// <param name="str">The source string to split.</param>
    /// <param name="c">The delimiter character.</param>
    /// <returns>A <see cref="SpaceSplitEnumerator"/> for iterating over segments.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpaceSplitEnumerator SpanSplit(this string str, char c)
    {
        // SpaceSplitEnumerator is a struct so there is no allocation here
        return new SpaceSplitEnumerator(str.AsSpan(), c);
    }

    // Must be a ref struct as it contains a ReadOnlySpan<char>
    /// <summary>
    /// Allocation-free enumerator that yields substrings split by a single delimiter.
    /// </summary>
    /// <remarks>
    /// This is a <c>ref struct</c> and cannot be boxed, captured, or used across
    /// async/await boundaries. It is intended for short-lived enumeration within a method.
    /// </remarks>
    public ref struct SpaceSplitEnumerator
    {
        private ReadOnlySpan<char> _str;
        private readonly char _c;

        /// <summary>
        /// Initializes the enumerator with the source span and delimiter.
        /// </summary>
        /// <param name="str">The source span to iterate.</param>
        /// <param name="c">The delimiter character used to split segments.</param>
        public SpaceSplitEnumerator(ReadOnlySpan<char> str, char c)
        {
            _str = str;
            _c = c;
            Current = default;
        }

        // Needed to be compatible with the foreach operator
        /// <summary>
        /// Returns the enumerator instance for use in <c>foreach</c> loops.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SpaceSplitEnumerator GetEnumerator() => this;

        /// <summary>
        /// Advances to the next segment in the source span.
        /// </summary>
        /// <returns><c>true</c> if a segment is available; otherwise <c>false</c>.</returns>
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

        /// <summary>
        /// Gets the current segment produced by the enumerator.
        /// </summary>
        public ReadOnlySpan<char> Current { get; private set; }
    }
}