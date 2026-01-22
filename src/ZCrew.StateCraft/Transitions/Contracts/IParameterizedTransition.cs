namespace ZCrew.StateCraft;

/// <summary>
///     A parameterized transition from a parameterless state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
internal interface IParameterizedTransition<TState, TTransition, in TNext> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Evaluate if the conditions to perform this transition have been met.
    /// </summary>
    /// <param name="nextParameter">The parameter for the next state.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> if the conditions have been met; <see langword="false"/> otherwise.</returns>
    Task<bool> EvaluateConditions(TNext nextParameter, CancellationToken token);
}

/// <summary>
///     A parameterized transition from a parameterized state.
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
/// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
internal interface IParameterizedTransition<TState, TTransition, in TPrevious, in TNext>
    : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Evaluate if the conditions to perform this transition have been met.
    /// </summary>
    /// <param name="previousParameter">The parameter from the previous state.</param>
    /// <param name="nextParameter">The parameter for the next state.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> if the conditions have been met; <see langword="false"/> otherwise.</returns>
    Task<bool> EvaluateConditions(TPrevious previousParameter, TNext nextParameter, CancellationToken token);
}
