using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace QHS.QueuedHosted
{
    public class ActionQueue : IActionQueue
    {
        private readonly ConcurrentQueue<Action<CancellationToken, IServiceScopeFactory>> _workItems = new ConcurrentQueue<Action<CancellationToken, IServiceScopeFactory>>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public async Task<Action<CancellationToken, IServiceScopeFactory>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }

        public void QueueWorkItem(Action<CancellationToken, IServiceScopeFactory> workItem)
        {
            if(workItem == null)
                throw new ArgumentNullException(nameof(workItem));
            _workItems.Enqueue(workItem);
            _signal.Release();
        }
    }
}
