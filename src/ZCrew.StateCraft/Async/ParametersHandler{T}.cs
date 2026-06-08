using ZCrew.StateCraft.Async.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Async;

/// <inheritdoc/>
internal sealed class ParametersHandler<T>
    : INextParametersHandler,
        ICurrentParametersHandler,
        IPreviousParametersHandler
{
    private readonly AsyncHandler<T> handler;

    public ParametersHandler(AsyncHandler<T> handler)
    {
        this.handler = handler;
    }

    /// <inheritdoc/>
    public string? Descriptor => this.handler.Descriptor;

    /// <inheritdoc/>
    Task INextParametersHandler.Invoke(IStateMachineParameters parameters, CancellationToken token)
    {
        var nextParameter = parameters.GetNextParameter<T>();
        return this.handler.Handler.InvokeAsync(nextParameter, token);
    }

    /// <inheritdoc/>
    Task ICurrentParametersHandler.Invoke(IStateMachineParameters parameters, CancellationToken token)
    {
        var currentParameter = parameters.GetCurrentParameter<T>();
        return this.handler.Handler.InvokeAsync(currentParameter, token);
    }

    /// <inheritdoc/>
    Task IPreviousParametersHandler.Invoke(IStateMachineParameters parameters, CancellationToken token)
    {
        var previousParameter = parameters.GetPreviousParameter<T>();
        return this.handler.Handler.InvokeAsync(previousParameter, token);
    }
}
