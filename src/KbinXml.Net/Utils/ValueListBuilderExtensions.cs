using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using KbinXml.Net.Internal;

namespace KbinXml.Net.Utils;

/// <summary>
/// Extension methods for <see cref="ValueListBuilder{T}"/> to assist with
/// efficient bulk operations using stack-allocated or pooled buffers.
/// </summary>
public static class ValueListBuilderExtensions
{
    /// <summary>
    /// Appends <paramref name="count"/> zero-initialized elements to the builder.
    /// </summary>
    /// <typeparam name="T">An unmanaged element type.</typeparam>
    /// <param name="valueListBuilder">The target builder to append to.</param>
    /// <param name="count">The number of zero elements to append.</param>
    /// <remarks>
    /// Uses stack allocation for small counts and rents a buffer from the
    /// shared <see cref="ArrayPool{T}"/> for larger counts to avoid heap allocations.
    /// The rented buffer is returned to the pool after use.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendZeros<T>(this ref ValueListBuilder<T> valueListBuilder, int count) where T : unmanaged
    {
        // 避免小数组的堆分配
        T[]? arr = null;
        var span = count <= Constants.MaxStackLength
            ? stackalloc T[count]
            : (arr = ArrayPool<T>.Shared.Rent(count)).AsSpan(0, count);

        try
        {
            if (arr != null) span.Clear();
            valueListBuilder.AppendSpan(span);
        }
        finally
        {
            if (arr != null) ArrayPool<T>.Shared.Return(arr);
        }
    }
}