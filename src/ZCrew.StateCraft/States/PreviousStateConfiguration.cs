using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States;

/// <summary>
///     Configuration for a parameterless previous state that collects conditions and produces a
///     <see cref="PreviousState{TState, TTransition}"/> at build time.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal class PreviousStateConfiguration<TState, TTransition> : IPartialPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<bool>> conditions = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="PreviousStateConfiguration{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="stateValue">The source state value.</param>
    public PreviousStateConfiguration(TState stateValue)
    {
        StateValue = stateValue;
    }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public IPreviousState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState(StateValue);
        return new PreviousState<TState, TTransition>(state, this.conditions);
    }

    /// <inheritdoc />
    public void Add(IAsyncFunc<bool> condition)
    {
        this.conditions.Add(condition);
    }
}

/// <summary>
///     Configuration for a parameterized previous state that collects conditions and produces a
///     <see cref="PreviousState{TState, TTransition, T}"/> at build time.
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
internal class PreviousStateConfiguration<TState, TTransition, T>
    : IPartialPreviousStateConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<T, bool>> conditions = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="PreviousStateConfiguration{TState, TTransition, T}"/> class.
    /// </summary>
    /// <param name="stateValue">The source state value.</param>
    public PreviousStateConfiguration(TState stateValue)
    {
        StateValue = stateValue;
    }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    /// <inheritdoc />
    public IPreviousState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState<T>(StateValue);
        return new PreviousState<TState, TTransition, T>(state, this.conditions);
    }

    /// <inheritdoc />
    public void Add(IAsyncFunc<T, bool> condition)
    {
        this.conditions.Add(condition);
    }
}
