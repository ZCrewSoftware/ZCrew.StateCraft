namespace ZCrew.StateCraft.IntegrationTests.Extensions;

internal static class TaskExtensions
{
    /// <summary>
    ///     Waits for the <paramref name="task"/> to complete or until <paramref name="timeout"/> has elapsed. The
    ///     default timeout is one second. This will avoid deadlocked tests which may hang a CI/CD pipeline.
    /// </summary>
    /// <param name="task">The task to await.</param>
    /// <param name="timeout">The timeout duration.</param>
    /// <exception cref="TimeoutException">If the <paramref name="task"/> did not complete in time.</exception>
    /// <remarks>
    ///     The name <c>WithTimeout</c> was unfortunately taken by an extension method in Xunit.
    /// </remarks>
    internal static async Task Timeout(this Task task, TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(1);
        var timeoutTask = Task.Delay(timeout.Value, TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(task, timeoutTask);

        if (completedTask == timeoutTask)
        {
            throw new TimeoutException($"Operation did not complete within {timeout.Value}");
        }

        await task;
    }

    /// <summary>
    ///     Waits for the <paramref name="task"/> to complete or until <paramref name="timeout"/> has elapsed. The
    ///     default timeout is one second. This will avoid deadlocked tests which may hang a CI/CD pipeline.
    /// </summary>
    /// <param name="task">The task to await.</param>
    /// <param name="timeout">The timeout duration.</param>
    /// <typeparam name="T">The result type.</typeparam>
    /// <returns>The result of the <paramref name="task"/> if it completed in time.</returns>
    /// <exception cref="TimeoutException">If the <paramref name="task"/> did not complete in time.</exception>
    /// <remarks>
    ///     The name <c>WithTimeout</c> was unfortunately taken by an extension method in Xunit.
    /// </remarks>
    internal static async Task<T> Timeout<T>(this Task<T> task, TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(1);
        var timeoutTask = Task.Delay(timeout.Value, TestContext.Current.CancellationToken);
        var completedTask = await Task.WhenAny(task, timeoutTask);

        if (completedTask == timeoutTask)
        {
            throw new TimeoutException($"Operation did not complete within {timeout.Value}");
        }

        return await task;
    }
}
