using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QHS.QueuedHosted
{
    /// <summary>
    /// This class contains the background tasks in the queue. 
    /// Tasks are removed from the queue and run in background processing.
    /// Work items are awaited before the service stops StopAsync.
    /// </summary>
    public class TaskQueuedHostedService : BackgroundService, IHostedService
    {
        private readonly ILogger _logger;
        private readonly ITaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Initializes the new instance of the QueuedHostedService class that contains a task queue and a log factory.
        /// </summary>
        /// <param name="taskQueue"></param>
        /// <param name="loggerFactory"></param>
        public TaskQueuedHostedService(ITaskQueue taskQueue, ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = loggerFactory.CreateLogger<TaskQueuedHostedService>();
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"TaskQueued Hosted Service is running.{Environment.NewLine}");
            await BackgroundProcessing(stoppingToken);
        }        

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken, _serviceScopeFactory);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        /// <summary>
        /// Method responsible for stopping the Queued Hosted Service
        /// </summary>
        /// <param name="stoppingToken"></param>
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TaskQueued Hosted Service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}
