using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QHS.QueuedHosted;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QHS.ApiHostedTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITaskQueue _taskQueue;
        private readonly IActionQueue _actionQueue;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ITaskQueue taskQueue, IActionQueue actionQueue)
        {
            _logger = logger;
            _taskQueue = taskQueue;
            _actionQueue = actionQueue;
        }

        [HttpGet]
        public IActionResult GetAsync(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                _taskQueue.QueueWorkItem(async (token, scopeProvider) => await Task.Run(async () =>
                 {
                     _logger.LogInformation("Task Worker running at: {Time}", DateTime.Now);
                     using var scope = scopeProvider.CreateScope();
                     var oItem = scope.ServiceProvider.GetService(typeof(IObjectItem)) as IObjectItem;
                     oItem.Escrever();
                     oItem = null;
                     await Task.Delay(3000);                     
                     _logger.LogInformation("Task Worker stopping at: {Time}", DateTime.Now);

                 }, token));

                _actionQueue.QueueWorkItem(async (stoppingToken, scopeProvider) =>
                {
                    await Task.Run(async () =>
                    {
                        _logger.LogInformation("Action Worker running at: {Time}", DateTime.Now);
                        using var scope = scopeProvider.CreateScope();
                        var oItem = scope.ServiceProvider.GetService(typeof(IObjectItem)) as IObjectItem;
                        oItem.Escrever2();
                        oItem = null;
                        await Task.Delay(3000);
                        _logger.LogInformation("Action Worker stopping at: {Time}", DateTime.Now);
                    }, stoppingToken);
                });
            }
            return Ok();
        }
    }
}
