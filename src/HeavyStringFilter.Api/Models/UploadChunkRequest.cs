namespace HeavyStringFilter.Api.Models;

public class UploadChunkRequest
{
    public string UploadId { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public string Data { get; set; } = string.Empty;
    public bool IsLastChunk { get; set; }
}