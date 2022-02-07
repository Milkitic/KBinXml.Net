using System.Threading;
using BenchmarkDotNet.Running;

namespace WriteBenchmark;

internal class Program
{
    static void Main(string[] args)
    {
        MultiThreadUtils.DoMultiThreadWork(i =>
        {
            var spin = new SpinWait();
            for (int j = 0; j < 100; j++)
            {
                spin.SpinOnce();
            }

            return null;
        }, 32, log: true);
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}