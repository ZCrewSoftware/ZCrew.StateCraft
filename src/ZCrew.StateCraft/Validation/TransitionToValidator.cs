namespace ZCrew.StateCraft.Validation;

internal static class TransitionToValidator
{
    /// <summary>
    ///     Validates that each transition's target state exists with matching type parameters.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    public static void Validate<TState, TTransition>(StateMachineValidationContext<TState, TTransition> context)
        where TState : notnull
        where TTransition : notnull
    {
        var stateReferences = new List<StateReference<TState, TTransition>>();

        // Populate all the states first before trying to match transitions to states
        foreach (var state in context.Configuration.States)
        {
            stateReferences.Add(
                new StateReference<TState, TTransition> { State = state.State, TypeParameters = state.TypeParameters }
            );
        }

        // Match each transition to a state or add an error if there is no matching state
        foreach (var state in context.Configuration.States)
        {
            foreach (var transition in state.Transitions)
            {
                var isValid = stateReferences.Any(stateReference => stateReference.Matches(transition));
                if (isValid)
                {
                    continue;
                }

                // Check if there's a state with the right value but wrong parameter arity —
                // common when using the WithTransition(transition, state) shortcut which
                // always creates a parameterless transition regardless of target state arity.
                var matchByValueOnly = stateReferences
                    .Where(s => EqualityComparer<TState>.Default.Equals(s.State, transition.NextStateValue))
                    .ToList();

                if (
                    transition.NextStateTypeParameters.Count == 0
                    && matchByValueOnly.Any(s => s.TypeParameters.Count > 0)
                )
                {
                    var registered = matchByValueOnly
                        .Select(s => $"({string.Join(", ", s.TypeParameters.Select(t => t.Name))})")
                        .First();

                    context.ValidationErrors.Add(
                        $"Transition: {transition} targets state '{transition.NextStateValue}' as parameterless, "
                            + $"but it is registered with parameters {registered}. "
                            + "Use the explicit WithTransition(transition, t => t.WithParameter<T>().To(state)) form instead."
                    );
                }
                else
                {
                    context.ValidationErrors.Add($"Transition: {transition} has no matching next state");
                }
            }
        }
    }

    private readonly record struct StateReference<TState, TTransition>
        where TState : notnull
        where TTransition : notnull
    {
        public required TState State { get; init; }
        public required IReadOnlyList<Type> TypeParameters { get; init; }

        public bool Matches(ITransitionConfiguration<TState, TTransition> transition)
        {
            // There must be a matching state
            if (!EqualityComparer<TState>.Default.Equals(State, transition.NextStateValue))
            {
                return false;
            }

            // There must be a matching number of parameters
            if (TypeParameters.Count != transition.NextStateTypeParameters.Count)
            {
                return false;
            }

            // Each parameter must match
            for (var i = 0; i < TypeParameters.Count; i++)
            {
                if (!TypeParameters[i].IsAssignableTo(transition.NextStateTypeParameters[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
