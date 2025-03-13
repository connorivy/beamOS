using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

Console.WriteLine("Hello, World!");

#if DEBUG
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
#else
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
#endif
