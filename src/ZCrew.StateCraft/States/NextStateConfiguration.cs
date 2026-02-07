using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States;

internal class NextStateConfiguration<TState, TTransition> : INextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<bool>> conditions;

    public NextStateConfiguration(TState stateValue, IReadOnlyList<IAsyncFunc<bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public bool IsConditional => this.conditions.Count > 0;

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [];

    public INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState(StateValue);
        return new NextState<TState, TTransition>(state, this.conditions);
    }
}

internal class NextStateConfiguration<TState, TTransition, T> : INextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T, bool>> conditions;

    public NextStateConfiguration(TState stateValue, IReadOnlyList<IAsyncFunc<T, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public bool IsConditional => this.conditions.Count > 0;

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    public INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable)
    {
        var state = stateTable.LookupState<T>(StateValue);
        return new NextState<TState, TTransition, T>(state, this.conditions);
    }
}
