using BeamOs.Benchmarks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

Console.WriteLine("Hello, World!");

#if DEBUG
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
#else
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
#endif

//AnalysisBench bench = new();
//for (int i = 0; i < 25; i++)
////for (int i = 0; i < 25; i++)
//{
//    bench.RunDsm();
//}
