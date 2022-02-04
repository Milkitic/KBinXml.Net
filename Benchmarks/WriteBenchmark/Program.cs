using System;
using System.IO;
using System.Text;
using KbinXml;

namespace WriteBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bytes = File.ReadAllBytes(@"data\test_case.bin");
            var str = KbinConverter.ReadLinq(bytes).ToString();
            var backs = KbinConverter.WriteRaw(str, Encoding.UTF8);
            Console.WriteLine("Hello World!");
        }
    }
}
