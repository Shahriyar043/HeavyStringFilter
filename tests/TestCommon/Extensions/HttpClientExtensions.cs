using System.Text;
using System.Text.Json;

namespace HeavyStringFilter.IntegrationTests.Helpers;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static Task<HttpResponseMessage> PostJsonAsync<T>(
        this HttpClient client,
        string url,
        T data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return client.PostAsync(url, content);
    }

    public static async Task<T?> GetJsonAsync<T>(
        this HttpClient client,
        string url)
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);
    }

    public static async Task<T?> ReadAs<T>(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);
    }
}
