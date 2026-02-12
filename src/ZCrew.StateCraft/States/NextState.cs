using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States;

/// <summary>
///     Runtime representation of a parameterless next state with its associated conditions.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal class NextState<TState, TTransition> : INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<bool>> conditions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NextState{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="state">The state that this next state references.</param>
    /// <param name="conditions">The conditions that must be satisfied before transitioning.</param>
    public NextState(IState<TState, TTransition> state, IReadOnlyList<IAsyncFunc<bool>> conditions)
    {
        State = state;
        this.conditions = conditions;
    }

    /// <inheritdoc />
    public IState<TState, TTransition> State { get; }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        foreach (var condition in this.conditions)
        {
            var result = await condition.InvokeAsync(token);
            if (!result)
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return State.ToString()!;
    }
}

/// <summary>
///     Runtime representation of a parameterized next state with its associated conditions.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T">The type of the parameter used to evaluate conditions.</typeparam>
internal class NextState<TState, TTransition, T> : INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T, bool>> conditions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NextState{TState, TTransition, T}"/> class.
    /// </summary>
    /// <param name="state">The state that this next state references.</param>
    /// <param name="conditions">The parameterized conditions that must be satisfied before transitioning.</param>
    public NextState(IState<TState, TTransition> state, IReadOnlyList<IAsyncFunc<T, bool>> conditions)
    {
        State = state;
        this.conditions = conditions;
    }

    /// <inheritdoc />
    public IState<TState, TTransition> State { get; }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter = parameters.GetNextParameter<T>();
        foreach (var condition in this.conditions)
        {
            var result = await condition.InvokeAsync(parameter, token);
            if (!result)
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return State.ToString()!;
    }
}

/// <summary>
///     Runtime representation of a two-parameter next state with its associated conditions.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter used to evaluate conditions.</typeparam>
/// <typeparam name="T2">The type of the second parameter used to evaluate conditions.</typeparam>
internal class NextState<TState, TTransition, T1, T2> : INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T1, T2, bool>> conditions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NextState{TState, TTransition, T1, T2}"/> class.
    /// </summary>
    /// <param name="state">The state that this next state references.</param>
    /// <param name="conditions">The parameterized conditions that must be satisfied before transitioning.</param>
    public NextState(IState<TState, TTransition> state, IReadOnlyList<IAsyncFunc<T1, T2, bool>> conditions)
    {
        State = state;
        this.conditions = conditions;
    }

    /// <inheritdoc />
    public IState<TState, TTransition> State { get; }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2) = parameters.GetNextParameters<T1, T2>();
        foreach (var condition in this.conditions)
        {
            var result = await condition.InvokeAsync(p1, p2, token);
            if (!result)
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return State.ToString()!;
    }
}

/// <summary>
///     Runtime representation of a three-parameter next state with its associated conditions.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter used to evaluate conditions.</typeparam>
/// <typeparam name="T2">The type of the second parameter used to evaluate conditions.</typeparam>
/// <typeparam name="T3">The type of the third parameter used to evaluate conditions.</typeparam>
internal class NextState<TState, TTransition, T1, T2, T3> : INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T1, T2, T3, bool>> conditions;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="NextState{TState, TTransition, T1, T2, T3}"/> class.
    /// </summary>
    /// <param name="state">The state that this next state references.</param>
    /// <param name="conditions">The parameterized conditions that must be satisfied before transitioning.</param>
    public NextState(IState<TState, TTransition> state, IReadOnlyList<IAsyncFunc<T1, T2, T3, bool>> conditions)
    {
        State = state;
        this.conditions = conditions;
    }

    /// <inheritdoc />
    public IState<TState, TTransition> State { get; }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2, p3) = parameters.GetNextParameters<T1, T2, T3>();
        foreach (var condition in this.conditions)
        {
            var result = await condition.InvokeAsync(p1, p2, p3, token);
            if (!result)
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return State.ToString()!;
    }
}

/// <summary>
///     Runtime representation of a four-parameter next state with its associated conditions.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter used to evaluate conditions.</typeparam>
/// <typeparam name="T2">The type of the second parameter used to evaluate conditions.</typeparam>
/// <typeparam name="T3">The type of the third parameter used to evaluate conditions.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter used to evaluate conditions.</typeparam>
internal class NextState<TState, TTransition, T1, T2, T3, T4> : INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T1, T2, T3, T4, bool>> conditions;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="NextState{TState, TTransition, T1, T2, T3, T4}"/> class.
    /// </summary>
    /// <param name="state">The state that this next state references.</param>
    /// <param name="conditions">The parameterized conditions that must be satisfied before transitioning.</param>
    public NextState(IState<TState, TTransition> state, IReadOnlyList<IAsyncFunc<T1, T2, T3, T4, bool>> conditions)
    {
        State = state;
        this.conditions = conditions;
    }

    /// <inheritdoc />
    public IState<TState, TTransition> State { get; }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2, p3, p4) = parameters.GetNextParameters<T1, T2, T3, T4>();
        foreach (var condition in this.conditions)
        {
            var result = await condition.InvokeAsync(p1, p2, p3, p4, token);
            if (!result)
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return State.ToString()!;
    }
}
