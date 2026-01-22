namespace ZCrew.StateCraft;

public interface IMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext>
    : ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    ///     The condition receives the mapped parameter value from the previous that will be passed to the next state.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext> If(Func<TNext, bool> condition);

    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    ///     The condition receives the mapped parameter value from the previous that will be passed to the next state.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext> If(
        Func<TNext, CancellationToken, Task<bool>> condition
    );

    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    ///     The condition receives the mapped parameter value from the previous that will be passed to the next state.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext> If(
        Func<TNext, CancellationToken, ValueTask<bool>> condition
    );

    /// <summary>
    ///     Configure the state to transition to.
    /// </summary>
    /// <param name="state">The next state.</param>
    /// <returns>The final state configuration step.</returns>
    IFinalTransitionConfiguration<TState, TTransition, TPrevious> To(TState state);

    /// <summary>
    ///     Configures this transition to return to the same state.
    /// </summary>
    /// <returns>The final state configuration step.</returns>
    /// <remarks>
    ///     Since a state may have the same <see cref="IState{TState,TTransition}.StateValue"/> but different parameters
    ///     this may point to a different configured state. This is merely shorthand for <see cref="To"/> with the same
    ///     <see cref="IState{TState,TTransition}.StateValue"/>.
    /// </remarks>
    IFinalTransitionConfiguration<TState, TTransition, TPrevious> ToSameState()
    {
        return To(PreviousStateValue);
    }
}
