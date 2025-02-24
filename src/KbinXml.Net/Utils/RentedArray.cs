using System;
using System.Buffers;

namespace KbinXml.Net.Utils;

public readonly ref struct RentedArray<T> : IDisposable
{
    public T[] Array { get; }
    private readonly ArrayPool<T> _pool;

    public RentedArray(ArrayPool<T> pool, int minimumLength)
    {
        _pool = pool;
        Array = _pool.Rent(minimumLength);
    }

    public void Dispose() => _pool.Return(Array);
}
