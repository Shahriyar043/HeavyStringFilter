namespace HeavyStringFilter.Application.Services;

public class FilterConfig
{
    public required List<string> FilterWords { get; set; }
    public int SimilarityThreshold { get; set; }
}