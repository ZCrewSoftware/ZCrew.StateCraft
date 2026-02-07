using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.States;

internal class PartialNextStateConfiguration<TState, TTransition> : IPartialNextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<bool>> conditions = [];

    public void Add(IAsyncFunc<bool> condition)
    {
        this.conditions.Add(condition);
    }

    public INextStateConfiguration<TState, TTransition> WithState(TState stateValue)
    {
        return new NextStateConfiguration<TState, TTransition>(stateValue, this.conditions);
    }
}

internal class PartialNextStateConfiguration<TState, TTransition, T>
    : IPartialNextStateConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<T, bool>> conditions = [];

    public void Add(IAsyncFunc<T, bool> condition)
    {
        this.conditions.Add(condition);
    }

    public INextStateConfiguration<TState, TTransition> WithState(TState stateValue)
    {
        return new NextStateConfiguration<TState, TTransition, T>(stateValue, this.conditions);
    }
}
