using System;
using System.Diagnostics;
using KbinXml.Net.Utils;
using Xunit;

namespace GeneralUnitTests
{
    public class DataTests
    {
        [Theory]
        [ClassData(typeof(ByteTestData))]
        public void ByteTest(byte value)
        {
            DoWorks(value, x => StableKbin.Converters.U8ToBytes(x).ToArray(),
                x => StableKbin.Converters.U8ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteU8String(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.U8ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(SbyteTestData))]
        public void SbyteTest(sbyte value)
        {
            DoWorks(value, x => StableKbin.Converters.S8ToBytes(x).ToArray(),
                x => StableKbin.Converters.S8ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteS8String(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.S8ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(Int16TestData))]
        public void Int16Test(short value)
        {
            DoWorks(value, x => StableKbin.Converters.S16ToBytes(x).ToArray(),
                x => StableKbin.Converters.S16ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteS16String(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.S16ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(Int32TestData))]
        public void Int32Test(int value)
        {
            DoWorks(value, x => StableKbin.Converters.S32ToBytes(x).ToArray(),
                x => StableKbin.Converters.S32ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteS32String(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.S32ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(Int64TestData))]
        public void Int64Test(long value)
        {
            DoWorks(value, x => StableKbin.Converters.S64ToBytes(x).ToArray(),
                x => StableKbin.Converters.S64ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteS64String(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.S64ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(UInt16TestData))]
        public void UInt16Test(ushort value)
        {
            DoWorks(value, x => StableKbin.Converters.U16ToBytes(x).ToArray(),
                x => StableKbin.Converters.U16ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteU16String(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.U16ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(UInt32TestData))]
        public void UInt32Test(uint value)
        {
            DoWorks(value, x => StableKbin.Converters.U32ToBytes(x).ToArray(),
                x => StableKbin.Converters.U32ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteU32String(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.U32ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(UInt64TestData))]
        public void UInt64Test(ulong value)
        {
            DoWorks(value, x => StableKbin.Converters.U64ToBytes(x).ToArray(),
                x => StableKbin.Converters.U64ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteU64String(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.U64ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(SingleTestData))]
        public void SingleTest(float value)
        {
            DoWorks(value, x => StableKbin.Converters.SingleToBytes(x).ToArray(),
                x => StableKbin.Converters.SingleToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteSingleString(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.SingleToString(bytes));
        }

        [Theory]
        [ClassData(typeof(DoubleTestData))]
        public void DoubleTest(double value)
        {
            DoWorks(value, x => StableKbin.Converters.DoubleToBytes(x).ToArray(),
                x => StableKbin.Converters.DoubleToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteDoubleString(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.DoubleToString(bytes));
        }

        [Theory]
        [ClassData(typeof(Ip4TestData))]
        public void Ip4Test(string value)
        {
            DoWorks(value, x => StableKbin.Converters.Ip4ToBytes(x).ToArray(),
                x => StableKbin.Converters.Ip4ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    ConvertHelper.WriteIp4String(ref builder, str);
                    return builder.AsSpan().ToArray();
                }, bytes => ConvertHelper.Ip4ToString(bytes));
        }

        private static void DoWorks(object value,
            Func<string, byte[]> toBytesOld,
            Func<byte[], string> toStringOld,
            Func<string, byte[]> toBytesNew,
            Func<byte[], string> toStringNew)
        {
            var str = value.ToString();

            var bytes = toBytesOld(str);
            var bytes2 = toBytesNew(str);

            var output = toStringOld(bytes);
            var output2 = toStringNew(bytes2);

            Assert.Equal(bytes, bytes2);
            Assert.Equal(output, output2);
        }
    }
}
