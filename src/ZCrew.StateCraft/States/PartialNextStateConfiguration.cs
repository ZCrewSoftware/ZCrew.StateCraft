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
}
