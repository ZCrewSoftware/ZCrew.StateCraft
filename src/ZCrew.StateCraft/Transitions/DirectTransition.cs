using System.Diagnostics;
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
[DebuggerDisplay("{ToDisplayString()}")]
internal class DirectTransition<TState, TTransition> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IStateMachine<TState, TTransition> stateMachine;

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
        return await this.stateMachine.RunWithExceptionHandling(
            async () =>
            {
                var previousStateCondition = await Previous.EvaluateConditions(parameters, token);
                if (!previousStateCondition)
                {
                    return false;
                }
                var nextStateCondition = await Next.EvaluateConditions(parameters, token);
                return nextStateCondition;
            },
            token
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {ToDisplayString()}";
    }

    private string ToDisplayString()
    {
        // TODO MWZ: this needs refactoring since #31 really botched it
        var previousStateParameters =
            Previous.State.TypeParameters.Count == 0
                ? string.Empty
                : $"<{string.Join(", ", Previous.State.TypeParameters.Select(type => type.FriendlyName))}>";
        if (
            Previous.State.StateValue.Equals(Next.State.StateValue)
            && Previous.State.TypeParameters.SequenceEqual(Next.State.TypeParameters)
        )
        {
            return $"{TransitionValue}({Previous.State.StateValue}{previousStateParameters}) ↩";
        }
        var nextStateParameters =
            Next.State.TypeParameters.Count == 0
                ? string.Empty
                : $"<{string.Join(", ", Next.State.TypeParameters.Select(type => type.FriendlyName))}>";
        return $"{TransitionValue}({Previous.State.StateValue}{previousStateParameters}) → {Next.State.StateValue}{nextStateParameters}";
    }
}
