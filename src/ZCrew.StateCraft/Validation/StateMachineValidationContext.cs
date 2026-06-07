namespace ZCrew.StateCraft.Validation;

internal sealed class StateMachineValidationContext<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public required IStateMachineInfo<TState, TTransition> Info { get; init; }

    public List<string> ValidationErrors { get; } = [];
}
