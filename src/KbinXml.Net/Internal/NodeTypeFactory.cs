using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using KbinXml.Net.Internal.TypeConverters;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#else
using System.Linq;
#endif

namespace KbinXml.Net.Internal;

internal static class NodeTypeFactory
{
    // @formatter:off
#pragma warning disable Format 
    private static readonly IReadOnlyDictionary<byte, NodeType> NodesDictionary = new Dictionary<byte, NodeType>
    {
        { 2,  new NodeType(1, 1,  "s8",     S8Converter.Instance       ) },
        { 3,  new NodeType(1, 1,  "u8",     U8Converter.Instance       ) },
        { 4,  new NodeType(2, 1,  "s16",    S16Converter.Instance      ) },
        { 5,  new NodeType(2, 1,  "u16",    U16Converter.Instance      ) },
        { 6,  new NodeType(4, 1,  "s32",    S32Converter.Instance      ) },
        { 7,  new NodeType(4, 1,  "u32",    U32Converter.Instance      ) },
        { 8,  new NodeType(8, 1,  "s64",    S64Converter.Instance      ) },
        { 9,  new NodeType(8, 1,  "u64",    U64Converter.Instance      ) },
        { 10, new NodeType(0, 0,  "bin",    DummyBinConverter.Instance ) },
        { 11, new NodeType(0, 0,  "str",    DummyStrConverter.Instance ) },
        { 12, new NodeType(4, 1,  "ip4",    Ip4Converter.Instance      ) },
        { 13, new NodeType(4, 1,  "time",   U32Converter.Instance      ) },
        { 14, new NodeType(4, 1,  "float",  FloatConverter.Instance    ) },
        { 15, new NodeType(8, 1,  "double", DoubleConverter.Instance   ) },

        { 16, new NodeType(1, 2,  "2s8",    S8Converter.Instance       ) },
        { 17, new NodeType(1, 2,  "2u8",    U8Converter.Instance       ) },
        { 18, new NodeType(2, 2,  "2s16",   S16Converter.Instance      ) },
        { 19, new NodeType(2, 2,  "2u16",   U16Converter.Instance      ) },
        { 20, new NodeType(4, 2,  "2s32",   S32Converter.Instance      ) },
        { 21, new NodeType(4, 2,  "2u32",   U32Converter.Instance      ) },
        { 22, new NodeType(8, 2,  "vs64",   S64Converter.Instance      ) },
        { 23, new NodeType(8, 2,  "vu64",   U64Converter.Instance      ) },
        { 24, new NodeType(4, 2,  "2f",     FloatConverter.Instance    ) },
        { 25, new NodeType(8, 2,  "vd",     DoubleConverter.Instance   ) },

        { 26, new NodeType(1, 3,  "3s8",    S8Converter.Instance       ) },
        { 27, new NodeType(1, 3,  "3u8",    U8Converter.Instance       ) },
        { 28, new NodeType(2, 3,  "3s16",   S16Converter.Instance      ) },
        { 29, new NodeType(2, 3,  "3u16",   U16Converter.Instance      ) },
        { 30, new NodeType(4, 3,  "3s32",   S32Converter.Instance      ) },
        { 31, new NodeType(4, 3,  "3u32",   U32Converter.Instance      ) },
        { 32, new NodeType(8, 3,  "3s64",   S64Converter.Instance      ) },
        { 33, new NodeType(8, 3,  "3u64",   U64Converter.Instance      ) },
        { 34, new NodeType(4, 3,  "3f",     FloatConverter.Instance    ) },
        { 35, new NodeType(8, 3,  "3d",     DoubleConverter.Instance   ) },

        { 36, new NodeType(1, 4,  "4s8",    S8Converter.Instance       ) },
        { 37, new NodeType(1, 4,  "4u8",    U8Converter.Instance       ) },
        { 38, new NodeType(2, 4,  "4s16",   S16Converter.Instance      ) },
        { 39, new NodeType(2, 4,  "4u16",   U16Converter.Instance      ) },
        { 40, new NodeType(4, 4,  "vs32",   S32Converter.Instance      ) },
        { 41, new NodeType(4, 4,  "vu32",   U32Converter.Instance      ) },
        { 42, new NodeType(8, 4,  "4s64",   S64Converter.Instance      ) },
        { 43, new NodeType(8, 4,  "4u64",   U64Converter.Instance      ) },
        { 44, new NodeType(4, 4,  "vf",     FloatConverter.Instance    ) },
        { 45, new NodeType(8, 4,  "4d",     DoubleConverter.Instance   ) },

        { 48, new NodeType(1, 16, "vs8",    S8Converter.Instance       ) },
        { 49, new NodeType(1, 16, "vu8",    U8Converter.Instance       ) },
        { 50, new NodeType(2, 8,  "vs16",   S16Converter.Instance      ) },
        { 51, new NodeType(2, 8,  "vu16",   U16Converter.Instance      ) },
        { 52, new NodeType(1, 1,  "bool",   U8Converter.Instance       ) },
        { 53, new NodeType(1, 2,  "2b",     U8Converter.Instance       ) },
        { 54, new NodeType(1, 3,  "3b",     U8Converter.Instance       ) },
        { 55, new NodeType(1, 4,  "4b",     U8Converter.Instance       ) },
        { 56, new NodeType(1, 16, "vb",     U8Converter.Instance       ) },
    }
#if NET8_0_OR_GREATER
    .ToFrozenDictionary()
#endif
        ;
#pragma warning restore Format
    // @formatter:on
    private static readonly NodeType?[] NodesArray = new NodeType?[57];

