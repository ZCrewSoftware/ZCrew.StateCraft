using System.Diagnostics;
using ZCrew.Extensions.Tasks;
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
/// <typeparam name="TPrevious">The type of the parameter for the previous state.</typeparam>
/// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
[DebuggerDisplay("{ToDisplayString()}")]
internal class MappedTransition<TState, TTransition, TPrevious, TNext>
    : IParameterlessTransition<TState, TTransition, TPrevious>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousStateConditions;
    private readonly Func<TPrevious, TNext> mappingFunction;
    private readonly IReadOnlyList<IAsyncFunc<TNext, bool>> nextStateConditions;
    private readonly Lazy<IParameterizedState<TState, TTransition, TNext>> nextStateCache;

    public MappedTransition(
        IParameterizedState<TState, TTransition, TPrevious> previousState,
        TTransition transitionValue,
        TState nextStateValue,
        IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousStateConditions,
        Func<TPrevious, TNext> mappingFunction,
        IReadOnlyList<IAsyncFunc<TNext, bool>> nextStateConditions
    )
    {
        PreviousState = previousState;
        TransitionValue = transitionValue;
        NextStateValue = nextStateValue;
        this.previousStateConditions = previousStateConditions;
        this.mappingFunction = mappingFunction;
        this.nextStateConditions = nextStateConditions;
        this.nextStateCache = new(() => previousState.StateMachine.StateTable.LookupState<TNext>(nextStateValue));
    }

    /// <inheritdoc />
    public TState PreviousStateValue => PreviousState.StateValue;

    /// <inheritdoc />
    public IState<TState, TTransition> PreviousState { get; }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public TState NextStateValue { get; }

    /// <inheritdoc />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IState<TState, TTransition> NextState => this.nextStateCache.Value;

    /// <inheritdoc />
    public IReadOnlyList<Type> PreviousStateTypeParameters { get; } = [typeof(TPrevious)];

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc />
    public async Task Transition(CancellationToken token)
    {
        var previousParameter = this.StateMachine.GetPreviousParameter<TState, TTransition, TPrevious>();
        var nextParameter = await this.StateMachine.RunWithExceptionHandling(
            () => Task.FromResult(this.mappingFunction(previousParameter)),
            token
        );
        this.StateMachine.Tracker?.Transitioned(this, nextParameter);
        this.StateMachine.NextParameter = nextParameter;
        await this.StateMachine.StateChange(PreviousStateValue, TransitionValue, NextStateValue, token);
        await this.nextStateCache.Value.StateChange(PreviousStateValue, TransitionValue, nextParameter, token);
    }

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(TPrevious previousParameter, CancellationToken token)
    {
        return await this.StateMachine.RunWithExceptionHandling(
            async () =>
            {
                // Evaluate conditions that use the previous state's parameter first
                foreach (var condition in this.previousStateConditions)
                {
                    if (!await condition.InvokeAsync(previousParameter, token))
                    {
                        return false;
                    }
                }

                // Avoid mapping if there are no other conditions
                if (this.nextStateConditions.Count == 0)
                {
                    return true;
                }

                var nextParameter = await this.StateMachine.RunWithExceptionHandling(
                    () => Task.FromResult(this.mappingFunction(previousParameter)),
                    token
                );

                // Evaluate conditions that use the next state's parameter
                foreach (var condition in this.nextStateConditions)
                {
                    if (!await condition.InvokeAsync(nextParameter, token))
                    {
                        return false;
                    }
                }

                return true;
            },
            token
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Mapped Transition: {ToDisplayString()}";
    }

    private string ToDisplayString()
    {
        if (PreviousStateValue.Equals(NextStateValue) && typeof(TPrevious) == typeof(TNext))
        {
            return $"{TransitionValue}({PreviousStateValue}<{typeof(TPrevious).FriendlyName}>) ↩";
        }

        return $"{TransitionValue}({PreviousStateValue}<{typeof(TPrevious).FriendlyName}>) → {NextStateValue}<{typeof(TNext).FriendlyName}>";
    }
}
