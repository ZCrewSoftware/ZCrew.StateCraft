using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Async.Contracts;

/// <summary>
///     Weakly-typed wrapper around a strongly-typed asynchronous handler invoked by the state machine that pulls from
///     the <see cref="IStateMachineParameters.GetPreviousParameter"/> overloads.
/// </summary>
internal interface IPreviousParametersHandler
{
    /// <summary>
    ///     The descriptor of the inner handler.
    /// </summary>
    string? Descriptor { get; }

    /// <summary>
    ///     Invokes the wrapped handler.
    /// </summary>
    /// <param name="parameters">The state machine parameters to pull the previous parameters from.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    Task Invoke(IStateMachineParameters parameters, CancellationToken token);
}
