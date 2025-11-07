using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using KbinXml.Net;
using Microsoft.IO;
using DataWriter = KbinXml.Net.Internal.Writers.DataWriter;

namespace UnitBenchmarks;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.Net10_0)]
//[SimpleJob(RuntimeMoniker.Net48)]
public class DataWriterStringBenchmark
{
    private RecyclableMemoryStream _stream = null!;
    private string[] _array = null!;

    [GlobalSetup]
    public void Setup()
    {
        _stream = KbinConverter.RecyclableMemoryStreamManager.GetStream("", ushort.MaxValue);
        var str =
            """
            The C# language is the most popular language for the .NET platform, a free, cross-platform, open source development environment. C# programs can run on many different devices, from Internet of Things (IoT) devices to the cloud and everywhere in between. You can write apps for phone, desktop, and laptop computers and servers.

            C# is a cross-platform general purpose language that makes developers productive while writing highly performant code. With millions of developers, C# is the most popular .NET language. C# has broad support in the ecosystem and all .NET workloads. Based on object-oriented principles, it incorporates many features from other paradigms, not least functional programming. Low-level features support high-efficiency scenarios without writing unsafe code. Most of the .NET runtime and libraries are written in C#, and advances in C# often benefit all .NET developers.

            C# is in the C family of languages. C# syntax is familiar if you used C, C++, JavaScript, TypeScript, or Java. Like C and C++, semi-colons (;) define the end of statements. C# identifiers are case-sensitive. C# has the same use of braces, { and }, control statements like if, else and switch, and looping constructs like for, and while. C# also has a foreach statement for any collection type.
            """;
        _array = str.Split([' '], StringSplitOptions.RemoveEmptyEntries);
    }

    [Benchmark(Baseline = true)]
    public object? OldWriter()
    {
        _stream.SetLength(0);
        using var writer = new Legacy.DataWriter(Encoding.UTF8, _stream);
        for (var i = 0; i < _array.Length; i++)
        {
            var b = _array[i];
            writer.WriteString(b.AsSpan());
            _stream.SetLength(_stream.Length);
        }

        return _stream;
    }


    [Benchmark()]
    public object? NewWriter()
    {
        _stream.SetLength(0);
        using var writer = new DataWriter(Encoding.UTF8, _stream);
        for (var i = 0; i < _array.Length; i++)
        {
            var b = _array[i];
            writer.WriteString(b.AsSpan());
            _stream.SetLength(_stream.Length);
        }

        return _stream;
    }
}