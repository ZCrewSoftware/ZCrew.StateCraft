using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft.Actions;

/// <inheritdoc />
internal class InvokeActionConfiguration : IActionConfiguration
{
    private readonly IAsyncAction action;

    internal InvokeActionConfiguration(IAsyncAction action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IAction Build()
    {
        return new InvokeAction(this.action);
    }
}

/// <inheritdoc />
internal class InvokeActionConfiguration<T> : IActionConfiguration<T>
{
    private readonly IAsyncAction<T> action;

    internal InvokeActionConfiguration(IAsyncAction<T> action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IAction<T> Build()
    {
        return new InvokeAction<T>(this.action);
    }
}

/// <inheritdoc />
internal class InvokeActionConfiguration<T1, T2> : IActionConfiguration<T1, T2>
{
    private readonly IAsyncAction<T1, T2> action;

    internal InvokeActionConfiguration(IAsyncAction<T1, T2> action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IAction<T1, T2> Build()
    {
        return new InvokeAction<T1, T2>(this.action);
    }
}

/// <inheritdoc />
internal class InvokeActionConfiguration<T1, T2, T3> : IActionConfiguration<T1, T2, T3>
{
    private readonly IAsyncAction<T1, T2, T3> action;

    internal InvokeActionConfiguration(IAsyncAction<T1, T2, T3> action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IAction<T1, T2, T3> Build()
    {
        return new InvokeAction<T1, T2, T3>(this.action);
    }
}

/// <inheritdoc />
internal class InvokeActionConfiguration<T1, T2, T3, T4> : IActionConfiguration<T1, T2, T3, T4>
{
    private readonly IAsyncAction<T1, T2, T3, T4> action;

    internal InvokeActionConfiguration(IAsyncAction<T1, T2, T3, T4> action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IAction<T1, T2, T3, T4> Build()
    {
        return new InvokeAction<T1, T2, T3, T4>(this.action);
    }
}
