using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.States.Configuration;

/// <summary>
///     A partial next state configuration that collects parameterless conditions before a target state is chosen.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal interface IPartialNextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Adds a parameterless condition that must be satisfied for the transition to proceed.
    /// </summary>
    /// <param name="condition">The condition to add.</param>
    void Add(IAsyncFunc<bool> condition);

    /// <summary>
    ///     Completes the partial configuration by specifying the target state, producing a full
    ///     <see cref="INextStateConfiguration{TState, TTransition}"/>.
    /// </summary>
    /// <param name="stateValue">The target state value.</param>
    /// <returns>The completed next state configuration.</returns>
    INextStateConfiguration<TState, TTransition> WithState(TState stateValue);
}

/// <summary>
///     A partial next state configuration that collects single-parameter conditions before a target state is chosen.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T">The type of the parameter passed to conditions.</typeparam>
internal interface IPartialNextStateConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Adds a single-parameter condition that must be satisfied for the transition to proceed.
    /// </summary>
    /// <param name="condition">The condition to add.</param>
    void Add(IAsyncFunc<T, bool> condition);

    /// <summary>
    ///     Completes the partial configuration by specifying the target state, producing a full
    ///     <see cref="INextStateConfiguration{TState, TTransition}"/>.
    /// </summary>
    /// <param name="stateValue">The target state value.</param>
    /// <returns>The completed next state configuration.</returns>
    INextStateConfiguration<TState, TTransition> WithState(TState stateValue);
}

/// <summary>
///     A partial next state configuration that collects two-parameter conditions before a target state is chosen.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter passed to conditions.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to conditions.</typeparam>
internal interface IPartialNextStateConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Adds a two-parameter condition that must be satisfied for the transition to proceed.
    /// </summary>
    /// <param name="condition">The condition to add.</param>
    void Add(IAsyncFunc<T1, T2, bool> condition);

    /// <summary>
    ///     Completes the partial configuration by specifying the target state, producing a full
    ///     <see cref="INextStateConfiguration{TState, TTransition}"/>.
    /// </summary>
    /// <param name="stateValue">The target state value.</param>
    /// <returns>The completed next state configuration.</returns>
    INextStateConfiguration<TState, TTransition> WithState(TState stateValue);
}

/// <summary>
///     A partial next state configuration that collects three-parameter conditions before a target state is chosen.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter passed to conditions.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to conditions.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to conditions.</typeparam>
internal interface IPartialNextStateConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Adds a three-parameter condition that must be satisfied for the transition to proceed.
    /// </summary>
    /// <param name="condition">The condition to add.</param>
    void Add(IAsyncFunc<T1, T2, T3, bool> condition);

    /// <summary>
    ///     Completes the partial configuration by specifying the target state, producing a full
    ///     <see cref="INextStateConfiguration{TState, TTransition}"/>.
    /// </summary>
    /// <param name="stateValue">The target state value.</param>
    /// <returns>The completed next state configuration.</returns>
    INextStateConfiguration<TState, TTransition> WithState(TState stateValue);
}

/// <summary>
///     A partial next state configuration that collects four-parameter conditions before a target state is chosen.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter passed to conditions.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to conditions.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to conditions.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter passed to conditions.</typeparam>
internal interface IPartialNextStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Adds a four-parameter condition that must be satisfied for the transition to proceed.
    /// </summary>
    /// <param name="condition">The condition to add.</param>
    void Add(IAsyncFunc<T1, T2, T3, T4, bool> condition);

    /// <summary>
    ///     Completes the partial configuration by specifying the target state, producing a full
    ///     <see cref="INextStateConfiguration{TState, TTransition}"/>.
    /// </summary>
    /// <param name="stateValue">The target state value.</param>
    /// <returns>The completed next state configuration.</returns>
    INextStateConfiguration<TState, TTransition> WithState(TState stateValue);
}
