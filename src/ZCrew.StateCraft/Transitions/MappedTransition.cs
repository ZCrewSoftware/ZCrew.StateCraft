using System.Diagnostics;
using ZCrew.StateCraft.Mapping.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.Transitions;

/// <summary>
///     A parameterized transition from a parameterized state that applies a mapping function the previous parameter.
///     This means that the user does not need to provide a parameter for this transition.
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
internal class MappedTransition<TState, TTransition> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IMappingFunction mappingFunction;
    private readonly IStateMachine<TState, TTransition> stateMachine;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MappedTransition{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="previous">The previous state in the transition.</param>
    /// <param name="next">The next state in the transition.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    /// <param name="mappingFunction">The mapping function that transforms the previous parameter.</param>
    /// <param name="stateMachine">The state machine that owns this transition.</param>
    public MappedTransition(
        IPreviousState<TState, TTransition> previous,
        INextState<TState, TTransition> next,
        TTransition transition,
        IMappingFunction mappingFunction,
        IStateMachine<TState, TTransition> stateMachine
    )
    {
        Previous = previous;
        Next = next;
        TransitionValue = transition;
        this.mappingFunction = mappingFunction;
        this.stateMachine = stateMachine;
    }

    /// <inheritdoc />
    public IPreviousState<TState, TTransition> Previous { get; }

    /// <inheritdoc />
    public INextState<TState, TTransition> Next { get; }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc />
    public Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        return this.stateMachine.RunWithExceptionHandling(
            async () =>
            {
                // Evaluate conditions that use the previous state's parameter first
                var previousResult = await Previous.EvaluateConditions(parameters, token);
                if (!previousResult)
                {
                    return false;
                }

                // Avoid mapping if there are no other conditions
                // if (!Next.IsConditional)
                // {
                //     return true;
                // }

                await this.mappingFunction.Map(parameters, token);

                // Evaluate conditions that use the next state's parameter
                return await Next.EvaluateConditions(parameters, token);
            },
            token
        );
    }

    /// <inheritdoc />
    public async Task Transition(IStateMachineParameters parameters, CancellationToken token)
    {
        this.stateMachine.Tracker?.Transitioned(this);
        await this.mappingFunction.Map(parameters, token);
        await this.stateMachine.StateChange(Previous.State.StateValue, TransitionValue, Next.State.StateValue, token);
        await Next.State.StateChange(Previous.State.StateValue, TransitionValue, parameters, token);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Mapped Transition: {ToDisplayString()}";
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
