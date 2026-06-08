using System.Runtime.CompilerServices;

namespace ZCrew.StateCraft;

/// <summary>
///     Configures an inverted transition (whose destination state has four parameters) from all (or most) states,
///     with the ability to exclude specific states and to register handlers that run when the transition is performed.
///     This interface also extends <see cref="ITransitionConfiguration{TState, TTransition}"/> so the configuration
///     chain can terminate without calling <see cref="Except(TState)"/>.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TNext1">The type of the first parameter for the destination state.</typeparam>
/// <typeparam name="TNext2">The type of the second parameter for the destination state.</typeparam>
/// <typeparam name="TNext3">The type of the third parameter for the destination state.</typeparam>
/// <typeparam name="TNext4">The type of the fourth parameter for the destination state.</typeparam>
public interface IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>
    : ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IFromAllStatesTransitionConfiguration{TState, TTransition}.Except(TState)"/>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> Except(TState state);

    /// <inheritdoc cref="IFromAllStatesTransitionConfiguration{TState, TTransition}.Except{TPrevious}(TState)"/>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> Except<TPrevious>(
        TState state
    );

    /// <inheritdoc cref="IFromAllStatesTransitionConfiguration{TState, TTransition}.Except{TPrevious1, TPrevious2}(TState)"/>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> Except<
        TPrevious1,
        TPrevious2
    >(TState state);

    /// <inheritdoc cref="IFromAllStatesTransitionConfiguration{TState, TTransition}.Except{TPrevious1, TPrevious2, TPrevious3}(TState)"/>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> Except<
        TPrevious1,
        TPrevious2,
        TPrevious3
    >(TState state);

    /// <inheritdoc cref="IFromAllStatesTransitionConfiguration{TState, TTransition}.Except{TPrevious1, TPrevious2, TPrevious3, TPrevious4}(TState)"/>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> Except<
        TPrevious1,
        TPrevious2,
        TPrevious3,
        TPrevious4
    >(TState state);

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when this transition is performed.
    ///     The handler receives the parameter values that were passed to the destination state.
    /// </summary>
    /// <param name="handler">The delegate to call when this transition is performed.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, the caller's expression for
    ///     <paramref name="handler"/> is captured automatically.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> OnTransition(
        Action<TNext1, TNext2, TNext3, TNext4> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when this transition is performed.
    ///     The handler receives the parameter values that were passed to the destination state.
    /// </summary>
    /// <param name="handler">The delegate to call when this transition is performed.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, the caller's expression for
    ///     <paramref name="handler"/> is captured automatically.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> OnTransition(
        Func<TNext1, TNext2, TNext3, TNext4, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when this transition is performed.
    ///     The handler receives the parameter values that were passed to the destination state.
    /// </summary>
    /// <param name="handler">The delegate to call when this transition is performed.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, the caller's expression for
    ///     <paramref name="handler"/> is captured automatically.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> OnTransition(
        Func<TNext1, TNext2, TNext3, TNext4, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    );
}
