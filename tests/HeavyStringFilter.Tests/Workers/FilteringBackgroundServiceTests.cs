using HeavyStringFilter.Application.Interfaces;
using HeavyStringFilter.Infrastructure.Workers;
using Microsoft.Extensions.Logging;
using Moq;

namespace HeavyStringFilter.Tests.Workers;

public class FilteringBackgroundServiceTests
{
    [Fact]
    public async Task Executes_Filter_When_Queue_Has_Task()
    {
        // Arrange
        var mockQueue = new Mock<IProcessingQueue>();
        var mockFilter = new Mock<IFilterService>();
        var mockLogger = new Mock<ILogger<FilteringBackgroundService>>();

        var testTask = new ProcessingTask("upload-1", "full text content");

        bool firstCall = true;

        mockQueue
            .Setup(q => q.TryDequeue(out It.Ref<ProcessingTask>.IsAny))
            .Returns((out ProcessingTask task) =>
            {
                if (firstCall)
                {
                    task = testTask;
                    firstCall = false;
                    return true;
                }
                else
                {
                    task = null!;
                    return false;
                }
            });

        mockFilter
            .Setup(f => f.Filter("full text content"))
            .Returns("filtered result");

        var service = new FilteringBackgroundService(mockQueue.Object, mockFilter.Object, mockLogger.Object);

        using var cts = new CancellationTokenSource();

        // Act
        var executionTask = service.StartAsync(cts.Token);

        await Task.Delay(200);
        cts.Cancel();
        await executionTask;

        // Assert
        mockQueue.Verify(q => q.TryDequeue(out It.Ref<ProcessingTask>.IsAny), Times.AtLeastOnce);
        mockFilter.Verify(f => f.Filter("full text content"), Times.Once);
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Filtered text for upload-1")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Does_Not_Filter_When_Queue_Is_Empty()
    {
        // Arrange
        var mockQueue = new Mock<IProcessingQueue>();
        var mockFilter = new Mock<IFilterService>();
        var mockLogger = new Mock<ILogger<FilteringBackgroundService>>();

        ProcessingTask dummy = null!;
        mockQueue.Setup(q => q.TryDequeue(out dummy)).Returns(false);

        var service = new FilteringBackgroundService(mockQueue.Object, mockFilter.Object, mockLogger.Object);

        using var cts = new CancellationTokenSource();

        // Act
        var executionTask = service.StartAsync(cts.Token);
        await Task.Delay(150);
        cts.Cancel();
        await executionTask;

        // Assert
        mockFilter.Verify(f => f.Filter(It.IsAny<string>()), Times.Never);
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
}
