using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Internal;

internal ref struct RefValueList<T> : IDisposable
{
    private Span<T> _span;
    private T[]? _arrayFromPool;
    private int _pos;

    public RefValueList(Span<T> initialSpan, int pos = 0)
    {
        _span = initialSpan;
        _arrayFromPool = null;
        _pos = pos;
    }

    public RefValueList(int minimumLength = 256)
    {
        T[] array = ArrayPool<T>.Shared.Rent(minimumLength);
        _span = _arrayFromPool = array;
    }

    public int Count
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _span.Length);
            _pos = value;
        }
    }

    public ref T this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _span[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        int pos = _pos;
        if (pos >= _span.Length)
            Grow();

        _span[pos] = item;
        _pos = pos + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendSpan(scoped ReadOnlySpan<T> items)
    {
        int currentPos = _pos;
        int itemsLength = items.Length;

        // Check if we need to grow the buffer
        if (currentPos + itemsLength > _span.Length)
        {
            // Calculate the new size ensuring it can fit all the new items
            int newSize = Math.Max(_span.Length * 2, currentPos + itemsLength);
            T[] array = ArrayPool<T>.Shared.Rent(newSize);

            bool success = _span.TryCopyTo(array);
            Debug.Assert(success);

            T[]? toReturn = _arrayFromPool;
            _span = _arrayFromPool = array;

            if (toReturn != null)
            {
                ArrayPool<T>.Shared.Return(toReturn);
            }
        }

        // Copy the items to the span
        items.CopyTo(_span.Slice(currentPos));
        _pos = currentPos + itemsLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan()
    {
        return _span.Slice(0, _pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpanUnsafe()
    {
        return _span.Slice(0, _pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        _span.Clear();
        _pos = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        T[]? toReturn = _arrayFromPool;
        if (toReturn != null)
        {
            _arrayFromPool = null;
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Grow()
    {
        T[] array = ArrayPool<T>.Shared.Rent(_span.Length * 2);

        bool success = _span.TryCopyTo(array);
        Debug.Assert(success);

        T[]? toReturn = _arrayFromPool;
        _span = _arrayFromPool = array;
        if (toReturn != null)
        {
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }
}