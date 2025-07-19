using BenchmarkDotNet.Attributes;
using HeavyStringFilter.Application.Interfaces;
using HeavyStringFilter.Application.Services;
using HeavyStringFilter.Infrastructure.Filtering;
using Microsoft.Extensions.Options;

namespace HeavyStringFilter.PerformanceTests;

[MemoryDiagnoser]
public class FilterPerformanceTests
{
    private IFilterService _filterService = null!;
    private string _inputText10MB = string.Empty!;
    private string _inputText50MB = string.Empty!;
    private string _inputText100MB = string.Empty!;

    [GlobalSetup]
    public void Setup()
    {
        var config = Options.Create(new FilterConfig
        {
            FilterWords = new List<string> { "bad", "ugly", "offensive" },
            SimilarityThreshold = 85
        });

        _filterService = new FilterService(config);

        _inputText10MB = GenerateText(10);
        _inputText50MB = GenerateText(50);
        _inputText100MB = GenerateText(100);
    }

    private string GenerateText(int sizeInMB)
    {
        int wordCount = (sizeInMB * 1024 * 1024) / 6;
        var words = new string[wordCount];

        var sampleWords = new[] { "hello", "world", "badword", "uglyface", "nice", "cool", "offensiveword" };
        var rnd = new Random(42);

        for (int i = 0; i < wordCount; i++)
            words[i] = sampleWords[rnd.Next(sampleWords.Length)];

        return string.Join(' ', words);
    }

    [Benchmark(Description = "Filter 10MB Text")]
    public void Filter_10MB()
    {
        _filterService.Filter(_inputText10MB);
    }

    [Benchmark(Description = "Filter 50MB Text")]
    public void Filter_50MB()
    {
        _filterService.Filter(_inputText50MB);
    }

    [Benchmark(Description = "Filter 100MB Text")]
    public void Filter_100MB()
    {
        _filterService.Filter(_inputText100MB);
    }
}
