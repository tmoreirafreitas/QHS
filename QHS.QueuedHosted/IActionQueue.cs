using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QHS.QueuedHosted
{
    public interface IActionQueue
    {
        void QueueWorkItem(Action<CancellationToken, IServiceScopeFactory> workItem);
        Task<Action<CancellationToken, IServiceScopeFactory>> DequeueAsync(CancellationToken cancellationToken);
    }
}
