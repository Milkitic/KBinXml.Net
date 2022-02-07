using System;
using System.Collections.Generic;
using System.Threading;

namespace WriteBenchmark
{
    internal static class MultiThreadUtils
    {
        public static List<object?>[] DoMultiThreadWork(Func<int, object?> func, int threadCount, int iterPerThread = 1)
        {
            var events = new ManualResetEvent[threadCount];
            for (var i = 0; i < events.Length; i++) events[i] = new ManualResetEvent(false);

            var objs = new List<object?>[threadCount];
            for (var i = 0; i < objs.Length; i++) objs[i] = new List<object?>();

            var max = Environment.ProcessorCount;
            ThreadPool.SetMaxThreads(max, max);

            for (int i = 0; i < threadCount; i++)
            {
                var j = i;
                ThreadPool.QueueUserWorkItem((_) =>
                {
                    for (int k = 0; k < iterPerThread; k++)
                    {
                        objs[j].Add(func(j));
                    }

                    events[j].Set();
                    //Console.WriteLine($"#{j} finished");
                });
            }

            WaitHandle.WaitAll(events);
            return objs;
        }
    }
}
