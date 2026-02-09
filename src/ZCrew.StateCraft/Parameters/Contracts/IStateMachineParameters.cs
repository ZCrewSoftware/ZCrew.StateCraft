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
///         <item><see cref="SetNextParameter{T}(T)"/> - Stages the parameters for the target state</item>
///         <item><see cref="CommitTransition"/> - Finalizes the transition, promoting next to current</item>
///     </list>
/// </para>
/// </remarks>
internal interface IStateMachineParameters
{
    /// <summary>
    ///     The status indicating which parameters have been set.
    /// </summary>
    StateMachineParametersFlags Status { get; }

    /// <summary>
    ///     The previous type parameters, if set.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.PreviousParametersSet"/> status.
    /// </exception>
    IReadOnlyList<Type> PreviousParameterTypes { get; }

    /// <summary>
    ///     The current type parameters, if set.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.CurrentParametersSet"/> status.
    /// </exception>
    IReadOnlyList<Type> CurrentParameterTypes { get; }

    /// <summary>
    ///     The next type parameters, if set.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.NextParametersSet"/> status.
    /// </exception>
    IReadOnlyList<Type> NextParameterTypes { get; }

    /// <summary>
    ///     Stages empty parameters for a parameterless transition.
    /// </summary>
    void SetEmptyNextParameters();

    /// <summary>
    ///     Stages a single parameter for the next state.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="nextParameter">
    ///     The parameter to stage. This becomes current upon <see cref="CommitTransition"/>.
    /// </param>
    void SetNextParameter<T>(T nextParameter);

    /// <summary>
    ///     Stages two parameters for the next state.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="parameter1">The first parameter to stage.</param>
    /// <param name="parameter2">The second parameter to stage.</param>
    void SetNextParameters<T1, T2>(T1 parameter1, T2 parameter2);

    /// <summary>
    ///     Stages three parameters for the next state.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <param name="parameter1">The first parameter to stage.</param>
    /// <param name="parameter2">The second parameter to stage.</param>
    /// <param name="parameter3">The third parameter to stage.</param>
    void SetNextParameters<T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3);

    /// <summary>
    ///     Stages four parameters for the next state.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <param name="parameter1">The first parameter to stage.</param>
    /// <param name="parameter2">The second parameter to stage.</param>
    /// <param name="parameter3">The third parameter to stage.</param>
    /// <param name="parameter4">The fourth parameter to stage.</param>
    void SetNextParameters<T1, T2, T3, T4>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4);

    /// <summary>
    ///     Retrieves a single parameter from the previous state.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <returns>The parameter value cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.PreviousParametersSet"/> status, or if the
    ///     parameter count is less than 1.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If the parameter cannot be cast to <typeparamref name="T"/>.
    /// </exception>
    T GetPreviousParameter<T>();

    /// <summary>
    ///     Retrieves two parameters from the previous state.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.PreviousParametersSet"/> status, or if the
    ///     parameter count is less than 2.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2) GetPreviousParameters<T1, T2>();

    /// <summary>
    ///     Retrieves three parameters from the previous state.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <typeparam name="T3">The expected type of the third parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.PreviousParametersSet"/> status, or if the
    ///     parameter count is less than 3.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2, T3) GetPreviousParameters<T1, T2, T3>();

    /// <summary>
    ///     Retrieves four parameters from the previous state.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <typeparam name="T3">The expected type of the third parameter.</typeparam>
    /// <typeparam name="T4">The expected type of the fourth parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.PreviousParametersSet"/> status, or if the
    ///     parameter count is less than 4.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2, T3, T4) GetPreviousParameters<T1, T2, T3, T4>();

    /// <summary>
    ///     Retrieves a single parameter from the current state.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <returns>The parameter value cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.CurrentParametersSet"/> status, or if the
    ///     parameter count is less than 1.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If the parameter cannot be cast to <typeparamref name="T"/>.
    /// </exception>
    T GetCurrentParameter<T>();

    /// <summary>
    ///     Retrieves two parameters from the current state.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.CurrentParametersSet"/> status, or if the
    ///     parameter count is less than 2.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2) GetCurrentParameters<T1, T2>();

    /// <summary>
    ///     Retrieves three parameters from the current state.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <typeparam name="T3">The expected type of the third parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.CurrentParametersSet"/> status, or if the
    ///     parameter count is less than 3.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2, T3) GetCurrentParameters<T1, T2, T3>();

    /// <summary>
    ///     Retrieves four parameters from the current state.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <typeparam name="T3">The expected type of the third parameter.</typeparam>
    /// <typeparam name="T4">The expected type of the fourth parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.CurrentParametersSet"/> status, or if the
    ///     parameter count is less than 4.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2, T3, T4) GetCurrentParameters<T1, T2, T3, T4>();

    /// <summary>
    ///     Retrieves a single staged parameter that will become current upon commit.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <returns>The parameter value cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.NextParametersSet"/> status, or if the
    ///     parameter count is less than 1.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If the parameter cannot be cast to <typeparamref name="T"/>.
    /// </exception>
    T GetNextParameter<T>();

    /// <summary>
    ///     Retrieves two staged parameters that will become current upon commit.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.NextParametersSet"/> status, or if the
    ///     parameter count is less than 2.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2) GetNextParameters<T1, T2>();

    /// <summary>
    ///     Retrieves three staged parameters that will become current upon commit.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <typeparam name="T3">The expected type of the third parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.NextParametersSet"/> status, or if the
    ///     parameter count is less than 3.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2, T3) GetNextParameters<T1, T2, T3>();

    /// <summary>
    ///     Retrieves four staged parameters that will become current upon commit.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <typeparam name="T3">The expected type of the third parameter.</typeparam>
    /// <typeparam name="T4">The expected type of the fourth parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="Status"/> does not indicate the
    ///     <see cref="StateMachineParametersFlags.NextParametersSet"/> status, or if the
    ///     parameter count is less than 4.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2, T3, T4) GetNextParameters<T1, T2, T3, T4>();

    /// <summary>
    ///     Begins a transition by capturing the current parameters for potential rollback.
    /// </summary>
    /// <remarks>
    ///     Must be called before <see cref="SetNextParameter{T}(T)"/> and
    ///     <see cref="CommitTransition"/>.
    ///     If an error occurs after this call, use <see cref="RollbackTransition"/> to
    ///     restore the previous state.
    /// </remarks>
    void BeginTransition();

    /// <summary>
    ///     Reverts a transition in progress, restoring the state captured by
    ///     <see cref="BeginTransition"/>.
    /// </summary>
    void RollbackTransition();

    /// <summary>
    ///     Completes the transition, promoting staged parameters to current and current
    ///     to previous.
    /// </summary>
    void CommitTransition();

    /// <summary>
    ///     Clears the staged parameters without affecting current or previous values.
    /// </summary>
    /// <remarks>
    ///     Useful for evaluating transitions without modifying actual parameter state.
    /// </remarks>
    void Clear();
}
