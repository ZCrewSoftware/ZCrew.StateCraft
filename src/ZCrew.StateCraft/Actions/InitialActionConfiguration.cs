using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft.Actions;

/// <inheritdoc />
internal class InitialActionConfiguration : IInitialActionConfiguration
{
    /// <inheritdoc />
    public IActionConfiguration Invoke(Action action)
    {
        return new InvokeActionConfiguration(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IActionConfiguration Invoke(Func<CancellationToken, Task> action)
    {
        return new InvokeActionConfiguration(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IActionConfiguration Invoke(Func<CancellationToken, ValueTask> action)
    {
        return new InvokeActionConfiguration(action.AsAsyncAction());
    }
}

/// <inheritdoc />
internal class InitialActionConfiguration<T> : IInitialActionConfiguration<T>
{
    /// <inheritdoc />
    public IActionConfiguration<T> Invoke(Action<T> action)
    {
        return new InvokeActionConfiguration<T>(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IActionConfiguration<T> Invoke(Func<T, CancellationToken, Task> action)
    {
        return new InvokeActionConfiguration<T>(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IActionConfiguration<T> Invoke(Func<T, CancellationToken, ValueTask> action)
    {
        return new InvokeActionConfiguration<T>(action.AsAsyncAction());
    }
}

/// <inheritdoc />
internal class InitialActionConfiguration<T1, T2> : IInitialActionConfiguration<T1, T2>
{
    /// <inheritdoc />
    public IActionConfiguration<T1, T2> Invoke(Action<T1, T2> action)
    {
        return new InvokeActionConfiguration<T1, T2>(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IActionConfiguration<T1, T2> Invoke(Func<T1, T2, CancellationToken, Task> action)
    {
        return new InvokeActionConfiguration<T1, T2>(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IActionConfiguration<T1, T2> Invoke(Func<T1, T2, CancellationToken, ValueTask> action)
    {
        return new InvokeActionConfiguration<T1, T2>(action.AsAsyncAction());
    }
}

/// <inheritdoc />
internal class InitialActionConfiguration<T1, T2, T3> : IInitialActionConfiguration<T1, T2, T3>
{
    /// <inheritdoc />
    public IActionConfiguration<T1, T2, T3> Invoke(Action<T1, T2, T3> action)
    {
        return new InvokeActionConfiguration<T1, T2, T3>(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IActionConfiguration<T1, T2, T3> Invoke(Func<T1, T2, T3, CancellationToken, Task> action)
    {
        return new InvokeActionConfiguration<T1, T2, T3>(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IActionConfiguration<T1, T2, T3> Invoke(Func<T1, T2, T3, CancellationToken, ValueTask> action)
    {
        return new InvokeActionConfiguration<T1, T2, T3>(action.AsAsyncAction());
    }
}

/// <inheritdoc />
internal class InitialActionConfiguration<T1, T2, T3, T4> : IInitialActionConfiguration<T1, T2, T3, T4>
{
    /// <inheritdoc />
    public IActionConfiguration<T1, T2, T3, T4> Invoke(Action<T1, T2, T3, T4> action)
    {
        return new InvokeActionConfiguration<T1, T2, T3, T4>(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IActionConfiguration<T1, T2, T3, T4> Invoke(Func<T1, T2, T3, T4, CancellationToken, Task> action)
    {
        return new InvokeActionConfiguration<T1, T2, T3, T4>(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IActionConfiguration<T1, T2, T3, T4> Invoke(Func<T1, T2, T3, T4, CancellationToken, ValueTask> action)
    {
        return new InvokeActionConfiguration<T1, T2, T3, T4>(action.AsAsyncAction());
    }
}
