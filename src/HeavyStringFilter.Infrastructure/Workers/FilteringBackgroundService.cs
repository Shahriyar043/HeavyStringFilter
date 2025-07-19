using HeavyStringFilter.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeavyStringFilter.Infrastructure.Workers;

public class FilteringBackgroundService(
    IProcessingQueue queue,
    IFilterService filterService,
    ILogger<FilteringBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (queue.TryDequeue(out var task))
            {
                var filtered = filterService.Filter(task.FullText);
                logger.LogInformation("Filtered text for {UploadId}: {Result}", task.UploadId, filtered);
            }

            await Task.Delay(100, stoppingToken);
        }
    }
}