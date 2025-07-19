using HeavyStringFilter.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace HeavyStringFilter.Infrastructure.Storage;

public class InMemoryUploadStorage(ILogger<InMemoryUploadStorage> logger) : IUploadStorage
{
    private readonly ConcurrentDictionary<string, SortedDictionary<int, string>> _chunks = new();

    public Task StoreChunkAsync(string uploadId, int chunkIndex, string data)
    {
        var list = _chunks.GetOrAdd(uploadId, _ => new SortedDictionary<int, string>());
        lock (list)
        {
            list[chunkIndex] = data;
        }

        logger.LogInformation("Stored chunk #{ChunkIndex} for UploadId '{UploadId}' (Length: {Length})",
           chunkIndex, uploadId, data.Length);
        return Task.CompletedTask;
    }

    public Task<string> CombineChunksAsync(string uploadId)
    {
        if (!_chunks.TryGetValue(uploadId, out var chunks))
        {
            logger.LogError("UploadId '{UploadId}' not found during chunk combination", uploadId);
            throw new InvalidOperationException($"UploadId '{uploadId}' not found");
        }

        var totalChunks = chunks.Count;
        var fullText = string.Join(" ", chunks.OrderBy(x => x.Key).Select(x => x.Value));

        logger.LogInformation("Combined {ChunkCount} chunks for UploadId '{UploadId}' (Total Length: {Length})",
            totalChunks, uploadId, fullText.Length);

        _chunks.TryRemove(uploadId, out _);

        return Task.FromResult(fullText);
    }
}