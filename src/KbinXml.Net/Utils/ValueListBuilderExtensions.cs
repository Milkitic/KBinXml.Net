using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using KbinXml.Net.Internal;

namespace KbinXml.Net.Utils;

public static class ValueListBuilderExtensions
{
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