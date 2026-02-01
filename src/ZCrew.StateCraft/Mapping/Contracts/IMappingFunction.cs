using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Mapping.Contracts;

/// <summary>
///     Defines a function that maps parameters from the previous state to parameters for the next state during a
///     transition.
/// </summary>
internal interface IMappingFunction
{
    /// <summary>
    ///     Executes the mapping function by retrieving input parameters from the previous state, transforming them
    ///     through the configured mapping function, and setting the output parameters for the next state.
    /// </summary>
    /// <param name="parameter">
    ///     The state machine parameters container used to read previous and write next parameters.
    /// </param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous mapping operation.</returns>
    Task Map(IStateMachineParameters parameter, CancellationToken token);
}
