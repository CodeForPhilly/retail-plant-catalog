using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using SavvyCrawler.Benchmarks;

var config = ManualConfig.Create(DefaultConfig.Instance)
    .AddDiagnoser(MemoryDiagnoser.Default);

BenchmarkRunner.Run<TermCounterBenchmarks>(config);
