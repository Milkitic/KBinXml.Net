using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KbinXml.Net.Utils;

/// <summary>
/// Provides a high-performance, stack-allocated string builder that minimizes heap allocations when constructing
/// strings. Designed for scenarios where temporary string manipulation is required with minimal memory overhead.
/// </summary>
/// <remarks>ValueStringBuilder is a ref struct that enables efficient string construction by using a
/// caller-provided buffer or renting memory from the array pool. It is intended for short-lived use within a single
/// method and cannot be stored on the heap, used across await/async boundaries, or boxed. After building the desired
/// string, call ToString() to retrieve the result OR Dispose() to return any rented buffers. This type is not
/// thread-safe.</remarks>
public ref partial struct ValueStringBuilder : IDisposable
{
    private char[]? _arrayToReturnToPool;
    private Span<char> _chars;
    private int _pos;

    /// <summary>
    /// Initializes a new instance of the ValueStringBuilder struct using the specified character buffer as the initial
    /// storage.
    /// </summary>
    /// <remarks>The provided buffer is used directly and is not copied. If the builder grows beyond the size
    /// of the initial buffer, additional memory may be allocated. The caller is responsible for ensuring that the
    /// buffer remains valid for the lifetime of the ValueStringBuilder instance.</remarks>
    /// <param name="initialBuffer">The span of characters to use as the initial buffer for the builder. The buffer is used to store the characters
    /// appended to the builder until it is filled.</param>
    public ValueStringBuilder(Span<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _chars = initialBuffer;
        _pos = 0;
    }

    /// <summary>
    /// Initializes a new instance of the ValueStringBuilder class with the specified initial capacity.
    /// </summary>
    /// <remarks>A larger initial capacity can improve performance by reducing the need for internal buffer
    /// resizing as characters are appended.</remarks>
    /// <param name="initialCapacity">The minimum number of characters that the builder can initially contain. Must be greater than zero.</param>
    public ValueStringBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _chars = _arrayToReturnToPool;
        _pos = 0;
    }

    /// <summary>
    /// Gets or sets the number of characters currently contained in the buffer.
    /// </summary>
    /// <remarks>Setting this property to a value less than the current length truncates the buffer. Setting
    /// it to a value greater than the current length expands the buffer and may fill the new region with undefined
    /// data, depending on the implementation. The value must be non-negative and less than or equal to the buffer's
    /// capacity.</remarks>
    public int Length
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _chars.Length);
            _pos = value;
        }
    }

    /// <summary>
    /// Gets the total number of elements that the internal character buffer can hold without resizing.
    /// </summary>
    public int Capacity => _chars.Length;

    /// <summary>
    /// Ensures that the underlying storage is large enough to accommodate the specified capacity.
    /// </summary>
    /// <param name="capacity">The minimum number of characters that the underlying storage must be able to hold. Must be non-negative.</param>
    public void EnsureCapacity(int capacity)
    {
        // This is not expected to be called this with negative capacity
        Debug.Assert(capacity >= 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if ((uint)capacity > (uint)_chars.Length)
            Grow(capacity - _pos);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// Does not ensure there is a null char after <see cref="Length"/>
    /// This overload is pattern matched in the C# 7.3+ compiler so you can omit
    /// the explicit method call, and write eg "fixed (char* c = builder)"
    /// </summary>
    public ref char GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(_chars);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ref char GetPinnableReference(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }
        return ref MemoryMarshal.GetReference(_chars);
    }

    /// <summary>
    /// Gets a reference to the character at the specified position in the collection.
    /// </summary>
    /// <remarks>The returned reference allows direct modification of the character at the specified position.
    /// Modifying the referenced value will update the underlying collection.</remarks>
    /// <param name="index">The zero-based index of the character to retrieve. Must be greater than or equal to 0 and less than the current
    /// length of the collection.</param>
    /// <returns>A reference to the character at the specified index.</returns>
    public ref char this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _chars[index];
        }
    }

    /// <summary>
    /// Returns the current contents as a string and releases any resources used by the instance.
    /// </summary>
    /// <remarks>Calling this method disposes the instance. After calling ToString, the instance should not be
    /// used.</remarks>
    /// <returns>A string containing the characters written to the instance up to this point. Returns an empty string if no
    /// characters have been written.</returns>
    public override string ToString()
    {
        string s = _chars.Slice(0, _pos).ToString();
        Dispose();
        return s;
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<char> RawChars => _chars;

    /// <summary>
    /// Returns a span around the contents of the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ReadOnlySpan<char> AsSpan(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }
        return _chars.Slice(0, _pos);
    }

    /// <summary>
    /// Returns a read-only span containing the characters written to the buffer so far.
    /// </summary>
    /// <returns>A read-only span of characters representing the current contents of the buffer. The span length corresponds to
    /// the number of characters written.</returns>
    public ReadOnlySpan<char> AsSpan() => _chars.Slice(0, _pos);

    /// <summary>
    /// Returns a read-only span that represents the characters from the specified starting position to the current
    /// position.
    /// </summary>
    /// <param name="start">The zero-based index at which the span begins. Must be greater than or equal to 0 and less than or equal to the
    /// current position.</param>
    /// <returns>A read-only span of characters starting at the specified position and ending at the current position. The span
    /// will be empty if start equals the current position.</returns>
    public ReadOnlySpan<char> AsSpan(int start) => _chars.Slice(start, _pos - start);

    /// <summary>
    /// Returns a read-only span that represents a substring of the current character sequence, starting at the
    /// specified position and having the specified length.
    /// </summary>
    /// <param name="start">The zero-based index at which the span begins. Must be greater than or equal to 0 and less than or equal to the
    /// length of the character sequence.</param>
    /// <param name="length">The number of characters to include in the span. Must be greater than or equal to 0 and start + length must not
    /// exceed the length of the character sequence.</param>
    /// <returns>A read-only span of characters that starts at the specified position and has the specified length.</returns>
    public ReadOnlySpan<char> AsSpan(int start, int length) => _chars.Slice(start, length);

    /// <summary>
    /// Copies the current contents into the provided destination and disposes this instance.
    /// </summary>
    /// <param name="destination">The target buffer to receive the characters.</param>
    /// <param name="charsWritten">On success, receives the number of characters written.</param>
    /// <returns><c>true</c> if the contents fit in <paramref name="destination"/>; otherwise <c>false</c>.</returns>
    public bool TryCopyTo(Span<char> destination, out int charsWritten)
    {
        if (_chars.Slice(0, _pos).TryCopyTo(destination))
        {
            charsWritten = _pos;
            Dispose();
            return true;
        }
        else
        {
            charsWritten = 0;
            Dispose();
            return false;
        }
    }

    /// <summary>
    /// Inserts <paramref name="count"/> copies of <paramref name="value"/> at the specified index.
    /// </summary>
    /// <param name="index">The position at which to insert.</param>
    /// <param name="value">The character to insert.</param>
    /// <param name="count">The number of times the character is inserted.</param>
    public void Insert(int index, char value, int count)
    {
        if (_pos > _chars.Length - count)
        {
            Grow(count);
        }

        int remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
        _chars.Slice(index, count).Fill(value);
        _pos += count;
    }

    /// <summary>
    /// Inserts the specified string at the given index.
    /// </summary>
    /// <param name="index">The position at which to insert.</param>
    /// <param name="s">The string to insert. If <c>null</c>, no action is taken.</param>
    public void Insert(int index, string? s)
    {
        if (s == null)
        {
            return;
        }

        int count = s.Length;

        if (_pos > (_chars.Length - count))
        {
            Grow(count);
        }

        int remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
        s
#if !NET8_0_OR_GREATER
            .AsSpan()
#endif
            .CopyTo(_chars.Slice(index));
        _pos += count;
    }

    /// <summary>
    /// Appends a single character to the end of the builder.
    /// </summary>
    /// <param name="c">The character to append.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        int pos = _pos;
        if ((uint)pos < (uint)_chars.Length)
        {
            _chars[pos] = c;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    /// <summary>
    /// Appends a string to the end of the builder.
    /// </summary>
    /// <param name="s">The string to append. If <c>null</c>, no action is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? s)
    {
        if (s == null)
        {
            return;
        }

        int pos = _pos;
        if (s.Length == 1 && (uint)pos < (uint)_chars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
        {
            _chars[pos] = s[0];
            _pos = pos + 1;
        }
        else
        {
            AppendSlow(s);
        }
    }

    private void AppendSlow(string s)
    {
        int pos = _pos;
        if (pos > _chars.Length - s.Length)
        {
            Grow(s.Length);
        }

        s
#if !NETCOREAPP
            .AsSpan()
#endif
            .CopyTo(_chars.Slice(pos));
        _pos += s.Length;
    }

    /// <summary>
    /// Appends <paramref name="count"/> copies of <paramref name="c"/> to the builder.
    /// </summary>
    /// <param name="c">The character to repeat.</param>
    /// <param name="count">The number of times to append the character.</param>
    public void Append(char c, int count)
    {
        if (_pos > _chars.Length - count)
        {
            Grow(count);
        }

        Span<char> dst = _chars.Slice(_pos, count);
        for (int i = 0; i < dst.Length; i++)
        {
            dst[i] = c;
        }
        _pos += count;
    }

    /// <summary>
    /// Appends characters from an unmanaged memory buffer.
    /// </summary>
    /// <param name="value">A pointer to the first character to append.</param>
    /// <param name="length">The number of characters to append.</param>
    public unsafe void Append(char* value, int length)
    {
        int pos = _pos;
        if (pos > _chars.Length - length)
        {
            Grow(length);
        }

        Span<char> dst = _chars.Slice(_pos, length);
        for (int i = 0; i < dst.Length; i++)
        {
            dst[i] = *value++;
        }
        _pos += length;
    }

    /// <summary>
    /// Appends the specified read-only span of characters to the builder.
    /// </summary>
    /// <param name="value">The characters to append.</param>
    public void Append(scoped ReadOnlySpan<char> value)
    {
        int pos = _pos;
        if (pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_chars.Slice(_pos));
        _pos += value.Length;
    }

    /// <summary>
    /// Reserves space for <paramref name="length"/> characters and returns a writable span for the caller to fill.
    /// </summary>
    /// <param name="length">The number of characters to reserve.</param>
    /// <returns>A Span&lt;char&gt; backed by the builder for writing.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        int origPos = _pos;
        if (origPos > _chars.Length - length)
        {
            Grow(length);
        }

        _pos = origPos + length;
        return _chars.Slice(origPos, length);
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Appends a value that implements <see cref="ISpanFormattable"/> using the provided format and provider.
    /// </summary>
    /// <typeparam name="T">The value type implementing <see cref="ISpanFormattable"/>.</typeparam>
    /// <param name="value">The value to format and append.</param>
    /// <param name="format">An optional format string.</param>
    /// <param name="provider">An optional format provider.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendSpanFormattable<T>(T value, string? format = null, IFormatProvider? provider = null)
        where T : ISpanFormattable
    {
        if (value.TryFormat(_chars.Slice(_pos), out int charsWritten, format, provider))
        {
            _pos += charsWritten;
        }
        else
        {
            Append(value.ToString(format, provider));
        }
    }
#endif

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);
        Append(c);
    }

    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative
        char[] poolArray = ArrayPool<char>.Shared.Rent((int)Math.Max((uint)(_pos + additionalCapacityBeyondPos), (uint)_chars.Length * 2));

        _chars.Slice(0, _pos).CopyTo(poolArray);

        char[]? toReturn = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = poolArray;
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    /// <summary>
    /// Returns any rented buffer to the pool and resets this instance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        char[]? toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }
}