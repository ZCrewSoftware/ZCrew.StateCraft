using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States;

/// <summary>
///     Configuration for a parameterless next state that holds conditions and produces a
///     <see cref="NextState{TState, TTransition}"/> at build time.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal class NextStateConfiguration<TState, TTransition> : INextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<bool>> conditions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NextStateConfiguration{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="stateValue">The target state value.</param>
    /// <param name="conditions">The conditions that must be satisfied before transitioning.</param>
    public NextStateConfiguration(TState stateValue, IReadOnlyList<IAsyncFunc<bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState(StateValue);
        return new NextState<TState, TTransition>(state, this.conditions);
    }
}

/// <summary>
///     Configuration for a parameterized next state that holds conditions and produces a
///     <see cref="NextState{TState, TTransition, T}"/> at build time.
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
internal class NextStateConfiguration<TState, TTransition, T> : INextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T, bool>> conditions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NextStateConfiguration{TState, TTransition, T}"/> class.
    /// </summary>
    /// <param name="stateValue">The target state value.</param>
    /// <param name="conditions">The parameterized conditions that must be satisfied before transitioning.</param>
    public NextStateConfiguration(TState stateValue, IReadOnlyList<IAsyncFunc<T, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    /// <inheritdoc />
    public INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState<T>(StateValue);
        return new NextState<TState, TTransition, T>(state, this.conditions);
    }
}
