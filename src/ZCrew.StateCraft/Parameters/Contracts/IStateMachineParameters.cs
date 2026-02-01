namespace ZCrew.StateCraft.Parameters.Contracts;

/// <summary>
///     Manages parameter storage for state machine transitions.
/// </summary>
/// <remarks>
/// <para>
///     Parameters are stored in three slots: previous, current, and next. During a transition,
///     values flow from current to previous, and from next to current upon commit.
/// </para>
/// <para>
///     A typical transition follows this sequence:
///     <list type="number">
///         <item><see cref="BeginTransition"/> - Captures the current state for potential rollback</item>
///         <item><see cref="SetNextParameters"/> - Stages the parameters for the target state</item>
///         <item><see cref="CommitTransition"/> - Finalizes the transition, promoting next to current</item>
///     </list>
/// </para>
/// </remarks>
internal interface IStateMachineParameters
{
    /// <summary>
    ///     Retrieves a parameter from the previous state.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <param name="index">The zero-based index of the parameter.</param>
    /// <returns>The parameter value cast to <typeparamref name="T"/>.</returns>
    T GetPreviousParameter<T>(int index);

    /// <summary>
    ///     Retrieves a parameter from the current state.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <param name="index">The zero-based index of the parameter.</param>
    /// <returns>The parameter value cast to <typeparamref name="T"/>.</returns>
    T GetCurrentParameter<T>(int index);

    /// <summary>
    ///     Retrieves a staged parameter that will become current upon commit.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <param name="index">The zero-based index of the parameter.</param>
    /// <returns>The parameter value cast to <typeparamref name="T"/>.</returns>
    T GetNextParameter<T>(int index);

    /// <summary>
    ///     Stages parameters for the next state.
    /// </summary>
    /// <param name="nextParameters">
    ///     The parameters to stage. These become current upon <see cref="CommitTransition"/>.
    /// </param>
    void SetNextParameters(object?[] nextParameters);

    /// <summary>
    ///     Begins a transition by capturing the current parameters for potential rollback.
    /// </summary>
    /// <remarks>
    ///     Must be called before <see cref="SetNextParameters"/> and <see cref="CommitTransition"/>.
    ///     If an error occurs after this call, use <see cref="RollbackTransition"/> to restore the previous state.
    /// </remarks>
    void BeginTransition();

    /// <summary>
    ///     Reverts a transition in progress, restoring the state captured by <see cref="BeginTransition"/>.
    /// </summary>
    void RollbackTransition();

    /// <summary>
    ///     Completes the transition, promoting staged parameters to current and current to previous.
    /// </summary>
    void CommitTransition();

    /// <summary>
    ///     Determines whether the transition can be committed.
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> if <see cref="SetNextParameters"/> has been called since the last
    ///     <see cref="BeginTransition"/>; otherwise, <see langword="false"/>.
    /// </returns>
    bool CanCommitTransition();

    /// <summary>
    ///     Clears the staged parameters without affecting current or previous values.
    /// </summary>
    /// <remarks>
    ///     Useful for evaluating transitions without modifying actual parameter state.
    /// </remarks>
    void Clear();
}
