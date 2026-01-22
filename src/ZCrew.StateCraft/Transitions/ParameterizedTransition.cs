using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.Transitions;

/// <summary>
///     A parameterized transition from a parameterless state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
[DebuggerDisplay("{ToDisplayString()}")]
internal class ParameterizedTransition<TState, TTransition, TNext>
    : IParameterizedTransition<TState, TTransition, TNext>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<bool>> parameterlessConditions;
    private readonly IReadOnlyList<IAsyncFunc<TNext, bool>> nextStateConditions;
    private readonly Lazy<IParameterizedState<TState, TTransition, TNext>> nextStateCache;

    public ParameterizedTransition(
        IParameterlessState<TState, TTransition> previousState,
        TTransition transitionValue,
        TState nextStateValue,
        IReadOnlyList<IAsyncFunc<bool>> parameterlessConditions,
        IReadOnlyList<IAsyncFunc<TNext, bool>> nextStateConditions
    )
    {
        PreviousState = previousState;
        TransitionValue = transitionValue;
        NextStateValue = nextStateValue;
        this.parameterlessConditions = parameterlessConditions;
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
    public IReadOnlyList<Type> PreviousStateTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc />
    public async Task Transition(CancellationToken token)
    {
        var parameter = this.StateMachine.GetNextParameter<TState, TTransition, TNext>();
        this.StateMachine.Tracker?.Transitioned(this, parameter);
        await this.StateMachine.StateChange(PreviousStateValue, TransitionValue, NextStateValue, token);
        await this.nextStateCache.Value.StateChange(PreviousStateValue, TransitionValue, parameter, token);
    }

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(TNext nextParameter, CancellationToken token)
    {
        return await this.StateMachine.RunWithExceptionHandling(
            async () =>
            {
                // Evaluate parameterless conditions first
                foreach (var condition in this.parameterlessConditions)
                {
                    if (!await condition.InvokeAsync(token))
                    {
                        return false;
                    }
                }

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
        return $"Transition: {ToDisplayString()}";
    }

    private string ToDisplayString()
    {
        return $"{TransitionValue}({PreviousStateValue}) → {NextStateValue}<{typeof(TNext).FriendlyName}>";
    }
}

/// <summary>
///     A parameterized transition from a parameterized state.
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
internal class ParameterizedTransition<TState, TTransition, TPrevious, TNext>
    : IParameterizedTransition<TState, TTransition, TPrevious, TNext>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousStateConditions;
    private readonly IReadOnlyList<IAsyncFunc<TNext, bool>> nextStateConditions;
    private readonly Lazy<IParameterizedState<TState, TTransition, TNext>> nextStateCache;

    public ParameterizedTransition(
        IParameterizedState<TState, TTransition, TPrevious> previousState,
        TTransition transitionValue,
        TState nextStateValue,
        IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousStateConditions,
        IReadOnlyList<IAsyncFunc<TNext, bool>> nextStateConditions
    )
    {
        PreviousState = previousState;
        TransitionValue = transitionValue;
        NextStateValue = nextStateValue;
        this.previousStateConditions = previousStateConditions;
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
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc />
    public async Task Transition(CancellationToken token)
    {
        var parameter = this.StateMachine.GetNextParameter<TState, TTransition, TNext>();
        this.StateMachine.Tracker?.Transitioned(this, parameter);
        await this.StateMachine.StateChange(PreviousStateValue, TransitionValue, NextStateValue, token);
        await this.nextStateCache.Value.StateChange(PreviousStateValue, TransitionValue, parameter, token);
    }

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(
        TPrevious previousParameter,
        TNext nextParameter,
        CancellationToken token
    )
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
        return $"Transition: {ToDisplayString()}";
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
