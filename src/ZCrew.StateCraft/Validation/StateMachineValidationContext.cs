using ZCrew.StateCraft.Validation.Models;

namespace ZCrew.StateCraft.Validation;

internal sealed class StateMachineValidationContext<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public List<StateValidationModel<TState, TTransition>> States { get; } = [];

    public List<TransitionValidationModel<TState, TTransition>> Transitions { get; } = [];

    public List<string> ValidationErrors { get; } = [];
}
