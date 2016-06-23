using System;
using System.Threading.Tasks;

namespace PresentationToolkit.Core.Common
{
    public static class TaskRunner
    {
        /// <summary>
        /// Runs the specified task to finish. If the task completed successfully, the action
        /// is invoked.
        /// </summary>
        /// <typeparam name="T">The task return type.</typeparam>
        /// <param name="task">The task to run.</param>
        /// <param name="action">The action to invoke if completed successfully.</param>
        /// <returns>The awaitable task.</returns>
        public static Task Run<T>(this Task<T> task, Action<T> action)
        {
            var completionSource = new TaskCompletionSource<object>();

            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    completionSource.UnwrappedException(t.Exception);
                }
                else if (t.IsCanceled)
                {
                    completionSource.SetCanceled();
                }
                else
                {
                    try
                    {
                        action(t.Result);
                        completionSource.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        completionSource.UnwrappedException(ex);
                    }
                }
            });

            return completionSource.Task;
        }
    }
}
