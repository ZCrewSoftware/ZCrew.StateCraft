using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.States;

/// <summary>
///     Collects parameterless conditions for a next state before the target state is chosen.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal class PartialNextStateConfiguration<TState, TTransition> : IPartialNextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<bool>> conditions = [];

    /// <inheritdoc />
    public void Add(IAsyncFunc<bool> condition)
    {
        this.conditions.Add(condition);
    }

    /// <inheritdoc />
    public INextStateConfiguration<TState, TTransition> WithState(TState stateValue)
    {
        return new NextStateConfiguration<TState, TTransition>(stateValue, this.conditions);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "?";
    }
}

/// <summary>
///     Collects parameterized conditions for a next state before the target state is chosen.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T">The type of the parameter passed to conditions.</typeparam>
internal class PartialNextStateConfiguration<TState, TTransition, T>
    : IPartialNextStateConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<T, bool>> conditions = [];

    /// <inheritdoc />
    public void Add(IAsyncFunc<T, bool> condition)
    {
        this.conditions.Add(condition);
    }

    /// <inheritdoc />
    public INextStateConfiguration<TState, TTransition> WithState(TState stateValue)
    {
        return new NextStateConfiguration<TState, TTransition, T>(stateValue, this.conditions);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"?<{typeof(T).FriendlyName}>";
    }
}

/// <summary>
///     Collects two-parameter conditions for a next state before the target state is chosen.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter passed to conditions.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to conditions.</typeparam>
internal class PartialNextStateConfiguration<TState, TTransition, T1, T2>
    : IPartialNextStateConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<T1, T2, bool>> conditions = [];

    /// <inheritdoc />
    public void Add(IAsyncFunc<T1, T2, bool> condition)
    {
        this.conditions.Add(condition);
    }

    /// <inheritdoc />
    public INextStateConfiguration<TState, TTransition> WithState(TState stateValue)
    {
        return new NextStateConfiguration<TState, TTransition, T1, T2>(stateValue, this.conditions);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"?<{typeof(T1).FriendlyName}, {typeof(T2).FriendlyName}>";
    }
}

/// <summary>
///     Collects three-parameter conditions for a next state before the target state is chosen.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter passed to conditions.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to conditions.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to conditions.</typeparam>
internal class PartialNextStateConfiguration<TState, TTransition, T1, T2, T3>
    : IPartialNextStateConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<T1, T2, T3, bool>> conditions = [];

    /// <inheritdoc />
    public void Add(IAsyncFunc<T1, T2, T3, bool> condition)
    {
        this.conditions.Add(condition);
    }

    /// <inheritdoc />
    public INextStateConfiguration<TState, TTransition> WithState(TState stateValue)
    {
        return new NextStateConfiguration<TState, TTransition, T1, T2, T3>(stateValue, this.conditions);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"?<{typeof(T1).FriendlyName}, {typeof(T2).FriendlyName}, {typeof(T3).FriendlyName}>";
    }
}

/// <summary>
///     Collects four-parameter conditions for a next state before the target state is chosen.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter passed to conditions.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to conditions.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to conditions.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter passed to conditions.</typeparam>
internal class PartialNextStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    : IPartialNextStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<T1, T2, T3, T4, bool>> conditions = [];

    /// <inheritdoc />
    public void Add(IAsyncFunc<T1, T2, T3, T4, bool> condition)
    {
        this.conditions.Add(condition);
    }

    /// <inheritdoc />
    public INextStateConfiguration<TState, TTransition> WithState(TState stateValue)
    {
        return new NextStateConfiguration<TState, TTransition, T1, T2, T3, T4>(stateValue, this.conditions);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"?<"
            + $"{typeof(T1).FriendlyName}, "
            + $"{typeof(T2).FriendlyName}, "
            + $"{typeof(T3).FriendlyName}, "
            + $"{typeof(T4).FriendlyName}>";
    }
}
