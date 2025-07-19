using HeavyStringFilter.Application.Interfaces;
using HeavyStringFilter.Application.Models;

namespace HeavyStringFilter.Application.Services;

public class UploadService(IUploadStorage storage, IProcessingQueue queue) : IUploadService
{
    public async Task StoreChunkAsync(UploadChunkDto chunkDto)
    {
        await storage.StoreChunkAsync(chunkDto.UploadId, chunkDto.ChunkIndex, chunkDto.Data);
    }

    public async Task EnqueueForProcessingAsync(string uploadId)
    {
        var fullText = await storage.CombineChunksAsync(uploadId);
        queue.Enqueue(new ProcessingTask(uploadId, fullText));
    }
}