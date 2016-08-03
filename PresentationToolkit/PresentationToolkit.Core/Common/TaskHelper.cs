using System;
using System.Threading.Tasks;

namespace PresentationToolkit.Core.Common
{
    /// <summary>
    /// Provides utility task extensions.
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// Gets an empty task instance already completed.
        /// </summary>
        public static Task Empty { get; } = Task.FromResult<object>(null);

        /// <summary>
        /// Executes the action as task with the specified argument.
        /// </summary>
        /// <typeparam name="T1">The argument type.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <param name="arg">The argument.</param>
        /// <returns>The empty task.</returns>
        public static Task From<T1>(Action<T1> action, T1 arg)
        {
            try
            {
                action(arg);
                return Empty;
            }
            catch (Exception excption)
            {
                return Task.FromException(excption);
            }
        }

        /// <summary>
        /// Unwraps the specified exception if it is an instance of aggregated exceptions.
        /// </summary>
        /// <typeparam name="T">The task return type.</typeparam>
        /// <param name="completionSource">The task completion source instance.</param>
        /// <param name="exception">The exception instance.</param>
        public static void Unwrap<T>(this TaskCompletionSource<T> completionSource, Exception exception)
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
        /// Attempts to unwraps the specified exception if it is an instance of aggregated exceptions.
        /// </summary>
        /// <typeparam name="T">The task return type.</typeparam>
        /// <param name="completionSource">The task completion source instance.</param>
        /// <param name="exception">The exception instance.</param>
        public static void TryUnwrap<T>(this TaskCompletionSource<T> completionSource, Exception exception)
        {
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                completionSource.TrySetException(aggregateException.InnerExceptions);
            }
            else
            {
                completionSource.TrySetException(exception);
            }
        }

        /// <summary>
        /// Creates a task and run the action if completed successfully.
        /// </summary>
        /// <param name="activator">The function to creat task.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>The awaitable task.</returns>
        public static Task Run<T>(Func<Task<T>> activator, Action<T> action)
        {
            return TaskRunner<T, object>.Run(activator.Invoke(), action);
        }

        /// <summary>
        /// Creates a task and run the action if completed successfully.
        /// </summary>
        /// <param name="activator">The function to creat task.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>The awaitable task.</returns>
        public static Task Run(Func<Task> activator, Action action)
        {
            return Run(activator.Invoke(), action);
        }

        /// <summary>
        /// Continues the task and calls the action once the task completed.
        /// </summary>
        /// <param name="task">The task to generate the result.</param>
        /// <param name="action">The action to call with the task result.</param>
        /// <returns>The continuation task.</returns>
        public static Task Run(Task task, Action action)
        {
            var completionSource = new TaskCompletionSource<object>();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    completionSource.Unwrap(t.Exception);
                }
                else if (t.IsCanceled)
                {
                    completionSource.SetCanceled();
                }
                else
                {
                    try
                    {
                        action();
                        completionSource.SetResult(null);
                    }
                    catch (Exception exception)
                    {
                        completionSource.Unwrap(exception);
                    }
                } 
            });

            return completionSource.Task;
        }

        /// <summary>
        /// Continues the task and calls the specified action when completed with state.
        /// </summary>
        /// <param name="task">The task to continue.</param>
        /// <param name="action">The action to call.</param>
        /// <param name="state">The state object.</param>
        /// <returns>The continuation task.</returns>
        public static Task Finally(this Task task, Action<object> action, object state)
        {
            try
            {
                switch (task.Status)
                {
                    case TaskStatus.Faulted:
                    case TaskStatus.Canceled:
                        action(state);
                        return task;
                    case TaskStatus.RanToCompletion:
                        return From(action, state);

                    default:
                        return RunTaskSynchronously(task, action, state, onlyOnSuccess: false);
                }
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>
        /// Runs the task synchronously and calls the action when completed with the state.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="action">The action to call.</param>
        /// <param name="state">The action state.</param>
        /// <param name="onlyOnSuccess">Only executes the action when task is completed successfully.</param>
        /// <returns>The continuation task.</returns>
        private static Task RunTaskSynchronously(Task task, Action<object> action, object state, bool onlyOnSuccess = true)
        {
            var completionSource = new TaskCompletionSource<object>();
            task.ContinueWith(t =>
            {
                try
                {
                    if (t.IsFaulted)
                    {
                        if (!onlyOnSuccess)
                        {
                            action(state);
                        }

                        completionSource.Unwrap(t.Exception);
                    }
                    else if (t.IsCanceled)
                    {
                        if (!onlyOnSuccess)
                        {
                            action(state);
                        }

                        completionSource.SetCanceled();
                    }
                    else
                    {
                        action(state);
                        completionSource.SetResult(null);
                    }
                }
                catch (Exception exception)
                {
                    completionSource.Unwrap(exception);
                }
            },
            TaskContinuationOptions.ExecuteSynchronously);

            return completionSource.Task;
        }

        /// <summary>
        /// Unwrap a task to create another task into one continuation until the all finishes.
        /// </summary>
        /// <param name="task">The task to create another task.</param>
        /// <returns>The unwarpped task.</returns>
        public static Task UnwrapTask(this Task<Task> task)
        {
            var innerTask = (task.Status == TaskStatus.RanToCompletion) ? task.Result : null;
            return innerTask ?? task.Unwrap();
        }

        /// <summary>
        /// Continutes the task with another task.
        /// </summary>
        /// <typeparam name="T1">First argument type.</typeparam>
        /// <typeparam name="T2">Second argument type.</typeparam>
        /// <typeparam name="T3">Third argument type.</typeparam>
        /// <param name="task">The task to continue with.</param>
        /// <param name="activator">The function to call.</param>
        /// <param name="arg1">First argument.</param>
        /// <param name="arg2">Second argument.</param>
        /// <param name="arg3">Third argument.</param>
        /// <returns></returns>
        public static Task Then<T1, T2, T3>(this Task task, Func<T1, T2, T3, Task> activator, T1 arg1, T2 arg2, T3 arg3)
        {
            switch (task.Status)
            {
                case TaskStatus.Faulted:
                case TaskStatus.Canceled:
                    return task;

                case TaskStatus.RanToCompletion:
                    return From(activator, arg1, arg2, arg3);

                default:
                    // Conituation
                    return GenericRunner<object, Task, T1, T2, T3>
                        .Then(task, activator, arg1, arg2, arg3)
                        .UnwrapTask();
            }
        }

        /// <summary>
        /// Invokes the specified function to generate the task. If the function call fails
        /// e.g. throws exceptions, wrap the exception with an empty task.
        /// </summary>
        /// <typeparam name="T1">First argument type.</typeparam>
        /// <typeparam name="T2">Second argument type.</typeparam>
        /// <typeparam name="T3">Third argument type.</typeparam>
        /// <param name="activator">The function to call.</param>
        /// <param name="arg1">First argument.</param>
        /// <param name="arg2">Second argument.</param>
        /// <param name="arg3">Third argument.</param>
        /// <returns>The task generated if sucessful or empty faulted task.</returns>
        public static Task From<T1, T2, T3>(Func<T1, T2, T3, Task> activator, T1 arg1, T2 arg2, T3 arg3)
        {
            try
            {
                return activator(arg1, arg2, arg3);
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>
        /// Helper to provide task continuations.
        /// </summary>
        /// <typeparam name="T">The task return type.</typeparam>
        /// <typeparam name="TResult">The continuation return type.</typeparam>
        private static class TaskRunner<T, TResult>
        {
            /// <summary>
            /// Continues the task and calls the action once the task completed.
            /// </summary>
            /// <param name="task">The task to generate the result.</param>
            /// <param name="action">The action to call with the task result.</param>
            /// <returns>The continuation task.</returns>
            internal static Task Run(Task<T> task, Action<T> action)
            {
                var completionSource = new TaskCompletionSource<object>();
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        completionSource.Unwrap(t.Exception);
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
                        catch (Exception exception)
                        {
                            completionSource.Unwrap(exception);
                        }
                    }
                });

                return completionSource.Task;
            }

            /// <summary>
            /// Continues the task and calls the action once the task completed.
            /// </summary>
            /// <param name="task">The task to generate the result.</param>
            /// <param name="action">The action to call with the task result.</param>
            /// <returns>The continuation task.</returns>
            internal static Task Run(Task<T> task, Action<Task<T>> action)
            {
                var completionSource = new TaskCompletionSource<object>();
                task.ContinueWith(t =>
                {
                    if (task.IsFaulted)
                    {
                        completionSource.Unwrap(t.Exception);
                    }
                    else if (task.IsCanceled)
                    {
                        completionSource.SetCanceled();
                    }
                    else
                    {
                        try
                        {
                            action(t);
                            completionSource.SetResult(null);
                        }
                        catch (Exception ex)
                        {
                            completionSource.Unwrap(ex);
                        }
                    }
                });

                return completionSource.Task;
            }

            /// <summary>
            /// Continues the task with the function once the taks completed.
            /// </summary>
            /// <param name="task">The task to wait for.</param>
            /// <param name="function">The function to call.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<TResult> Run(Task task, Func<TResult> function)
            {
                var completionSource = new TaskCompletionSource<TResult>();
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        completionSource.Unwrap(t.Exception);
                    }
                    else if (t.IsCanceled)
                    {
                        completionSource.SetCanceled();
                    }
                    else
                    {
                        try
                        {
                            completionSource.SetResult(function());
                        }
                        catch (Exception ex)
                        {
                            completionSource.Unwrap(ex);
                        }
                    }
                });

                return completionSource.Task;
            }

            /// <summary>
            /// Continues the task with the function once the taks completed.
            /// </summary>
            /// <param name="task">The task to wait for.</param>
            /// <param name="function">The function to call.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<TResult> Run(Task<T> task, Func<Task<T>, TResult> function)
            {
                var completionSource = new TaskCompletionSource<TResult>();
                task.ContinueWith(t =>
                {
                    if (task.IsFaulted)
                    {
                        completionSource.Unwrap(t.Exception);
                    }
                    else if (task.IsCanceled)
                    {
                        completionSource.SetCanceled();
                    }
                    else
                    {
                        try
                        {
                            completionSource.SetResult(function(t));
                        }
                        catch (Exception ex)
                        {
                            completionSource.Unwrap(ex);
                        }
                    }
                });

                return completionSource.Task;
            }
        }

        /// <summary>
        /// Wrapper for tasks with generate parameters.
        /// </summary>
        /// <typeparam name="T">The task return type.</typeparam>
        /// <typeparam name="TResult">Continuation function return type.</typeparam>
        /// <typeparam name="T1">First argument type.</typeparam>
        /// <typeparam name="T2">Second argument type.</typeparam>
        /// <typeparam name="T3">Third argument type.</typeparam>
        private static class GenericRunner<T, TResult, T1, T2, T3>
        {
            /// <summary>
            /// Continues the task and calls the action with one argument.
            /// </summary>
            /// <param name="task">The task to continue with.</param>
            /// <param name="action">The action to call.</param>
            /// <param name="arg1">The action argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task Then(Task task, Action<T1> action, T1 arg1)
            {
                return Run(task, () => action(arg1));
            }

            /// <summary>
            /// Continues the task and calls the action with two argument.
            /// </summary>
            /// <param name="task">The task to continue with.</param>
            /// <param name="action">The action to call.</param>
            /// <param name="arg1">The first action argument.</param>
            /// <param name="arg2">The second action argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task Then(Task task, Action<T1, T2> action, T1 arg1, T2 arg2)
            {
                return Run(task, () => action(arg1, arg2));
            }

            /// <summary>
            /// Continues the task and calls the action when completed with the task result 
            /// and one argument.
            /// </summary>
            /// <param name="task">The task to continue with.</param>
            /// <param name="action">The action to call.</param>
            /// <param name="arg1">The action argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task Then(Task<T> task, Action<T, T1> action, T1 arg1)
            {
                return TaskRunner<T, object>.Run(task, t => action(t.Result, arg1));
            }

            /// <summary>
            /// Continues the task and calls the function when completed with one argument
            /// and returns the result.
            /// </summary>
            /// <typeparam name="TResult">The function return type.</typeparam>
            /// <param name="task">The task to continue with.</param>
            /// <param name="action">The action to call.</param>
            /// <param name="arg1">The function argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<TResult> Then(Task task, Func<T1, TResult> function, T1 arg1)
            {
                return TaskRunner<object, TResult>.Run(task, () => function(arg1));
            }

            /// <summary>
            /// Continues the task and calls the function when completed with two arguments
            /// and returns the result.
            /// </summary>
            /// <typeparam name="TResult">The function return type.</typeparam>
            /// <param name="task">The task to continue with.</param>
            /// <param name="function">The function to call.</param>
            /// <param name="arg1">The first function argument.</param>
            /// <param name="arg2">The second function argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<TResult> Then(Task task, Func<T1, T2, TResult> function, T1 arg1, T2 arg2)
            {
                return TaskRunner<object, TResult>.Run(task, () => function(arg1, arg2));
            }

            /// <summary>
            /// Continues the task and calls the function when completed with the task result and
            /// one argument, then returns the function result.
            /// </summary>
            /// <typeparam name="TResult">The function return type.</typeparam>
            /// <param name="task">The task to continue with.</param>
            /// <param name="function">The function to call.</param>
            /// <param name="arg1">The function argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<TResult> Then(Task<T> task, Func<T, T1, TResult> function, T1 arg1)
            {
                return TaskRunner<T, TResult>.Run(task, t => function(t.Result, arg1));
            }

            /// <summary>
            /// Continues the task and calls the function when completed with the task result and
            /// two arguments, then returns the function result.
            /// </summary>
            /// <typeparam name="TResult">The function return type.</typeparam>
            /// <param name="task">The task to continue with.</param>
            /// <param name="function">The function to call.</param>
            /// <param name="arg1">The first function argument.</param>
            /// <param name="arg2">The second function argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<TResult> Then(Task<T> task, Func<T, T1, T2, TResult> function, T1 arg1, T2 arg2)
            {
                return TaskRunner<T, TResult>.Run(task, t => function(t.Result, arg1, arg2));
            }

            /// <summary>
            /// Continues the task and calls the function when completed to create another task
            /// with one argument.
            /// </summary>
            /// <param name="task">The task to continue with.</param>
            /// <param name="activator">The function to call.</param>
            /// <param name="arg1">The function argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<Task> Then(Task task, Func<T1, Task> activator, T1 arg1)
            {
                return TaskRunner<object, Task>.Run(task, () => activator(arg1));
            }

            /// <summary>
            /// Continues the task and calls the function when completed to create another task
            /// with two arguments.
            /// </summary>
            /// <param name="task">The task to continue with.</param>
            /// <param name="activator">The function to call.</param>
            /// <param name="arg1">The first function argument.</param>
            /// <param name="arg2">The second function argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<Task> Then(Task task, Func<T1, T2, Task> activator, T1 arg1, T2 arg2)
            {
                return TaskRunner<object, Task>.Run(task, () => activator(arg1, arg2));
            }

            /// <summary>
            /// Continues the task and calls the function when completed to create another task
            /// with three arguments.
            /// </summary>
            /// <param name="task">The task to continue with.</param>
            /// <param name="activator">The function to call.</param>
            /// <param name="arg1">The first function argument.</param>
            /// <param name="arg2">The second function argument.</param>
            /// <param name="arg3">The third function argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<Task> Then(Task task, Func<T1, T2, T3, Task> activator, T1 arg1, T2 arg2, T3 arg3)
            {
                return TaskRunner<object, Task>.Run(task, () => activator(arg1, arg2, arg3));
            }

            /// <summary>
            /// Continues the task and calls the function when completed with the task result 
            /// and one argument.
            /// </summary>
            /// <param name="task">The task to continue with.</param>
            /// <param name="activator">The function to call.</param>
            /// <param name="arg1">The function argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<Task<TResult>> Then(Task<T> task, Func<T, T1, Task<TResult>> activator, T1 arg1)
            {
                return TaskRunner<T, Task<TResult>>.Run(task, t => activator(t.Result, arg1));
            }

            /// <summary>
            /// Continues the task and calls the function when completed with the task instance
            /// and one argument.
            /// </summary>
            /// <param name="task">The task to continue with.</param>
            /// <param name="activator">The function to call.</param>
            /// <param name="arg1">The function argument.</param>
            /// <returns>The continuation task.</returns>
            internal static Task<Task<T>> Then(Task<T> task, Func<Task<T>, T1, Task<T>> activator, T1 arg1)
            {
                return TaskRunner<T, Task<T>>.Run(task, t => activator(t, arg1));
            }
        }
    }
}
