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
    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter = parameters.GetNextParameter<T>(0);
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
}
