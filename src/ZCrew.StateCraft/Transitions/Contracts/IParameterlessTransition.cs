namespace ZCrew.StateCraft.Transitions.Contracts;

/// <summary>
///     A parameterless transition from a parameterless state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal interface IParameterlessTransition<TState, TTransition> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Evaluate if the conditions to perform this transition have been met.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> if the conditions have been met; <see langword="false"/> otherwise.</returns>
    Task<bool> EvaluateConditions(CancellationToken token);
}

/// <summary>
///     A parameterless transition from a parameterized state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TPrevious">The type of the parameter for the previous state.</typeparam>
internal interface IParameterlessTransition<TState, TTransition, in TPrevious> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Evaluate if the conditions to perform this transition have been met.
    /// </summary>
    /// <param name="previousParameter">The parameter from the previous state.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> if the conditions have been met; <see langword="false"/> otherwise.</returns>
    Task<bool> EvaluateConditions(TPrevious previousParameter, CancellationToken token);
}
