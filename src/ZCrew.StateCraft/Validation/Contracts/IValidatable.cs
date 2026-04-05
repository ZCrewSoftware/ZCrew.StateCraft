namespace ZCrew.StateCraft.Validation.Contracts;

/// <summary>
///     Represents a type that has meaningful information to add to a validation context.
/// </summary>
/// <typeparam name="TState"></typeparam>
/// <typeparam name="TTransition"></typeparam>
internal interface IValidatable<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Add information to the validation context to perform validation steps.
    /// </summary>
    /// <param name="context">The validation context to add to.</param>
    void AddToValidationContext(StateMachineValidationContext<TState, TTransition> context);
}
