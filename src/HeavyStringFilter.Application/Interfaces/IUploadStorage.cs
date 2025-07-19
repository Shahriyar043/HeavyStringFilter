namespace HeavyStringFilter.Application.Interfaces;

public interface IUploadStorage
{
    Task StoreChunkAsync(string uploadId, int chunkIndex, string data);
    Task<string> CombineChunksAsync(string uploadId);
}