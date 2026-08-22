using ZCrew.StateCraft.Async.Contracts;
using ZCrew.StateCraft.Identities.Extensions;
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
internal class MappedTransition<TState, TTransition> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IMappingFunction mappingFunction;
    private readonly IStateMachine<TState, TTransition> stateMachine;
    private readonly IReadOnlyList<INextParametersHandler> onTransitionHandlers;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MappedTransition{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="previous">The previous state in the transition.</param>
    /// <param name="next">The next state in the transition.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    /// <param name="mappingFunction">The mapping function that transforms the previous parameter.</param>
    /// <param name="stateMachine">The state machine that owns this transition.</param>
    /// <param name="onTransitionHandlers">The <c>OnTransition</c> handlers.</param>
    public MappedTransition(
        IPreviousState<TState, TTransition> previous,
        INextState<TState, TTransition> next,
        TTransition transition,
        IMappingFunction mappingFunction,
        IStateMachine<TState, TTransition> stateMachine,
        IReadOnlyList<INextParametersHandler> onTransitionHandlers
    )
    {
        Previous = previous;
        Next = next;
        TransitionValue = transition;
        this.mappingFunction = mappingFunction;
        this.stateMachine = stateMachine;
        this.onTransitionHandlers = onTransitionHandlers;
    }

    /// <inheritdoc />
    public IPreviousState<TState, TTransition> Previous { get; }

    /// <inheritdoc />
    public INextState<TState, TTransition> Next { get; }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionParameterTypes { get; } = [];

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        // Evaluate conditions that use the previous state's parameter first
        var previousStateCondition = await this.stateMachine.ExceptionBehavior.CallCondition(
            t => Previous.EvaluateConditions(parameters, t),
            token
        );

        // Avoid mapping if there are no other conditions
        if (!previousStateCondition)
        {
            return false;
        }

        await this.stateMachine.ExceptionBehavior.CallMap(t => this.mappingFunction.Map(parameters, t), token);

        // Evaluate conditions that use the next state's parameter
        var nextStateCondition = await this.stateMachine.ExceptionBehavior.CallCondition(
            t => Next.EvaluateConditions(parameters, t),
            token
        );

        // If the post-condition failed, clear the stale mapped values from the parameter
        // slot so subsequent transitions in the lookup loop are not affected by the
        // type filter seeing leftover types from this mapping.
        if (!nextStateCondition)
        {
            parameters.SetEmptyNextParameters();
        }

        return nextStateCondition;
    }

    /// <inheritdoc />
    public async Task Transition(IStateMachineParameters parameters, CancellationToken token)
    {
        await EnsureNextParametersWereMapped(parameters, token);
        foreach (var handler in this.onTransitionHandlers)
        {
            await this.stateMachine.ExceptionBehavior.CallOnTransition(t => handler.Invoke(parameters, t), token);
        }
    }

    /// <inheritdoc />
    public async Task StateChange(IStateMachineParameters parameters, CancellationToken token)
    {
        await EnsureNextParametersWereMapped(parameters, token);
        await Next.State.StateChange(Previous.State.StateValue, TransitionValue, parameters, token);
    }

    /// <inheritdoc cref="ITransitionIdentity{TTransition}"/>
    public override string ToString()
    {
        return this.ToDisplayStringFromOneToOne(Previous.State, Next.State);
    }

    private Task EnsureNextParametersWereMapped(IStateMachineParameters parameters, CancellationToken token)
    {
        if (parameters.IsNextSet)
        {
            return Task.CompletedTask;
        }

        return this.stateMachine.ExceptionBehavior.CallMap(t => this.mappingFunction.Map(parameters, t), token);
    }
}
