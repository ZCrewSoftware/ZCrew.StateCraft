using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft.Actions;

/// <inheritdoc />
internal class InvokeAction : IAction
{
    private readonly IAsyncAction action;

    internal InvokeAction(IAsyncAction action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public Task Invoke(CancellationToken token)
    {
        return this.action.InvokeAsync(token);
    }
}

/// <inheritdoc />
internal class InvokeAction<T> : IAction<T>
{
    private readonly IAsyncAction<T> action;

    internal InvokeAction(IAsyncAction<T> action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public Task Invoke(T parameter, CancellationToken token)
    {
        return this.action.InvokeAsync(parameter, token);
    }
}

/// <inheritdoc />
internal class InvokeAction<T1, T2> : IAction<T1, T2>
{
    private readonly IAsyncAction<T1, T2> action;

    internal InvokeAction(IAsyncAction<T1, T2> action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public Task Invoke(T1 parameter1, T2 parameter2, CancellationToken token)
    {
        return this.action.InvokeAsync(parameter1, parameter2, token);
    }
}

/// <inheritdoc />
internal class InvokeAction<T1, T2, T3> : IAction<T1, T2, T3>
{
    private readonly IAsyncAction<T1, T2, T3> action;

    internal InvokeAction(IAsyncAction<T1, T2, T3> action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public Task Invoke(T1 parameter1, T2 parameter2, T3 parameter3, CancellationToken token)
    {
        return this.action.InvokeAsync(parameter1, parameter2, parameter3, token);
    }
}

/// <inheritdoc />
internal class InvokeAction<T1, T2, T3, T4> : IAction<T1, T2, T3, T4>
{
    private readonly IAsyncAction<T1, T2, T3, T4> action;

    internal InvokeAction(IAsyncAction<T1, T2, T3, T4> action)
    {
        this.action = action;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public Task Invoke(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, CancellationToken token)
    {
        return this.action.InvokeAsync(parameter1, parameter2, parameter3, parameter4, token);
    }
}
