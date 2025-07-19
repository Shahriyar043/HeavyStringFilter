using HeavyStringFilter.Application.Models;

namespace HeavyStringFilter.Application.Interfaces;

public interface IUploadService
{
    Task StoreChunkAsync(UploadChunkDto chunkDto);
    Task EnqueueForProcessingAsync(string uploadId);
}
