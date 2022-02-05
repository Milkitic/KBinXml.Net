using System;
using System.Collections.Generic;
using System.Linq;
using KbinXml.Utils;
using static KbinXml.Utils.ConvertHelper;

namespace KbinXml.Internal;

internal static class TypeDictionary
{
    internal static readonly Dictionary<byte, NodeType> TypeMap = new()
    {
        { 2, new NodeType(1, 1, "s8", WriteS8String, S8ToString) },
        { 3, new NodeType(1, 1, "u8", WriteU8String, U8ToString) },
        { 4, new NodeType(2, 1, "s16", WriteS16String, S16ToString) },
        { 5, new NodeType(2, 1, "u16", WriteU16String, U16ToString) },
        { 6, new NodeType(4, 1, "s32", WriteS32String, S32ToString) },
        { 7, new NodeType(4, 1, "u32", WriteU32String, U32ToString) },
        { 8, new NodeType(8, 1, "s64", WriteS64String, S64ToString) },
        { 9, new NodeType(8, 1, "u64", WriteU64String, U64ToString) },
        { 10, new NodeType(0, 0, "bin", ThrowExceptionConvert, ThrowExceptionConvert) },
        { 11, new NodeType(0, 0, "str", ThrowExceptionConvert, ThrowExceptionConvert) },
        { 12, new NodeType(4, 1, "ip4", WriteIp4String, Ip4ToString) },
        { 13, new NodeType(4, 1, "time", WriteU32String, U32ToString) },
        { 14, new NodeType(4, 1, "float", WriteSingleString, SingleToString) },
        { 15, new NodeType(8, 1, "double", WriteDoubleString, DoubleToString) },
        { 16, new NodeType(1, 2, "2s8", WriteS8String, S8ToString) },
        { 17, new NodeType(1, 2, "2u8", WriteU8String, U8ToString) },
        { 18, new NodeType(2, 2, "2s16", WriteS16String, S16ToString) },
        { 19, new NodeType(2, 2, "2u16", WriteU16String, U16ToString) },
        { 20, new NodeType(4, 2, "2s32", WriteS32String, S32ToString) },
        { 21, new NodeType(4, 2, "2u32", WriteU32String, U32ToString) },
        { 22, new NodeType(8, 2, "vs64", WriteS64String, S64ToString) },
        { 23, new NodeType(8, 2, "vu64", WriteU64String, U64ToString) },
        { 24, new NodeType(4, 2, "2f", WriteSingleString, SingleToString) },
        { 25, new NodeType(8, 2, "vd", WriteDoubleString, DoubleToString) },
        { 26, new NodeType(1, 3, "3s8", WriteS8String, S8ToString) },
        { 27, new NodeType(1, 3, "3u8", WriteU8String, U8ToString) },
        { 28, new NodeType(2, 3, "3s16", WriteS16String, S16ToString) },
        { 29, new NodeType(2, 3, "3u16", WriteU16String, U16ToString) },
        { 30, new NodeType(4, 3, "3s32", WriteS32String, S32ToString) },
        { 31, new NodeType(4, 3, "3u32", WriteU32String, U32ToString) },
        { 32, new NodeType(8, 3, "3s64", WriteS64String, S64ToString) },
        { 33, new NodeType(8, 3, "3u64", WriteU64String, U64ToString) },
        { 34, new NodeType(4, 3, "3f", WriteSingleString, SingleToString) },
        { 35, new NodeType(8, 3, "3d", WriteDoubleString, DoubleToString) },
        { 36, new NodeType(1, 4, "4s8", WriteS8String, S8ToString) },
        { 37, new NodeType(1, 4, "4u8", WriteU8String, U8ToString) },
        { 38, new NodeType(2, 4, "4s16", WriteS16String, S16ToString) },
        { 39, new NodeType(2, 4, "4u16", WriteU16String, U16ToString) },
        { 40, new NodeType(4, 4, "vs32", WriteS32String, S32ToString) },
        { 41, new NodeType(4, 4, "vu32", WriteU32String, U32ToString) },
        { 42, new NodeType(8, 4, "4s64", WriteS64String, S64ToString) },
        { 43, new NodeType(8, 4, "4u64", WriteU64String, U64ToString) },
        { 44, new NodeType(4, 4, "vf", WriteSingleString, SingleToString) },
        { 45, new NodeType(8, 4, "4d", WriteDoubleString, DoubleToString) },
        { 48, new NodeType(1, 16, "vs8", WriteS8String, S8ToString) },
        { 49, new NodeType(1, 16, "vu8", WriteU8String, U8ToString) },
        { 50, new NodeType(2, 8, "vs16", WriteS16String, S16ToString) },
        { 51, new NodeType(2, 8, "vu16", WriteU16String, U16ToString) },
        { 52, new NodeType(1, 1, "bool", WriteU8String, U8ToString) },
        { 53, new NodeType(1, 2, "2b", WriteU8String, U8ToString) },
        { 54, new NodeType(1, 3, "3b", WriteU8String, U8ToString) },
        { 55, new NodeType(1, 4, "4b", WriteU8String, U8ToString) },
        { 56, new NodeType(1, 16, "vb", WriteU8String, U8ToString) },
    };

    internal static readonly Dictionary<string, byte> ReverseTypeMap = TypeMap.ToDictionary(x => x.Value.Name, x => x.Key);

    /// <summary>
    /// Get an instance of a <see cref="NodeType"/> from the internal type map.
    /// </summary>
    /// <param name="name">The name of the type.</param>
    /// <returns>The found type.</returns>
    public static NodeType GetType(string name)
    {
        return TypeMap[ReverseTypeMap[name]];
    }

    private static string ThrowExceptionConvert(ReadOnlySpan<byte> c) => throw new NotSupportedException();
    private static int ThrowExceptionConvert(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> c) => throw new NotSupportedException();

}