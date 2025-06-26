using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Echo.Secs.Data;

namespace Echo.Secs.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SecsHeaderBenchmark secsHeaderBenchmark = new SecsHeaderBenchmark
            {
                LoopCount = 1_000_000 // Set the number of iterations for the benchmark
            };

            _ = SecsMessageParser.CreateHeader1(secsHeaderBenchmark.Buf);
            _ = SecsMessageParser.CreateHeader2(secsHeaderBenchmark.Buf);

            var summary = BenchmarkRunner.Run<SecsHeaderBenchmark>();

            Console.WriteLine(summary);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    [MemoryDiagnoser]
    [RankColumn]
    [DisassemblyDiagnoser(printSource: true)]
    public class SecsHeaderBenchmark
    {
        public readonly byte[] Buf = [0x00, 0x00, 0x00, 0x0A, 0x00, 0x10, 0x81, 0x03, 0x00, 0x01, 0x00, 0x00, 0x27, 0x0F];

        public int LoopCount { get; set; } = 1_000_000;

        //[Benchmark]
        //public void BenchFunc1() // 
        //{
        //    for (int a = 0; a < LoopCount; a++)
        //    {
        //        _ = SecsMessageParser.CreateHeader1(Buf);
        //    }
        //}

        [Benchmark]
        public void BenchFunc6() // 
        {
            for (int a = 0; a < LoopCount; a++)
            {
                _ = SecsMessageParser.CreateHeader6(Buf);
            }
        }

        [Benchmark]
        public void BenchFunc6a() // 
        {
            for (int a = 0; a < LoopCount; a++)
            {
                _ = SecsMessageParser.CreateHeader6a(Buf);
            }
        }
    }
}
