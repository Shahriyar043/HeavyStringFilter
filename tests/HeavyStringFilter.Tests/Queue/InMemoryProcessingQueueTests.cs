using HeavyStringFilter.Application.Interfaces;
using HeavyStringFilter.Infrastructure.Queue;

namespace HeavyStringFilter.Tests.Queue;

public class InMemoryProcessingQueueTests
{
    private readonly IProcessingQueue _queue;

    public InMemoryProcessingQueueTests()
    {
        _queue = new InMemoryProcessingQueue();
    }

    [Fact]
    public void Enqueue_Then_Dequeue_Returns_Same_Item()
    {
        var task = new ProcessingTask("upload-1", "text content");

        _queue.Enqueue(task);
        var success = _queue.TryDequeue(out var dequeued);

        Assert.True(success);
        Assert.Equal(task.UploadId, dequeued.UploadId);
        Assert.Equal(task.FullText, dequeued.FullText);
    }

    [Fact]
    public void Dequeue_From_Empty_Returns_False()
    {
        var success = _queue.TryDequeue(out var result);

        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void Enqueue_Multiple_Items_Respects_FIFO_Order()
    {
        var task1 = new ProcessingTask("id-1", "text-1");
        var task2 = new ProcessingTask("id-2", "text-2");

        _queue.Enqueue(task1);
        _queue.Enqueue(task2);

        _queue.TryDequeue(out var first);
        _queue.TryDequeue(out var second);

        Assert.Equal("id-1", first.UploadId);
        Assert.Equal("id-2", second.UploadId);
    }

    [Fact]
    public void Multiple_Dequeue_Exhausts_Queue()
    {
        _queue.Enqueue(new ProcessingTask("id-1", "t1"));
        _queue.Enqueue(new ProcessingTask("id-2", "t2"));

        _queue.TryDequeue(out _);
        _queue.TryDequeue(out _);

        var success = _queue.TryDequeue(out var empty);
        Assert.False(success);
        Assert.Null(empty);
    }
}
