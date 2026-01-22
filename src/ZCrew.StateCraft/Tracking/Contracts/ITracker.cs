namespace ZCrew.StateCraft.Tracking.Contracts;

/// <summary>
///     Defines a tracker that receives notifications about state machine lifecycle events.
///     Implementations can be used for logging, debugging, or monitoring state machine behavior.
/// </summary>
/// <typeparam name="TState">The type representing state identifiers.</typeparam>
/// <typeparam name="TTransition">The type representing transition identifiers.</typeparam>
internal interface ITracker<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Called when the state machine is activated and enters its initial state.
    /// </summary>
    /// <param name="initialState">The initial state that was entered.</param>
    void Activated(IState<TState, TTransition> initialState);

    /// <summary>
    ///     Called when the state machine is activated with a parameter and enters its initial state.
    /// </summary>
    /// <typeparam name="T">The type of the activation parameter.</typeparam>
    /// <param name="initialState">The initial state that was entered.</param>
    /// <param name="initialParameter">The parameter passed during activation.</param>
    void Activated<T>(IState<TState, TTransition> initialState, T initialParameter);

    /// <summary>
    ///     Called when the state machine is deactivated and exits its final state.
    /// </summary>
    /// <param name="finalState">The state that was active when the state machine was deactivated.</param>
    void Deactivated(IState<TState, TTransition> finalState);

    /// <summary>
    ///     Called when the state machine is deactivated and exits its final state.
    /// </summary>
    /// <param name="finalState">The state that was active when the state machine was deactivated.</param>
    /// <param name="finalParameter">The parameter passed during deactivation.</param>
    void Deactivated<T>(IState<TState, TTransition> finalState, T finalParameter);

    /// <summary>
    ///     Called when a transition occurs between states.
    /// </summary>
    /// <param name="transition">The transition that was executed.</param>
    void Transitioned(ITransition<TState, TTransition> transition);

    /// <summary>
    ///     Called when a transition occurs between states with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the transition parameter.</typeparam>
    /// <param name="transition">The transition that was executed.</param>
    /// <param name="parameter">The parameter passed to the transition.</param>
    void Transitioned<T>(ITransition<TState, TTransition> transition, T parameter);

    /// <summary>
    ///     Called when a state is entered.
    /// </summary>
    /// <param name="state">The state that was entered.</param>
    void Entered(IState<TState, TTransition> state);

    /// <summary>
    ///     Called when a state is entered with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the entry parameter.</typeparam>
    /// <param name="state">The state that was entered.</param>
    /// <param name="parameter">The parameter passed when entering the state.</param>
    void Entered<T>(IState<TState, TTransition> state, T parameter);

    /// <summary>
    ///     Called when a state is exited.
    /// </summary>
    /// <param name="state">The state that was exited.</param>
    void Exited(IState<TState, TTransition> state);

    /// <summary>
    ///     Called when a state is exited with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the exit parameter.</typeparam>
    /// <param name="state">The state that was exited.</param>
    /// <param name="parameter">The parameter passed when exiting the state.</param>
    void Exited<T>(IState<TState, TTransition> state, T parameter);
}
