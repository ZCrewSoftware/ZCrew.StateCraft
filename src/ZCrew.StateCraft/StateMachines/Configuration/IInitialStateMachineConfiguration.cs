using System.Runtime.CompilerServices;

namespace ZCrew.StateCraft;

/// <summary>
///     Defines the initial state and then allows configuring the state machine.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IInitialStateMachineConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures the initial state of the machine to <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState(TState state);

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState(
        Func<TState> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState(
        Func<CancellationToken, Task<TState>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState(
        Func<CancellationToken, ValueTask<TState>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the initial state of the machine to <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter">The parameter for the initial state.</param>
    /// <typeparam name="T">The type of the initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(TState state, T parameter);

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameter.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T">The type of the initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(
        Func<(TState, T)> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T">The type of the initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(
        Func<CancellationToken, Task<(TState, T)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T">The type of the initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(
        Func<CancellationToken, ValueTask<(TState, T)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the initial state of the machine to <paramref name="state"/> with two parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first parameter for the initial state.</param>
    /// <param name="parameter2">The second parameter for the initial state.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        TState state,
        T1 parameter1,
        T2 parameter2
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and two parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<(TState, T1, T2)> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and two parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<CancellationToken, Task<(TState, T1, T2)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and two parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<CancellationToken, ValueTask<(TState, T1, T2)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the initial state of the machine to <paramref name="state"/> with three parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first parameter for the initial state.</param>
    /// <param name="parameter2">The second parameter for the initial state.</param>
    /// <param name="parameter3">The third parameter for the initial state.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        TState state,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and three parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<(TState, T1, T2, T3)> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and three parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<CancellationToken, Task<(TState, T1, T2, T3)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and three parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<CancellationToken, ValueTask<(TState, T1, T2, T3)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the initial state of the machine to <paramref name="state"/> with four parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first parameter for the initial state.</param>
    /// <param name="parameter2">The second parameter for the initial state.</param>
    /// <param name="parameter3">The third parameter for the initial state.</param>
    /// <param name="parameter4">The fourth parameter for the initial state.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        TState state,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and four parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<(TState, T1, T2, T3, T4)> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and four parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<CancellationToken, Task<(TState, T1, T2, T3, T4)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and four parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, the caller's expression for
    ///     <paramref name="stateProvider"/> is captured automatically.
    /// </param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<CancellationToken, ValueTask<(TState, T1, T2, T3, T4)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    );
}
