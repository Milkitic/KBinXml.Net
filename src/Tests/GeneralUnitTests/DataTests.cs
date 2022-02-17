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
            DoWorks(value, kbinxmlcs.Converters.U8ToBytes,
                kbinxmlcs.Converters.U8ToString,
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
            DoWorks(value, kbinxmlcs.Converters.S8ToBytes,
                kbinxmlcs.Converters.S8ToString,
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
            DoWorks(value, kbinxmlcs.Converters.S16ToBytes,
                kbinxmlcs.Converters.S16ToString,
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
            DoWorks(value, kbinxmlcs.Converters.S32ToBytes,
                kbinxmlcs.Converters.S32ToString,
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
            DoWorks(value, kbinxmlcs.Converters.S64ToBytes,
                kbinxmlcs.Converters.S64ToString,
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
            DoWorks(value, kbinxmlcs.Converters.U16ToBytes,
                kbinxmlcs.Converters.U16ToString,
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
            DoWorks(value, kbinxmlcs.Converters.U32ToBytes,
                kbinxmlcs.Converters.U32ToString,
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
            DoWorks(value, kbinxmlcs.Converters.U64ToBytes,
                kbinxmlcs.Converters.U64ToString,
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
            DoWorks(value, kbinxmlcs.Converters.SingleToBytes,
                kbinxmlcs.Converters.SingleToString,
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
            DoWorks(value, kbinxmlcs.Converters.DoubleToBytes,
                kbinxmlcs.Converters.DoubleToString,
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
            DoWorks(value, kbinxmlcs.Converters.Ip4ToBytes,
                kbinxmlcs.Converters.Ip4ToString,
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
