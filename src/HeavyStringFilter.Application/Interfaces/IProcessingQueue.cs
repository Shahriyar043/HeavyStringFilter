namespace HeavyStringFilter.Application.Interfaces;

public interface IProcessingQueue
{
    void Enqueue(ProcessingTask task);
    bool TryDequeue(out ProcessingTask task);
}

public record ProcessingTask(string UploadId, string FullText);
