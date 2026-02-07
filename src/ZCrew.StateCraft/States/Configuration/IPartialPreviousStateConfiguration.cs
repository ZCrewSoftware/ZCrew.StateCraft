using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.States.Configuration;

/// <summary>
///     A partial previous state configuration that collects parameterless conditions and extends
///     <see cref="IPreviousStateConfiguration{TState, TTransition}"/>.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal interface IPartialPreviousStateConfiguration<TState, TTransition>
    : IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Adds a parameterless condition that must be satisfied for the transition to proceed.
    /// </summary>
    /// <param name="condition">The condition to add.</param>
    void Add(IAsyncFunc<bool> condition);
}

/// <summary>
///     A partial previous state configuration that collects single-parameter conditions and extends
///     <see cref="IPreviousStateConfiguration{TState, TTransition}"/>.
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
internal interface IPartialPreviousStateConfiguration<TState, TTransition, T>
    : IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Adds a single-parameter condition that must be satisfied for the transition to proceed.
    /// </summary>
    /// <param name="condition">The condition to add.</param>
    void Add(IAsyncFunc<T, bool> condition);
}

/// <summary>
///     A partial previous state configuration that collects two-parameter conditions and extends
///     <see cref="IPreviousStateConfiguration{TState, TTransition}"/>.
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
internal interface IPartialPreviousStateConfiguration<TState, TTransition, T1, T2>
    : IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Adds a two-parameter condition that must be satisfied for the transition to proceed.
    /// </summary>
    /// <param name="condition">The condition to add.</param>
    void Add(IAsyncFunc<T1, T2, bool> condition);
}

/// <summary>
///     A partial previous state configuration that collects three-parameter conditions and extends
///     <see cref="IPreviousStateConfiguration{TState, TTransition}"/>.
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
internal interface IPartialPreviousStateConfiguration<TState, TTransition, T1, T2, T3>
    : IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Adds a three-parameter condition that must be satisfied for the transition to proceed.
    /// </summary>
    /// <param name="condition">The condition to add.</param>
    void Add(IAsyncFunc<T1, T2, T3, bool> condition);
}

/// <summary>
///     A partial previous state configuration that collects four-parameter conditions and extends
///     <see cref="IPreviousStateConfiguration{TState, TTransition}"/>.
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
internal interface IPartialPreviousStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    : IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Adds a four-parameter condition that must be satisfied for the transition to proceed.
    /// </summary>
    /// <param name="condition">The condition to add.</param>
    void Add(IAsyncFunc<T1, T2, T3, T4, bool> condition);
}
