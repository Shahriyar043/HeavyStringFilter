namespace HeavyStringFilter.Application.Models;

public record UploadChunkDto(
    string UploadId,
    int ChunkIndex,
    string Data,
    bool IsLastChunk
);
