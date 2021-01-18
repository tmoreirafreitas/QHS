using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QHS.QueuedHosted
{
    /// <summary>
    /// The TaskQueue interface contains all the methods for performing basic queue functions.
    /// The functions need to be implemented by the class that contracts with this interface
    /// </summary>
    /// <remarks>
    /// This interface can Queue and Dequeue tasks.
    /// </remarks>
    public interface ITaskQueue
    {        
        /// <summary>
        /// Method that adds a task to the queue
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws an exception if the workItem is not found.</exception>
        /// <param name="workItem"></param>
        void QueueWorkItem(Func<CancellationToken, IServiceScopeFactory, Task> workItem);

        /// <summary>
        /// Method that removes a task from the queue
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>return the Task workItem</returns>
        Task<Func<CancellationToken, IServiceScopeFactory, Task>> DequeueAsync(CancellationToken cancellationToken);
    }

    public interface ITaskQueue<T>
    {
        void QueueWorkItem(Func<CancellationToken, Task<T>> workItem);
        void QueueWorkItem(Func<CancellationToken, IServiceScopeFactory, Task<T>> workItem);
        Task<Func<CancellationToken, IServiceScopeFactory, Task<T>>> DequeueAsync(CancellationToken cancellationToken);
    }
}
