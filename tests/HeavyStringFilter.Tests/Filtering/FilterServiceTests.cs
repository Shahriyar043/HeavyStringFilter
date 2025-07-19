using HeavyStringFilter.Application.Interfaces;
using HeavyStringFilter.Application.Services;
using HeavyStringFilter.Infrastructure.Filtering;
using Microsoft.Extensions.Options;

namespace HeavyStringFilter.Tests.Filtering;

public class FilterServiceTests
{
    private IFilterService CreateService(IEnumerable<string> filterWords, int threshold)
    {
        var config = new FilterConfig
        {
            FilterWords = filterWords.ToList(),
            SimilarityThreshold = threshold
        };

        return new FilterService(Options.Create(config));
    }

    [Fact]
    public void Filters_Exact_Match()
    {
        var service = CreateService(["badword"], 80);
        var input = "this is a badword in sentence";
        var expected = "this is a in sentence";

        var result = service.Filter(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Filters_Similar_Word_Above_Threshold()
    {
        var service = CreateService(["badword"], 80);
        var input = "this is a baadword in sentence";
        var expected = "this is a in sentence";

        var result = service.Filter(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Keeps_Word_Below_Threshold()
    {
        var service = CreateService(["badword"], 97);
        var input = "this is a baadword in sentence";
        var expected = "this is a baadword in sentence";

        var result = service.Filter(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Does_Not_Filter_Different_Words()
    {
        var service = CreateService(["xxx"], 80);
        var input = "this is safe content";
        var expected = "this is safe content";

        var result = service.Filter(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Returns_Empty_For_Empty_String()
    {
        var service = CreateService(["anything"], 80);
        var input = "";
        var expected = "";

        var result = service.Filter(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Returns_Empty_For_Whitespace_Only()
    {
        var service = CreateService(["abc"], 80);
        var input = "    ";
        var expected = "";

        var result = service.Filter(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Is_Case_Insensitive()
    {
        var service = CreateService(["BadWord"], 80);
        var input = "this BADWORD is badWord";
        var expected = "this is";

        var result = service.Filter(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Filters_Multiple_Matching_Words()
    {
        var service = CreateService(["ban", "curse"], 80);
        var input = "this sentence has ban curse and clean";
        var expected = "this sentence has and clean";

        var result = service.Filter(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Works_With_Single_Word_Input()
    {
        var service = CreateService(["forbidden"], 80);

        var resultMatch = service.Filter("forbidden");
        var resultNoMatch = service.Filter("allowed");

        Assert.Equal("", resultMatch);
        Assert.Equal("allowed", resultNoMatch);
    }
}
