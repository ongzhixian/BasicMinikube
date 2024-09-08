using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using SampleBenchmark;

var config = ManualConfig
    .Create(DefaultConfig.Instance)
    .AddDiagnoser(MemoryDiagnoser.Default)
    .WithSummaryStyle(new SummaryStyle(null, false, null, null, ratioStyle: RatioStyle.Percentage));

_ = BenchmarkRunner.Run<ArrayBenchmarks>(config);