using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Enclave.Common.Test.Core;

public abstract class TestBase
{
    protected static Task Sleep(TimeSpan time) 
        => Task.Delay(time, TestContext.Current.CancellationToken);
    protected static Task Sleep(int millisecs)
        => Task.Delay(millisecs, TestContext.Current.CancellationToken);

    protected static Task RunTask(Func<Task> taskFactory) 
        => Task.Run(taskFactory);
    protected static Task RunTask(Action taskFactory)
        => Task.Run(taskFactory);

    protected static Task<T> WaitTask<T>(Task<T> task, TimeSpan timeout) 
        => task.WaitAsync(timeout, TestContext.Current.CancellationToken);
    protected static Task WaitTask(Task task, TimeSpan timeout)
        => task.WaitAsync(timeout, TestContext.Current.CancellationToken);

}
