
namespace PresentationToolkit.Core.Tests
{
    /*
    public class TaskAsyncHelperFacts
    {
        private static readonly CultureInfo _defaultCulture = Thread.CurrentThread.CurrentCulture;
        private static readonly CultureInfo _defaultUICulture = Thread.CurrentThread.CurrentUICulture;
        private static readonly CultureInfo _testCulture = new CultureInfo("zh-Hans");
        private static readonly CultureInfo _testUICulture = new CultureInfo("zh-CN");

        private static readonly Func<Task>[] _successfulTaskGenerators = new Func<Task>[]
        {
            () => TaskAsyncHelper.FromResult<object>(null), // Sync Completed
            async () => await Task.Yield(), // Async Completed
        };

        private static readonly Func<Task>[] _failedTaskGenerators = new Func<Task>[]
        {
            () =>
            {
                var faultedTcs = new TaskCompletionSource<object>();
                faultedTcs.SetException(new Exception());
                return faultedTcs.Task; // Sync Faulted
            },
            () =>
            {
                var canceledTcs = new TaskCompletionSource<object>();
                canceledTcs.SetCanceled();
                return canceledTcs.Task; // Sync Canceled
            },
            async () =>
            {
                await Task.Yield();
                throw new Exception();
            },  // Async Faulted
            async () =>
            {
                await Task.Yield();
                throw new OperationCanceledException();
            } // Async Canceled
        };

        private void EnsureCulturePreserved(IEnumerable<Func<Task>> taskGenerators, Action<Task, Action> testAction)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = _testCulture;
                Thread.CurrentThread.CurrentUICulture = _testUICulture;

                TaskCompletionSource<CultureInfo> cultureTcs = null;
                TaskCompletionSource<CultureInfo> uiCultureTcs = null;

                Action initialize = () =>
                {
                    cultureTcs = new TaskCompletionSource<CultureInfo>();
                    uiCultureTcs = new TaskCompletionSource<CultureInfo>();
                };

                Action saveCulture = () =>
                {
                    cultureTcs.SetResult(Thread.CurrentThread.CurrentCulture);
                    uiCultureTcs.SetResult(Thread.CurrentThread.CurrentUICulture);
                };

                foreach (var taskGenerator in taskGenerators)
                {
                    initialize();

                    testAction(taskGenerator(), saveCulture);

                    Assert.Equal(_testCulture, cultureTcs.Task.Result);
                    Assert.Equal(_testUICulture, uiCultureTcs.Task.Result);
                }

                // Verify that threads in the ThreadPool keep the default culture
                initialize();

                Task.Delay(100).ContinueWith(_ => saveCulture());

                Assert.Equal(_defaultCulture, cultureTcs.Task.Result);
                Assert.Equal(_defaultUICulture, uiCultureTcs.Task.Result);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = _defaultCulture;
                Thread.CurrentThread.CurrentUICulture = _defaultUICulture;
            }
        }

        [Fact]
        public void ThenPreservesCulture()
        {
            // Then with sync/async completed tasks
            EnsureCulturePreserved(_successfulTaskGenerators,
                (task, continuation) => task.Then(continuation));
        }

        [Fact]
        public void ContinuePreservedCulturePreservesCulture()
        {
            // ContinueWithPreservedCulture with sync/async faulted, canceled and completed tasks
            EnsureCulturePreserved(_successfulTaskGenerators.Concat(_failedTaskGenerators),
                (task, continuation) => task.ContinueWithPreservedCulture(_ => continuation()));
        }

        [Fact]
        public void PreserveCultureAwaiterPreservesCulture()
        {
            // PreserveCultureAwaiter with sync/async faulted, canceled and completed tasks
            EnsureCulturePreserved(_successfulTaskGenerators.Concat(_failedTaskGenerators),
                async (task, continuation) =>
                {
                    try
                    {
                        await task.PreserveCulture();
                    }
                    catch
                    {
                        // The MSBuild xUnit.net runner crashes if we don't catch here
                    }
                    finally
                    {
                        continuation();
                    }
                });
        }
    }
    */

    /*
    public class TaskQueueFacts
    {
        [Fact]
        public void DrainingTaskQueueShutsQueueOff()
        {
            var queue = new TaskQueue();
            queue.Enqueue(() => TaskAsyncHelper.Empty);
            queue.Drain();
            Task task = queue.Enqueue(() => TaskAsyncHelper.FromError(new Exception()));

            Assert.True(task.IsCompleted);
            Assert.False(task.IsFaulted);
        }

        [Fact]
        public void TaskQueueDoesNotQueueNewTasksIfPreviousTaskFaulted()
        {
            var queue = new TaskQueue();
            queue.Enqueue(() => TaskAsyncHelper.FromError(new Exception()));
            Task task = queue.Enqueue(() => TaskAsyncHelper.Empty);

            Assert.True(task.IsCompleted);
            Assert.True(task.IsFaulted);
        }

        [Fact]
        public void TaskQueueRunsTasksInSequence()
        {
            var queue = new TaskQueue();
            int n = 0;
            queue.Enqueue(() =>
            {
                n++;
                return TaskAsyncHelper.Empty;
            });

            Task task = queue.Enqueue(() =>
            {
                return Task.Delay(100).Then(() => n++);
            });

            task.Wait();
            Assert.Equal(n, 2);
        }

        [Fact]
        public void FailedToEnqueueReturnsNull()
        {
            var queue = new TaskQueue(TaskAsyncHelper.Empty, 2);
            queue.Enqueue(() => Task.Delay(100));
            queue.Enqueue(() => Task.Delay(100));
            Task task = queue.Enqueue(() => Task.Delay(100));
            Assert.Null(task);
        }
    }
    */
}
