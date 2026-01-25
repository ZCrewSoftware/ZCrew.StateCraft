using System.Text;

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
        var context = new StateMachineValidationContext<TState, TTransition>
        {
            Configuration = stateMachineConfiguration,
        };

        DuplicateStateValidator.Validate(context);
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
