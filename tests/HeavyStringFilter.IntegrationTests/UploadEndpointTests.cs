using HeavyStringFilter.Api.Models;
using HeavyStringFilter.Application.Enums;
using HeavyStringFilter.IntegrationTests.Configuration;
using HeavyStringFilter.IntegrationTests.Helpers;
using System.Net;

namespace HeavyStringFilter.IntegrationTests;

public class UploadEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UploadEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Upload_Returns_Accepted_When_LastChunk()
    {
        // Arrange
        var request = new UploadChunkRequest
        {
            UploadId = "upload-001",
            ChunkIndex = 0,
            Data = "this is some text",
            IsLastChunk = true
        };

        // Act
        var response = await _client.PostJsonAsync("/api/upload", request);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

        var result = await response.ReadAs<UploadResponse>();
        Assert.Equal(UploadStatus.Accepted.ToString(), result?.Status);
    }
}

public class UploadResponse
{
    public string Status { get; set; } = string.Empty;
}
