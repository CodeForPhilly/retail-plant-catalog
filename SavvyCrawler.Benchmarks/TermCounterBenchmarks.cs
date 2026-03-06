using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace SavvyCrawler.Benchmarks;

/// <summary>
/// Benchmarks current TermCounter.Examine() before regex optimizations.
/// Scenarios: varying term count × content size to match real crawl usage.
/// </summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
public class TermCounterBenchmarks
{
    private string _content = "";
    private TermCounter _counter = null!;

    /// <summary>Approx content size in chars (10KB / 50KB / 100KB).</summary>
    [Params(10_000, 50_000, 100_000)]
    public int ContentLength { get; set; }

    /// <summary>Number of terms to search for (e.g. plant names).</summary>
    [Params(10, 100, 500, 1000)]
    public int TermCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _content = GenerateContent(ContentLength);
        var terms = GenerateTerms(TermCount);
        _counter = new TermCounter(terms);
    }

    /// <summary>One Examine() with pre-built TermCounter (matches production: one counter per crawl, many Examine per page).</summary>
    [Benchmark(Baseline = true)]
    public Dictionary<string, int> Examine_Current()
    {
        return _counter.Examine(_content);
    }

    private static string GenerateContent(int length)
    {
        var words = new[] { "plant", "tree", "flower", "shrub", "Japonica", "Camellia", "garden", "soil", "water", "sun", "leaf", "root", "seed", "grow", "native", "perennial", "annual" };
        var r = new Random(42);
        var sb = new System.Text.StringBuilder(length);
        while (sb.Length < length)
        {
            sb.Append(words[r.Next(words.Length)]);
            if (sb.Length < length) sb.Append(' ');
        }
        return sb.ToString();
    }

    private static string[] GenerateTerms(int count)
    {
        var terms = new List<string>();
        var r = new Random(43);
        var bases = new[] { "Japonica", "Camellia", "Pawpaw", "Strawberry", "Umbrella-Tree", "Blazing Star", "Southern Crabapple", "Wild Bergamot", "Bald Cypress", "Chalk Maple", "Allium", "Monarda" };
        for (int i = 0; i < count; i++)
            terms.Add($"{bases[r.Next(bases.Length)]} {i}");
        return terms.ToArray();
    }
}
