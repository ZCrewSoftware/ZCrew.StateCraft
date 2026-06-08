using System.Diagnostics;
using ZCrew.StateCraft.Async.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Async;

/// <inheritdoc/>
internal sealed class ParametersHandler : INextParametersHandler, ICurrentParametersHandler, IPreviousParametersHandler
{
    private readonly AsyncHandler handler;

    public ParametersHandler(AsyncHandler handler)
    {
        this.handler = handler;
    }

    /// <inheritdoc/>
    public string? Descriptor => this.handler.Descriptor;

    /// <inheritdoc/>
    Task INextParametersHandler.Invoke(IStateMachineParameters parameters, CancellationToken token)
    {
        Debug.Assert(parameters.IsNextSet); // For tests, be sure there are next parameters since we don't use them
        return this.handler.Handler.InvokeAsync(token);
    }

    /// <inheritdoc/>
    Task ICurrentParametersHandler.Invoke(IStateMachineParameters parameters, CancellationToken token)
    {
        Debug.Assert(parameters.IsCurrentSet); // For tests, be sure there are current parameters since we don't use them
        return this.handler.Handler.InvokeAsync(token);
    }

    /// <inheritdoc/>
    Task IPreviousParametersHandler.Invoke(IStateMachineParameters parameters, CancellationToken token)
    {
        Debug.Assert(parameters.IsPreviousSet); // For tests, be sure there are previous parameters since we don't use them
        return this.handler.Handler.InvokeAsync(token);
    }
}
