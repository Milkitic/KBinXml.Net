using System.Collections;
using System.Collections.Generic;

namespace GeneralUnitTests;

public class ByteTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1 };
        yield return new object[] { byte.MaxValue };
        yield return new object[] { 0 };
        yield return new object[] { 125 };
        yield return new object[] { 233 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class SbyteTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { sbyte.MinValue, };
        yield return new object[] { 1 };
        yield return new object[] { sbyte.MaxValue };
        yield return new object[] { 0 };
        yield return new object[] { 64 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
public class Int16TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { short.MinValue, };
        yield return new object[] { 1 };
        yield return new object[] { short.MaxValue };
        yield return new object[] { 0 };
        yield return new object[] { 5125 };
        yield return new object[] { 22535 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Int32TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { int.MinValue, };
        yield return new object[] { 1 };
        yield return new object[] { int.MaxValue };
        yield return new object[] { 0 };
        yield return new object[] { 5125 };
        yield return new object[] { 65536 };
        yield return new object[] { 512433451 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Int64TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { long.MinValue, };
        yield return new object[] { 1 };
        yield return new object[] { long.MaxValue };
        yield return new object[] { 0 };
        yield return new object[] { 5125 };
        yield return new object[] { 65536 };
        yield return new object[] { 512433451 };
        yield return new object[] { 5252112433451 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class UInt16TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1 };
        yield return new object[] { ushort.MaxValue };
        yield return new object[] { 0 };
        yield return new object[] { 5125 };
        yield return new object[] { 52535 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class UInt32TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1 };
        yield return new object[] { uint.MaxValue };
        yield return new object[] { 0 };
        yield return new object[] { 5125 };
        yield return new object[] { 65536 };
        yield return new object[] { 512433451 };
        yield return new object[] { 3012433451 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class UInt64TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1 };
        yield return new object[] { ulong.MaxValue };
        yield return new object[] { 0 };
        yield return new object[] { 5125 };
        yield return new object[] { 65536 };
        yield return new object[] { 512433451 };
        yield return new object[] { 5252112433451 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class SingleTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1f };
        yield return new object[] { float.MaxValue };
        yield return new object[] { 0f };
        yield return new object[] { 5125.72234f};
        yield return new object[] { 65536.65845f };
        yield return new object[] { 512433451.195623f };
        yield return new object[] { 3012433451.276934f };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class DoubleTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1 };
        yield return new object[] { double.MaxValue };
        yield return new object[] { 0 };
        yield return new object[] { 5125.1235432136234 };
        yield return new object[] { 65536.2304814324556 };
        yield return new object[] { 512433451.21304456075 };
        yield return new object[] { 5252112433451.2572341251235 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Ip4TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "192.168.1.1" };
        yield return new object[] { "255.255.255.0" };
        yield return new object[] { "223.5.5.5" };
        yield return new object[] { "127.0.0.1" };
        yield return new object[] { "0.0.0.0" };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}