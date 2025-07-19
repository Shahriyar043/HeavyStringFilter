using System.Text;
using System.Text.Json;

namespace TestCommon.Extensions;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static StringContent ToJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, _options);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public static async Task<T?> FromJsonAsync<T>(HttpResponseMessage response)
    {
        if (response.Content == null)
            return default;

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, _options);
    }

    public static string Serialize<T>(T obj) =>
        JsonSerializer.Serialize(obj, _options);

    public static T? Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, _options);
}
