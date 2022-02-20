using System.Collections.Generic;
using System.Linq;
using static StableKbin.Converters;

namespace StableKbin
{
    internal static class TypeDictionary
    {
        internal static readonly Dictionary<byte, NodeType> TypeMap = new Dictionary<byte, NodeType>()
        {
            {2, new NodeType(1, 1, "s8", S8ToBytes, S8ToString) },
            {3, new NodeType(1, 1, "u8", U8ToBytes, U8ToString) },
            {4, new NodeType(2, 1, "s16", S16ToBytes, S16ToString) },
            {5, new NodeType(2, 1, "u16", U16ToBytes, U16ToString) },
            {6, new NodeType(4, 1, "s32", S32ToBytes, S32ToString) },
            {7, new NodeType(4, 1, "u32", U32ToBytes, U32ToString) },
            {8, new NodeType(8, 1, "s64", S64ToBytes, S64ToString) },
            {9, new NodeType(8, 1, "u64", U64ToBytes, U64ToString) },
            {10, new NodeType(0, 0, "bin", null, null) },
            {11, new NodeType(0, 0, "str", null, null) },
            {12, new NodeType(4, 1, "ip4", Ip4ToBytes, Ip4ToString) },
            {13, new NodeType(4, 1, "time", U32ToBytes, U32ToString) },
            {14, new NodeType(4, 1, "float", SingleToBytes, SingleToString) },
            {15, new NodeType(8, 1, "double", DoubleToBytes, DoubleToString) },
            {16, new NodeType(1, 2, "2s8", S8ToBytes, S8ToString) },
            {17, new NodeType(1, 2, "2u8", U8ToBytes, U8ToString) },
            {18, new NodeType(2, 2, "2s16", S16ToBytes, S16ToString) },
            {19, new NodeType(2, 2, "2u16", U16ToBytes, U16ToString) },
            {20, new NodeType(4, 2, "2s32", S32ToBytes, S32ToString) },
            {21, new NodeType(4, 2, "2u32", U32ToBytes, U32ToString) },
            {22, new NodeType(8, 2, "vs64", S64ToBytes, S64ToString) },
            {23, new NodeType(8, 2, "vu64", U64ToBytes, U64ToString) },
            {24, new NodeType(4, 2, "2f", SingleToBytes, SingleToString) },
            {25, new NodeType(8, 2, "vd", DoubleToBytes, DoubleToString) },
            {26, new NodeType(1, 3, "3s8", S8ToBytes, S8ToString) },
            {27, new NodeType(1, 3, "3u8", U8ToBytes, U8ToString) },
            {28, new NodeType(2, 3, "3s16", S16ToBytes, S16ToString) },
            {29, new NodeType(2, 3, "3u16", U16ToBytes, U16ToString) },
            {30, new NodeType(4, 3, "3s32", S32ToBytes, S32ToString) },
            {31, new NodeType(4, 3, "3u32", U32ToBytes, U32ToString) },
            {32, new NodeType(8, 3, "3s64", S64ToBytes, S64ToString) },
            {33, new NodeType(8, 3, "3u64", U64ToBytes, U64ToString) },
            {34, new NodeType(4, 3, "3f", SingleToBytes, SingleToString) },
            {35, new NodeType(8, 3, "3d", DoubleToBytes, DoubleToString) },
            {36, new NodeType(1, 4, "4s8", S8ToBytes, S8ToString) },
            {37, new NodeType(1, 4, "4u8", U8ToBytes, U8ToString) },
            {38, new NodeType(2, 4, "4s16", S16ToBytes, S16ToString) },
            {39, new NodeType(2, 4, "4u16", U16ToBytes, U16ToString) },
            {40, new NodeType(4, 4, "vs32", S32ToBytes, S32ToString) },
            {41, new NodeType(4, 4, "vu32", U32ToBytes, U32ToString) },
            {42, new NodeType(8, 4, "4s64", S64ToBytes, S64ToString) },
            {43, new NodeType(8, 4, "4u64", U64ToBytes, U64ToString) },
            {44, new NodeType(4, 4, "vf", SingleToBytes, SingleToString) },
            {45, new NodeType(8, 4, "4d", DoubleToBytes, DoubleToString) },
            {48, new NodeType(1, 16, "vs8", S8ToBytes, S8ToString) },
            {49, new NodeType(1, 16, "vu8", U8ToBytes, U8ToString) },
            {50, new NodeType(2, 8, "vs16", S16ToBytes, S16ToString) },
            {51, new NodeType(2, 8, "vu16", U16ToBytes, U16ToString) },
            {52, new NodeType(1, 1, "bool", U8ToBytes, U8ToString) },
            {53, new NodeType(1, 2, "2b", U8ToBytes, U8ToString) },
            {54, new NodeType(1, 3, "3b", U8ToBytes, U8ToString) },
            {55, new NodeType(1, 4, "4b", U8ToBytes, U8ToString) },
            {56, new NodeType(1, 16, "vb", U8ToBytes, U8ToString) },
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
    }
}
