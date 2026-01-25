using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft.Actions;

/// <inheritdoc />
internal class InitialParameterlessActionConfiguration : IInitialParameterlessActionConfiguration
{
    /// <inheritdoc />
    public IParameterlessAction Build()
    {
        throw new InvalidOperationException("Unable to build an action from a partial action configuration");
    }

    /// <inheritdoc />
    public IFinalParameterlessActionConfiguration Invoke(Action action)
    {
        return new InvokeParameterlessActionConfiguration(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IFinalParameterlessActionConfiguration Invoke(Func<CancellationToken, Task> action)
    {
        return new InvokeParameterlessActionConfiguration(action.AsAsyncAction());
    }

    /// <inheritdoc />
    public IFinalParameterlessActionConfiguration Invoke(Func<CancellationToken, ValueTask> action)
    {
        return new InvokeParameterlessActionConfiguration(action.AsAsyncAction());
    }
}
