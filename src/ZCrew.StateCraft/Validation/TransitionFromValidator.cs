namespace ZCrew.StateCraft.Validation;

internal static class TransitionFromValidator
{
    /// <summary>
    ///     Validates that each transition's previous state exists with matching type parameters.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    public static void Validate<TState, TTransition>(StateMachineValidationContext<TState, TTransition> context)
        where TState : notnull
        where TTransition : notnull
    {
        // Match each transition to a state or add an error if there is no matching state
        foreach (var transition in context.Transitions)
        {
            var isValid = context.States.Any(state => state.IsAssignableFromPreviousState(transition));
            if (isValid)
            {
                continue;
            }

            // Check if there's a state with the right value but wrong parameter arity —
            // common when using the WithTransition(transition, state) shortcut which
            // always creates a parameterless transition regardless of target state arity.
            var matchByValueOnly = context
                .States.Where(s => EqualityComparer<TState>.Default.Equals(s.State, transition.PreviousStateValue))
                .ToList();

            if (
                transition.PreviousStateTypeParameters.Count == 0
                && matchByValueOnly.Any(s => s.TypeParameters.Count > 0)
            )
            {
                var alternatives = matchByValueOnly.Select(s => s.ToString()).ToList();

                if (alternatives.Count == 1)
                {
                    context.ValidationErrors.Add(
                        $"Transition: {transition} targets state '{transition.PreviousStateValue}' as parameterless, "
                            + $"but it is registered as {alternatives[0]}. "
                            + "Use the explicit WithTransition(transition, t => t.WithParameter<T>().To(state)) form instead."
                    );
                }
                else
                {
                    var stateList = string.Join(", ", alternatives);
                    context.ValidationErrors.Add(
                        $"Transition: {transition} targets state '{transition.PreviousStateValue}' as parameterless, "
                            + $"but it is registered with parameters: {stateList}. "
                            + "Use the explicit WithTransition(transition, t => t.WithParameter<T>().To(state)) form instead."
                    );
                }
            }
            else
            {
                context.ValidationErrors.Add($"Transition: {transition} has no matching previous state");
            }
        }
    }
}
