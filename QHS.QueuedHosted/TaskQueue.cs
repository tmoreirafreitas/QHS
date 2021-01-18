using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace QHS.QueuedHosted
{
    /// <summary>
    /// The main TaskQueue class.
    /// Contains all methods for performing basic queue functions.
    /// </summary>
    /// <remarks>
    /// This class can Queue and Dequeue tasks.
    /// </remarks>
    public class TaskQueue : ITaskQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, IServiceScopeFactory, Task>> _workItems = new ConcurrentQueue<Func<CancellationToken, IServiceScopeFactory, Task>>();  
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        /// <summary>
        /// Method that removes a task from the queue
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>return the Task workItem</returns>
        public async Task<Func<CancellationToken, IServiceScopeFactory, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }

        /// <summary>
        /// Method that adds a task to the queue
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws an exception if the workItem is not found.</exception>
        /// <param name="workItem"></param>
        public void QueueWorkItem(Func<CancellationToken, IServiceScopeFactory, Task> workItem)
        {
            if(workItem == null)
                throw new ArgumentNullException(nameof(workItem));
            _workItems.Enqueue(workItem);
            _signal.Release();
        }
    }
}