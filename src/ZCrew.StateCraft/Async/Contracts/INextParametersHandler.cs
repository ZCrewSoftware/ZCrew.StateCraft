using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Async.Contracts;

/// <summary>
///     Weakly-typed wrapper around a strongly-typed asynchronous handler invoked by the state machine that pulls from
///     the <see cref="IStateMachineParameters.GetNextParameter"/> overloads.
/// </summary>
internal interface INextParametersHandler
{
    /// <summary>
    ///     The descriptor of the inner handler.
    /// </summary>
    string? Descriptor { get; }

    /// <summary>
    ///     Invokes the wrapped handler.
    /// </summary>
    /// <param name="parameters">The state machine parameters to pull the next parameters from.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    Task Invoke(IStateMachineParameters parameters, CancellationToken token);
}
