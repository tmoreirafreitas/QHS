using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QHS.QueuedHosted
{
    public class ActionQueuedHostedService : BackgroundService, IHostedService
    {
        private readonly ILogger _logger;
        private readonly IActionQueue _actionQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ActionQueuedHostedService(ILoggerFactory loggerFactory, IActionQueue actionQueue, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = loggerFactory.CreateLogger<ActionQueuedHostedService>();
            _actionQueue = actionQueue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"ActionQueued Hosted Service is running.{Environment.NewLine}");
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _actionQueue.DequeueAsync(stoppingToken);
                try
                {
                    workItem(stoppingToken, _serviceScopeFactory);
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
            _logger.LogInformation("ActionQueued Hosted Service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}
