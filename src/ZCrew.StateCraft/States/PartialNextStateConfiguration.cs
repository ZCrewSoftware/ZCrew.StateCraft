using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.States;

internal class PartialNextStateConfiguration<TState, TTransition> : IPartialNextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<bool>> conditions = [];

    public void Add(Func<bool> condition)
    {
        this.conditions.Add(condition.AsAsyncFunc());
    }

    public INextStateConfiguration<TState, TTransition> WithState(TState stateValue)
    {
        return new NextStateConfiguration<TState, TTransition>(stateValue, this.conditions);
    }
}
