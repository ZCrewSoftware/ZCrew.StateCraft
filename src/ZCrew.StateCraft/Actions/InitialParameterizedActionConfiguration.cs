using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft.Actions;

/// <inheritdoc />
internal class InitialParameterizedActionConfiguration<T> : IInitialParameterizedActionConfiguration<T>
{
    /// <inheritdoc />
    public IParameterizedAction<T> Build()
    {
        throw new InvalidOperationException("Unable to build an action from a partial action configuration");
    }

    /// <inheritdoc />
    public IFinalParameterizedActionConfiguration<T> Invoke(Action<T> action)
    {
        return new InvokeParameterizedActionConfiguration<T>(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IFinalParameterizedActionConfiguration<T> Invoke(Func<T, CancellationToken, Task> action)
    {
        return new InvokeParameterizedActionConfiguration<T>(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IFinalParameterizedActionConfiguration<T> Invoke(Func<T, CancellationToken, ValueTask> action)
    {
        return new InvokeParameterizedActionConfiguration<T>(action.AsAsyncAction());
    }
}
