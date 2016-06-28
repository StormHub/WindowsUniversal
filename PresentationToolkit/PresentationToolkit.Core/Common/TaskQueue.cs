using System;
using System.Threading.Tasks;
using System.Threading;

namespace PresentationToolkit.Core.Common
{
    /// <summary>
    /// Provides serial queued task execution.
    /// </summary>
    internal sealed class TaskQueue
    {
        private const int MaximumQueueSize = 10;

        private readonly object syncLock = new object();
        private readonly int maximumSize;

        private Task lastQueuedTask;
        private long size;
        private volatile bool drained;

        /// <summary>
        /// Initializes a new instance of <see cref="TaskQueue"/>
        /// </summary>
        /// <param name="maximumSize">The maximum size.</param>
        public TaskQueue(int maximumSize = MaximumQueueSize)
        {
            this.maximumSize = maximumSize;
            lastQueuedTask = AsyncTaskExtensions.Empty;
        }

        /// <summary>
        /// Indicates whether the queue is drained to the last.
        /// </summary>
        public bool IsDrained
        {
            get
            {
                return drained;
            }
        }

        /// <summary>
        /// Enqueues a task with the function to generate it.
        /// </summary>
        /// <param name="activator">The function to generate the task.</param>
        /// <param name="state">The state object.</param>
        /// <returns>The queued task.</returns>
        public Task Enqueue(Func<object, Task> activator, object state)
        {
            lock (syncLock)
            {
                if (drained)
                {
                    return lastQueuedTask;
                }

                // Increment the size if the queue
                if (Interlocked.Increment(ref size) > maximumSize)
                {
                    Interlocked.Decrement(ref size);

                    // We failed to enqueue because the size limit was reached
                    return null;
                }

                var newTask = lastQueuedTask
                    .Then((n, ns, q) => q.InvokeNext(n, ns), activator, state, this);

                lastQueuedTask = newTask;

                return newTask;
            }
        }

        /// <summary>
        /// Enqueues the function to create a task.
        /// </summary>
        /// <param name="activator">The function to create the task.</param>
        /// <returns>The queued task.</returns>
        public Task Enqueue(Func<Task> activator)
        {
            return Enqueue(state => ((Func<Task>)state).Invoke(), activator);
        }

        private Task InvokeNext(Func<object, Task> next, object nextState)
        {
            return next(nextState).Finally(s => ((TaskQueue)s).Dequeue(), this);
        }

        /// <summary>
        /// Dequeues the task size.
        /// </summary>
        private void Dequeue()
        {
            Interlocked.Decrement(ref size);
        }

        /// <summary>
        /// Drains the queue and causes it to stop queuing tasks and running till the
        /// last one queued.
        /// </summary>
        /// <returns>The las queued task.</returns>
        public Task Drain()
        {
            lock (syncLock)
            {
                drained = true;

                return lastQueuedTask;
            }
        }
    }
}
