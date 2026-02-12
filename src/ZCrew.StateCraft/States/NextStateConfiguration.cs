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

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{StateValue}";
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

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{StateValue}<{typeof(T).FriendlyName}>";
    }
}

/// <summary>
///     Configuration for a two-parameter next state that holds conditions and produces a
///     <see cref="NextState{TState, TTransition, T1, T2}"/> at build time.
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
internal class NextStateConfiguration<TState, TTransition, T1, T2> : INextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T1, T2, bool>> conditions;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="NextStateConfiguration{TState, TTransition, T1, T2}"/> class.
    /// </summary>
    /// <param name="stateValue">The target state value.</param>
    /// <param name="conditions">The parameterized conditions that must be satisfied before transitioning.</param>
    public NextStateConfiguration(TState stateValue, IReadOnlyList<IAsyncFunc<T1, T2, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2)];

    /// <inheritdoc />
    public INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState<T1, T2>(StateValue);
        return new NextState<TState, TTransition, T1, T2>(state, this.conditions);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{StateValue}<{typeof(T1).FriendlyName}, {typeof(T2).FriendlyName}>";
    }
}

/// <summary>
///     Configuration for a three-parameter next state that holds conditions and produces a
///     <see cref="NextState{TState, TTransition, T1, T2, T3}"/> at build time.
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
internal class NextStateConfiguration<TState, TTransition, T1, T2, T3> : INextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T1, T2, T3, bool>> conditions;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="NextStateConfiguration{TState, TTransition, T1, T2, T3}"/> class.
    /// </summary>
    /// <param name="stateValue">The target state value.</param>
    /// <param name="conditions">The parameterized conditions that must be satisfied before transitioning.</param>
    public NextStateConfiguration(TState stateValue, IReadOnlyList<IAsyncFunc<T1, T2, T3, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3)];

    /// <inheritdoc />
    public INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState<T1, T2, T3>(StateValue);
        return new NextState<TState, TTransition, T1, T2, T3>(state, this.conditions);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{StateValue}<{typeof(T1).FriendlyName}, {typeof(T2).FriendlyName}, {typeof(T3).FriendlyName}>";
    }
}

/// <summary>
///     Configuration for a four-parameter next state that holds conditions and produces a
///     <see cref="NextState{TState, TTransition, T1, T2, T3, T4}"/> at build time.
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
internal class NextStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    : INextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T1, T2, T3, T4, bool>> conditions;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="NextStateConfiguration{TState, TTransition, T1, T2, T3, T4}"/> class.
    /// </summary>
    /// <param name="stateValue">The target state value.</param>
    /// <param name="conditions">The parameterized conditions that must be satisfied before transitioning.</param>
    public NextStateConfiguration(TState stateValue, IReadOnlyList<IAsyncFunc<T1, T2, T3, T4, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    /// <inheritdoc />
    public bool IsConditional => this.conditions.Count > 0;

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

    /// <inheritdoc />
    public INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState<T1, T2, T3, T4>(StateValue);
        return new NextState<TState, TTransition, T1, T2, T3, T4>(state, this.conditions);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{StateValue}<"
            + $"{typeof(T1).FriendlyName}, "
            + $"{typeof(T2).FriendlyName}, "
            + $"{typeof(T3).FriendlyName}, "
            + $"{typeof(T4).FriendlyName}>";
    }
}
