using HeavyStringFilter.Application.Interfaces;
using System.Collections.Concurrent;

namespace HeavyStringFilter.Infrastructure.Queue;

public class InMemoryProcessingQueue : IProcessingQueue
{
    private readonly ConcurrentQueue<ProcessingTask> _queue = new();

    public void Enqueue(ProcessingTask task) => _queue.Enqueue(task);

    public bool TryDequeue(out ProcessingTask task) => _queue.TryDequeue(out task);
}
