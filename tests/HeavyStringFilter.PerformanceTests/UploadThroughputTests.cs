using HeavyStringFilter.Api.Models;
using HeavyStringFilter.IntegrationTests.Configuration;
using HeavyStringFilter.IntegrationTests.Helpers;
using System.Diagnostics;
using Xunit.Abstractions;

namespace HeavyStringFilter.PerformanceTests;

public class UploadThroughputTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public UploadThroughputTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task Upload_MultipleChunks_Performs_Under_Threshold(int chunkCount)
    {
        var uploadId = Guid.NewGuid().ToString("N");
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < chunkCount; i++)
        {
            var request = new UploadChunkRequest
            {
                UploadId = uploadId,
                ChunkIndex = i,
                Data = $"chunk-{i}-data",
                IsLastChunk = i == chunkCount - 1
            };

            var response = await _client.PostJsonAsync("/api/upload", request);
            response.EnsureSuccessStatusCode();
        }

        sw.Stop();
        var avg = sw.ElapsedMilliseconds / (double)chunkCount;

        _output.WriteLine($"Sent {chunkCount} chunks in {sw.ElapsedMilliseconds} ms (avg: {avg:F2} ms per chunk)");

        Assert.InRange(avg, 0, 100);
    }
}
