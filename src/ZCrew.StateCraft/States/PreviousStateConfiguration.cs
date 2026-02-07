using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States;

internal class PreviousStateConfiguration<TState, TTransition> : IPartialPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<bool>> conditions = [];

    public PreviousStateConfiguration(TState stateValue)
    {
        StateValue = stateValue;
    }

    public bool IsConditional => this.conditions.Count > 0;

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [];

    public IPreviousState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState(StateValue);
        return new PreviousState<TState, TTransition>(state, this.conditions);
    }

    public void Add(IAsyncFunc<bool> condition)
    {
        this.conditions.Add(condition);
    }
}

internal class PreviousStateConfiguration<TState, TTransition, T>
    : IPartialPreviousStateConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncFunc<T, bool>> conditions = [];

    public PreviousStateConfiguration(TState stateValue)
    {
        StateValue = stateValue;
    }

    public bool IsConditional => this.conditions.Count > 0;

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    public IPreviousState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState<T>(StateValue);
        return new PreviousState<TState, TTransition, T>(state, this.conditions);
    }

    public void Add(IAsyncFunc<T, bool> condition)
    {
        this.conditions.Add(condition);
    }
}
