using System;
using System.Threading.Tasks;

namespace PresentationToolkit.Core.Common
{
    /// <summary>
    /// Task related helper methods.
    /// </summary>
    internal static class TaskHelper
    {
        /// <summary>
        /// Attempts to unwrap the exception from <see cref="AggregateException"/>.
        /// </summary>
        /// <typeparam name="T">The task return type.</typeparam>
        /// <param name="completionSource">The task completion source.</param>
        /// <param name="exception">The <see cref="Exception"/> instance.</param>
        internal static void UnwrappedException<T>(this TaskCompletionSource<T> completionSource, Exception exception)
        {
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                completionSource.SetException(aggregateException.InnerExceptions);
            }
            else
            {
                completionSource.SetException(exception);
            }
        }

        /// <summary>
        /// Attempts to unwrap the exception from <see cref="AggregateException"/>.
        /// </summary>
        /// <typeparam name="T">The task return type.</typeparam>
        /// <param name="completionSource">The task completion source.</param>
        /// <param name="exception">The <see cref="Exception"/> instance.</param>
        internal static bool TryUnwrappedException<T>(this TaskCompletionSource<T> completionSource, Exception exception)
        {
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                return completionSource.TrySetException(aggregateException.InnerExceptions);
            }
            else
            {
                return completionSource.TrySetException(exception);
            }
        }
    }
}
