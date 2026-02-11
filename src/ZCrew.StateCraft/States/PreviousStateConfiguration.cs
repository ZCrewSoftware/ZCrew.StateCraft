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

/// <summary>
///     Configuration for a two-parameter previous state that collects conditions and produces a
///     <see cref="PreviousState{TState, TTransition, T1, T2}"/> at build time.
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
internal class PreviousStateConfiguration<TState, TTransition, T1, T2>
    : IPartialPreviousStateConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<T1, T2, bool>> conditions = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="PreviousStateConfiguration{TState, TTransition, T1, T2}"/> class.
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
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2)];

    /// <inheritdoc />
    public IPreviousState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState<T1, T2>(StateValue);
        return new PreviousState<TState, TTransition, T1, T2>(state, this.conditions);
    }

    /// <inheritdoc />
    public void Add(IAsyncFunc<T1, T2, bool> condition)
    {
        this.conditions.Add(condition);
    }
}

/// <summary>
///     Configuration for a three-parameter previous state that collects conditions and produces a
///     <see cref="PreviousState{TState, TTransition, T1, T2, T3}"/> at build time.
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
internal class PreviousStateConfiguration<TState, TTransition, T1, T2, T3>
    : IPartialPreviousStateConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<T1, T2, T3, bool>> conditions = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="PreviousStateConfiguration{TState, TTransition, T1, T2, T3}"/> class.
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
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3)];

    /// <inheritdoc />
    public IPreviousState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState<T1, T2, T3>(StateValue);
        return new PreviousState<TState, TTransition, T1, T2, T3>(state, this.conditions);
    }

    /// <inheritdoc />
    public void Add(IAsyncFunc<T1, T2, T3, bool> condition)
    {
        this.conditions.Add(condition);
    }
}

/// <summary>
///     Configuration for a four-parameter previous state that collects conditions and produces a
///     <see cref="PreviousState{TState, TTransition, T1, T2, T3, T4}"/> at build time.
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
internal class PreviousStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    : IPartialPreviousStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<T1, T2, T3, T4, bool>> conditions = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="PreviousStateConfiguration{TState, TTransition, T1, T2, T3, T4}"/> class.
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
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

    /// <inheritdoc />
    public IPreviousState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState<T1, T2, T3, T4>(StateValue);
        return new PreviousState<TState, TTransition, T1, T2, T3, T4>(state, this.conditions);
    }

    /// <inheritdoc />
    public void Add(IAsyncFunc<T1, T2, T3, T4, bool> condition)
    {
        this.conditions.Add(condition);
    }
}
