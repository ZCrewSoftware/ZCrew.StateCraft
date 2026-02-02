using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States;

internal class PreviousStateConfiguration<TState, TTransition>
    : IPreviousStateConfiguration<TState, TTransition>,
        IPartialPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<bool>> conditions = [];

    public PreviousStateConfiguration(TState stateValue)
    {
        StateValue = stateValue;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [];

    public IPreviousState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState(StateValue);
        return new PreviousState<TState, TTransition>(state, this.conditions);
    }

    public void Add(Func<bool> condition)
    {
        this.conditions.Add(condition.AsAsyncFunc());
    }

    public void Add(Func<CancellationToken, Task<bool>> condition)
    {
        this.conditions.Add(condition.AsAsyncFunc());
    }

    public void Add(Func<CancellationToken, ValueTask<bool>> condition)
    {
        this.conditions.Add(condition.AsAsyncFunc());
    }
}