    static NodeTypeFactory()
    {
        foreach (var nodeType in NodesDictionary)
        {
            NodesArray[nodeType.Key] = nodeType.Value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetNodeType(byte typeCode,
#if NET6_0_OR_GREATER
        [NotNullWhen(true)]
#endif
        out NodeType? nodeType)
    {
        nodeType = NodesArray[typeCode];
        return nodeType != null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NodeType GetNodeType(byte typeCode)
    {
        var nodeType = NodesArray[typeCode];
        if (nodeType == null) throw new InvalidOperationException($"Unknown type code: {typeCode}");
        return nodeType;
    }

    /// <summary>
    /// Get an instance of a <see cref="NodeType"/> from the internal type map.
    /// </summary>
    /// <param name="name">The name of the type.</param>
    /// <returns>The found type.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NodeType GetNodeType(string name)
    {
        return GetNodeType(GetNodeTypeId(name.AsSpan()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetNodeTypeId(string name)
    {
        return GetNodeTypeId(name.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetNodeTypeId(string name, out byte typeId)
    {
        return TryGetNodeTypeId(name.AsSpan(), out typeId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetNodeTypeId(ReadOnlySpan<char> name)
    {
        return TryGetNodeTypeId(name, out var id)
            ? id
            : throw new InvalidOperationException($"Unknown type name: {name.ToString()}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetNodeTypeId(ReadOnlySpan<char> name, out byte typeId)
    {
        switch (name)
        {
            case "s8": typeId = 2; return true;
            case "u8": typeId = 3; return true;
            case "s16": typeId = 4; return true;
            case "u16": typeId = 5; return true;
            case "s32": typeId = 6; return true;
            case "u32": typeId = 7; return true;
            case "s64": typeId = 8; return true;
            case "u64": typeId = 9; return true;
            case "bin": typeId = 10; return true;
            case "str": typeId = 11; return true;
            case "ip4": typeId = 12; return true;
            case "time": typeId = 13; return true;
            case "float": typeId = 14; return true;
            case "double": typeId = 15; return true;
            case "2s8": typeId = 16; return true;
            case "2u8": typeId = 17; return true;
            case "2s16": typeId = 18; return true;
            case "2u16": typeId = 19; return true;
            case "2s32": typeId = 20; return true;
            case "2u32": typeId = 21; return true;
            case "vs64": typeId = 22; return true;
            case "vu64": typeId = 23; return true;
            case "2f": typeId = 24; return true;
            case "vd": typeId = 25; return true;
            case "3s8": typeId = 26; return true;
            case "3u8": typeId = 27; return true;
            case "3s16": typeId = 28; return true;
            case "3u16": typeId = 29; return true;
            case "3s32": typeId = 30; return true;
            case "3u32": typeId = 31; return true;
            case "3s64": typeId = 32; return true;
            case "3u64": typeId = 33; return true;
            case "3f": typeId = 34; return true;
            case "3d": typeId = 35; return true;
            case "4s8": typeId = 36; return true;
            case "4u8": typeId = 37; return true;
            case "4s16": typeId = 38; return true;
            case "4u16": typeId = 39; return true;
            case "vs32": typeId = 40; return true;
            case "vu32": typeId = 41; return true;
            case "4s64": typeId = 42; return true;
            case "4u64": typeId = 43; return true;
            case "vf": typeId = 44; return true;
            case "4d": typeId = 45; return true;
            case "vs8": typeId = 48; return true;
            case "vu8": typeId = 49; return true;
            case "vs16": typeId = 50; return true;
            case "vu16": typeId = 51; return true;
            case "bool": typeId = 52; return true;
            case "2b": typeId = 53; return true;
            case "3b": typeId = 54; return true;
            case "4b": typeId = 55; return true;
            case "vb": typeId = 56; return true;
            default:
                typeId = 0;
                return false;
        }
    }
}