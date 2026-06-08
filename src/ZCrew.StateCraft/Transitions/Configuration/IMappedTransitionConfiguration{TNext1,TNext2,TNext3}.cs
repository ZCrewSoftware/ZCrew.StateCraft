using System.Runtime.CompilerServices;

namespace ZCrew.StateCraft;

/// <summary>
///     Configures a mapped transition that produces three parameters for the next state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TNext1">The type of the first mapped parameter for the next state.</typeparam>
/// <typeparam name="TNext2">The type of the second mapped parameter for the next state.</typeparam>
/// <typeparam name="TNext3">The type of the third mapped parameter for the next state.</typeparam>
public interface IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,bool}, string?)"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> If(
        Func<TNext1, TNext2, TNext3, bool> condition,
        [CallerArgumentExpression(nameof(condition))] string? descriptor = null
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,CancellationToken,Task{bool}}, string?)"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> If(
        Func<TNext1, TNext2, TNext3, CancellationToken, Task<bool>> condition,
        [CallerArgumentExpression(nameof(condition))] string? descriptor = null
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,CancellationToken,ValueTask{bool}}, string?)"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> If(
        Func<TNext1, TNext2, TNext3, CancellationToken, ValueTask<bool>> condition,
        [CallerArgumentExpression(nameof(condition))] string? descriptor = null
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.OnTransition(Action{TNext}, string?)"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> OnTransition(
        Action<TNext1, TNext2, TNext3> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.OnTransition(Func{TNext,CancellationToken,Task}, string?)"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> OnTransition(
        Func<TNext1, TNext2, TNext3, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.OnTransition(Func{TNext,CancellationToken,ValueTask}, string?)"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> OnTransition(
        Func<TNext1, TNext2, TNext3, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.To"/>
    ITransitionConfiguration<TState, TTransition> To(TState state);

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.ToSameState"/>
    ITransitionConfiguration<TState, TTransition> ToSameState();
}
