using HeavyStringFilter.Application.Interfaces;
using HeavyStringFilter.Application.Services;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Text;

namespace HeavyStringFilter.Infrastructure.Filtering;

public class FilterService(IOptions<FilterConfig> options) : IFilterService
{
    private readonly FilterConfig _config = options.Value;

    public string Filter(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var sb = new StringBuilder(input.Length);
        var span = input.AsSpan();
        int wordStart = 0;

        for (int i = 0; i <= span.Length; i++)
        {
            if (i == span.Length || char.IsWhiteSpace(span[i]))
            {
                if (i > wordStart)
                {
                    var word = span[wordStart..i];
                    if (!IsSimilarToAnyFilterWord(word))
                    {
                        if (sb.Length > 0)
                            sb.Append(' ');
                        sb.Append(word);
                    }
                }
                wordStart = i + 1;
            }
        }

        return sb.ToString();
    }

    private bool IsSimilarToAnyFilterWord(ReadOnlySpan<char> word)
    {
        foreach (var filterWord in _config.FilterWords)
        {
            if (Similarity(filterWord, word) >= _config.SimilarityThreshold)
                return true;
        }
        return false;
    }

    private int Similarity(string a, ReadOnlySpan<char> b)
    {
        return (int)(JaroWinklerDistance(a.AsSpan(), b) * 100);
    }

    private double JaroWinklerDistance(ReadOnlySpan<char> s1, ReadOnlySpan<char> s2)
    {
        if (s1.Equals(s2, StringComparison.OrdinalIgnoreCase)) return 1.0;

        int len1 = s1.Length, len2 = s2.Length;
        int matchRange = Math.Max(len1, len2) / 2 - 1;

        var s1Matches = ArrayPool<bool>.Shared.Rent(len1);
        var s2Matches = ArrayPool<bool>.Shared.Rent(len2);

        try
        {
            Array.Clear(s1Matches, 0, len1);
            Array.Clear(s2Matches, 0, len2);

            int matches = 0, transpositions = 0;

            for (int i = 0; i < len1; i++)
            {
                int start = Math.Max(0, i - matchRange);
                int end = Math.Min(i + matchRange + 1, len2);

                for (int j = start; j < end; j++)
                {
                    if (s2Matches[j]) continue;
                    if (char.ToLowerInvariant(s1[i]) != char.ToLowerInvariant(s2[j])) continue;

                    s1Matches[i] = true;
                    s2Matches[j] = true;
                    matches++;
                    break;
                }
            }

            if (matches == 0) return 0.0;

            int k = 0;
            for (int i = 0; i < len1; i++)
            {
                if (!s1Matches[i]) continue;
                while (!s2Matches[k]) k++;
                if (char.ToLowerInvariant(s1[i]) != char.ToLowerInvariant(s2[k])) transpositions++;
                k++;
            }

            double m = matches;
            double jaro = (m / len1 + m / len2 + (m - transpositions / 2.0) / m) / 3.0;

            int prefix = 0;
            for (int i = 0; i < Math.Min(4, Math.Min(len1, len2)); i++)
            {
                if (char.ToLowerInvariant(s1[i]) == char.ToLowerInvariant(s2[i])) prefix++;
                else break;
            }

            return jaro + (prefix * 0.1 * (1 - jaro));
        }
        finally
        {
            ArrayPool<bool>.Shared.Return(s1Matches);
            ArrayPool<bool>.Shared.Return(s2Matches);
        }
    }
}
