using System.Text;
using ZCrew.StateCraft.Validation.Contracts;

namespace ZCrew.StateCraft.Validation;

/// <summary>
///     Performs validation on state machine components.
/// </summary>
internal static class StateMachineValidation
{
    /// <summary>
    ///     Perform validation on a <see cref="IStateMachineConfiguration{TState,TTransition}"/>.
    /// </summary>
    /// <param name="stateMachineConfiguration">The state machine configuration.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <exception cref="InvalidOperationException">If there was a validation error.</exception>
    public static void Validate<TState, TTransition>(
        IStateMachineConfiguration<TState, TTransition> stateMachineConfiguration
    )
        where TState : notnull
        where TTransition : notnull
    {
        var context = new StateMachineValidationContext<TState, TTransition>();
        var states = stateMachineConfiguration.States;
        var transitions = stateMachineConfiguration.States.SelectMany(state => state.Transitions);

        // To accomodate transitions that are one-to-many, many-to-one, or many-to-many, add the states first and then
        // the transitions. This ensures all states are present when creating the transition models
        foreach (var state in states)
        {
            if (state is IValidatable<TState, TTransition> validatable)
            {
                validatable.AddToValidationContext(context);
            }
        }
        foreach (var transition in transitions)
        {
            if (transition is IValidatable<TState, TTransition> validatable)
            {
                validatable.AddToValidationContext(context);
            }
        }

        DuplicateStateValidator.Validate(context);
        TransitionFromValidator.Validate(context);
        TransitionToValidator.Validate(context);
        UnreachableTransitionValidator.Validate(context);

        if (context.ValidationErrors.Count > 0)
        {
            var message = new StringBuilder("Failed to create state machine due to validation errors:");
            foreach (var validationError in context.ValidationErrors)
            {
                message.AppendLine().Append("    ").Append(validationError);
            }

            throw new InvalidOperationException(message.ToString());
        }
    }
}
