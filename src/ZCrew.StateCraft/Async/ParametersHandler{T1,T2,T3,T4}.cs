using ZCrew.StateCraft.Async.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Async;

/// <inheritdoc/>
internal sealed class ParametersHandler<T1, T2, T3, T4>
    : INextParametersHandler,
        ICurrentParametersHandler,
        IPreviousParametersHandler
{
    private readonly AsyncHandler<T1, T2, T3, T4> handler;

    public ParametersHandler(AsyncHandler<T1, T2, T3, T4> handler)
    {
        this.handler = handler;
    }

    /// <inheritdoc/>
    public string? Descriptor => this.handler.Descriptor;

    /// <inheritdoc/>
    Task INextParametersHandler.Invoke(IStateMachineParameters parameters, CancellationToken token)
    {
        var nextParameters = parameters.GetNextParameters<T1, T2, T3, T4>();
        return this.handler.Handler.InvokeAsync(
            nextParameters.Item1,
            nextParameters.Item2,
            nextParameters.Item3,
            nextParameters.Item4,
            token
        );
    }

    /// <inheritdoc/>
    Task ICurrentParametersHandler.Invoke(IStateMachineParameters parameters, CancellationToken token)
    {
        var currentParameters = parameters.GetCurrentParameters<T1, T2, T3, T4>();
        return this.handler.Handler.InvokeAsync(
            currentParameters.Item1,
            currentParameters.Item2,
            currentParameters.Item3,
            currentParameters.Item4,
            token
        );
    }

    /// <inheritdoc/>
    Task IPreviousParametersHandler.Invoke(IStateMachineParameters parameters, CancellationToken token)
    {
        var previousParameters = parameters.GetPreviousParameters<T1, T2, T3, T4>();
        return this.handler.Handler.InvokeAsync(
            previousParameters.Item1,
            previousParameters.Item2,
            previousParameters.Item3,
            previousParameters.Item4,
            token
        );
    }
}
