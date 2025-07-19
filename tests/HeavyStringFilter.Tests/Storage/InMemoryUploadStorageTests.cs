using HeavyStringFilter.Application.Interfaces;
using HeavyStringFilter.Infrastructure.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace HeavyStringFilter.Tests.Storage;

public class InMemoryUploadStorageTests
{
    private readonly Mock<ILogger<InMemoryUploadStorage>> _mockLogger;
    private readonly IUploadStorage _storage;

    public InMemoryUploadStorageTests()
    {
        _mockLogger = new Mock<ILogger<InMemoryUploadStorage>>();
        _storage = new InMemoryUploadStorage(_mockLogger.Object);
    }

    [Fact]
    public async Task StoreChunkAsync_SavesChunkCorrectly()
    {
        var uploadId = "test1";

        await _storage.StoreChunkAsync(uploadId, 0, "hello");
        await _storage.StoreChunkAsync(uploadId, 1, "world");

        var result = await _storage.CombineChunksAsync(uploadId);

        Assert.Equal("hello world", result);
    }

    [Fact]
    public async Task CombineChunksAsync_ReturnsChunksInCorrectOrder()
    {
        var uploadId = "order-test";

        await _storage.StoreChunkAsync(uploadId, 1, "world");
        await _storage.StoreChunkAsync(uploadId, 0, "hello");

        var result = await _storage.CombineChunksAsync(uploadId);

        Assert.Equal("hello world", result);
    }

    [Fact]
    public async Task CombineChunksAsync_Throws_When_UploadId_Not_Found()
    {
        var missingId = "missing-id";

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _storage.CombineChunksAsync(missingId));

        Assert.Equal($"UploadId '{missingId}' not found", ex.Message);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("UploadId 'missing-id' not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StoreChunkAsync_Overwrites_When_SameIndexUsed()
    {
        var uploadId = "overwrite-test";

        await _storage.StoreChunkAsync(uploadId, 0, "first ");
        await _storage.StoreChunkAsync(uploadId, 0, "second ");

        var result = await _storage.CombineChunksAsync(uploadId);

        Assert.Equal("second ", result);
    }

    [Fact]
    public async Task Logs_Chunk_Storage_And_Combination()
    {
        var uploadId = "log-test";

        await _storage.StoreChunkAsync(uploadId, 0, "part1");
        await _storage.StoreChunkAsync(uploadId, 1, "part2");

        var result = await _storage.CombineChunksAsync(uploadId);

        Assert.Equal("part1 part2", result);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Stored chunk #0")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Combined")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
