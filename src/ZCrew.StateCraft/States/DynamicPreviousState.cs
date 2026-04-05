using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States;

/// <summary>
///     Runtime representation of a previous state with an arbitrary number of parameters and no conditions.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <remarks>
///     This exists for scenarios where a state is dynamically referenced. In these cases, conditions aren't available
///     and type parameters don't matter.
/// </remarks>
internal sealed class DynamicPreviousState<TState, TTransition> : IPreviousState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DynamicPreviousState{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="state">The state that this previous state references.</param>
    public DynamicPreviousState(IState<TState, TTransition> state)
    {
        State = state;
    }

    /// <inheritdoc />
    public IState<TState, TTransition> State { get; }

    /// <inheritdoc />
    public bool IsConditional => false;

    /// <inheritdoc />
    public Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return State.ToString()!;
    }
}
