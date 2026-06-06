using ZCrew.StateCraft.Validation.Models;

namespace ZCrew.StateCraft.Validation;

internal sealed class StateMachineValidationContext<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    [Obsolete("Use Info")]
    public List<StateValidationModel<TState, TTransition>> States { get; } = [];

    [Obsolete("Use Info")]
    public List<TransitionValidationModel<TState, TTransition>> Transitions { get; } = [];

    public required IStateMachineInfo<TState, TTransition> Info { get; init; }

    public List<string> ValidationErrors { get; } = [];
}
