using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.Transitions;

/// <summary>
///     A parameterless transition from a parameterless state.
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
internal class ParameterlessTransition<TState, TTransition> : IParameterlessTransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<bool>> conditions;
    private readonly Lazy<IParameterlessState<TState, TTransition>> nextStateCache;

    public ParameterlessTransition(
        IParameterlessState<TState, TTransition> previousState,
        TTransition transitionValue,
        TState nextStateValue,
        IReadOnlyList<IAsyncFunc<bool>> conditions
    )
    {
        PreviousState = previousState;
        TransitionValue = transitionValue;
        NextStateValue = nextStateValue;
        this.conditions = conditions;
        this.nextStateCache = new(() => previousState.StateMachine.StateTable.LookupState(nextStateValue));
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
    public IReadOnlyList<Type> PreviousStateTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [];

    /// <inheritdoc />
    public async Task Transition(CancellationToken token)
    {
        this.StateMachine.Tracker?.Transitioned(this);
        await this.StateMachine.StateChange(PreviousStateValue, TransitionValue, NextStateValue, token);
        await this.nextStateCache.Value.StateChange(PreviousStateValue, TransitionValue, token);
    }

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(CancellationToken token)
    {
        return await this.StateMachine.RunWithExceptionHandling(
            async () =>
            {
                foreach (var condition in this.conditions)
                {
                    if (!await condition.InvokeAsync(token))
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
        return $"Transition: {ToDisplayString()}";
    }

    private string ToDisplayString()
    {
        if (PreviousStateValue.Equals(NextStateValue))
        {
            return $"{TransitionValue}({PreviousStateValue}) ↩";
        }
        return $"{TransitionValue}({PreviousStateValue}) → {NextStateValue}";
    }
}

/// <summary>
///     A parameterless transition from a parameterized state.
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
[DebuggerDisplay("{ToDisplayString()}")]
internal class ParameterlessTransition<TState, TTransition, TPrevious>
    : IParameterlessTransition<TState, TTransition, TPrevious>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousStateConditions;
    private readonly IReadOnlyList<IAsyncFunc<bool>> parameterlessConditions;
    private readonly Lazy<IParameterlessState<TState, TTransition>> nextStateCache;

    public ParameterlessTransition(
        IParameterizedState<TState, TTransition, TPrevious> previousState,
        TTransition transitionValue,
        TState nextStateValue,
        IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousStateConditions,
        IReadOnlyList<IAsyncFunc<bool>> parameterlessConditions
    )
    {
        PreviousState = previousState;
        TransitionValue = transitionValue;
        NextStateValue = nextStateValue;
        this.previousStateConditions = previousStateConditions;
        this.parameterlessConditions = parameterlessConditions;
        this.nextStateCache = new(() => previousState.StateMachine.StateTable.LookupState(nextStateValue));
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
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [];

    /// <inheritdoc />
    public async Task Transition(CancellationToken token)
    {
        this.StateMachine.Tracker?.Transitioned(this);
        await this.StateMachine.StateChange(PreviousStateValue, TransitionValue, NextStateValue, token);
        await this.nextStateCache.Value.StateChange(PreviousStateValue, TransitionValue, token);
    }

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(TPrevious previousParameter, CancellationToken token)
    {
        return await this.StateMachine.RunWithExceptionHandling(
            async () =>
            {
                // Evaluate conditions that use the previous state's parameter
                foreach (var condition in this.previousStateConditions)
                {
                    if (!await condition.InvokeAsync(previousParameter, token))
                    {
                        return false;
                    }
                }

                // Evaluate parameterless conditions
                foreach (var condition in this.parameterlessConditions)
                {
                    if (!await condition.InvokeAsync(token))
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
        return $"Transition: {ToDisplayString()}";
    }

    private string ToDisplayString()
    {
        return $"{TransitionValue}({PreviousStateValue}<{typeof(TPrevious).FriendlyName}>) → {NextStateValue}";
    }
}
