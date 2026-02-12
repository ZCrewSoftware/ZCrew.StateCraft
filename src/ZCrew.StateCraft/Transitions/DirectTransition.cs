using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.Transitions;

/// <summary>
///     A direct transition from a state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal class DirectTransition<TState, TTransition> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IStateMachine<TState, TTransition> stateMachine;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DirectTransition{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="previous">The previous state in the transition.</param>
    /// <param name="next">The next state in the transition.</param>
    /// <param name="transitionValue">The transition value that triggers this transition.</param>
    /// <param name="stateMachine">The state machine that owns this transition.</param>
    public DirectTransition(
        IPreviousState<TState, TTransition> previous,
        INextState<TState, TTransition> next,
        TTransition transitionValue,
        IStateMachine<TState, TTransition> stateMachine
    )
    {
        Previous = previous;
        Next = next;
        TransitionValue = transitionValue;
        this.stateMachine = stateMachine;
    }

    /// <inheritdoc />
    public IPreviousState<TState, TTransition> Previous { get; }

    /// <inheritdoc />
    public INextState<TState, TTransition> Next { get; }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionTypeParameters => Next.State.TypeParameters;

    /// <inheritdoc />
    public async Task Transition(IStateMachineParameters parameters, CancellationToken token)
    {
        this.stateMachine.Tracker?.Transitioned(this);
        await this.stateMachine.StateChange(Previous.State.StateValue, TransitionValue, Next.State.StateValue, token);
        await Next.State.StateChange(Previous.State.StateValue, TransitionValue, parameters, token);
    }

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        var previousStateCondition = await this.stateMachine.ExceptionBehavior.CallCondition(
            t => Previous.EvaluateConditions(parameters, t),
            token
        );
        if (!previousStateCondition)
        {
            return false;
        }
        var nextStateCondition = await this.stateMachine.ExceptionBehavior.CallCondition(
            t => Next.EvaluateConditions(parameters, t),
            token
        );
        return nextStateCondition;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (
            Previous.State.StateValue.Equals(Next.State.StateValue)
            && Previous.State.TypeParameters.SequenceEqual(Next.State.TypeParameters)
        )
        {
            return $"{TransitionValue}({Previous}) ↩";
        }

        return $"{TransitionValue}({Previous}) → {Next}";
    }
}
