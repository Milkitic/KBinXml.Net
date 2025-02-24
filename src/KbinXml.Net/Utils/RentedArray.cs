using System;
using System.Buffers;

namespace KbinXml.Net.Utils;

public readonly ref struct RentedArray<T> : IDisposable
{
    public Span<T> Span { get; }
    private readonly ArrayPool<T> _pool;
    private readonly T[]? _array;

    public RentedArray(ArrayPool<T> pool, int minimumLength)
    {
        _pool = pool;
        _array = _pool.Rent(minimumLength);
        Span = _array.AsSpan(0, minimumLength);
    }

    public void Dispose() => _pool.Return(_array!);
}