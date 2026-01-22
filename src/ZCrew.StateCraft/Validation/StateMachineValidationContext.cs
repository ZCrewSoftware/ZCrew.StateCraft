namespace ZCrew.StateCraft.Validation;

internal sealed class StateMachineValidationContext<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public required IStateMachineConfiguration<TState, TTransition> Configuration { get; set; }

    public readonly List<string> ValidationErrors = [];
}
