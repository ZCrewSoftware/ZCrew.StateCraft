namespace ZCrew.StateCraft.Validation;

internal static class UnreachableTransitionValidator
{
    /// <summary>
    ///     Validates that each transition from a state is reachable. A transition is not reachable if there was a
    ///     previous non-conditional transition with the same transition value and with the same parameter type or a
    ///     base parameter type.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    public static void Validate<TState, TTransition>(StateMachineValidationContext<TState, TTransition> context)
        where TState : notnull
        where TTransition : notnull
    {
        foreach (var state in context.Configuration.States)
        {
            var nonConditionalTransitions = new List<Transition<TState, TTransition>>();

            foreach (var transition in state.Transitions)
            {
                var currentTransition = new Transition<TState, TTransition>
                {
                    TransitionValue = transition.TransitionValue,
                    TypeParameters = transition.TransitionTypeParameters,
                };

                if (nonConditionalTransitions.Any(prev => prev.Shadows(currentTransition)))
                {
                    context.ValidationErrors.Add(
                        $"{transition} is unreachable because it is shadowed by a previous transition"
                    );
                    continue;
                }

                if (!transition.IsConditional)
                {
                    nonConditionalTransitions.Add(currentTransition);
                }
            }
        }
    }

    private readonly record struct Transition<TState, TTransition>
        where TState : notnull
        where TTransition : notnull
    {
        public required TTransition TransitionValue { get; init; }
        public required IReadOnlyList<Type> TypeParameters { get; init; }

        public bool Shadows(Transition<TState, TTransition> other)
        {
            // Check if the transitions have the same transition value
            if (!EqualityComparer<TTransition>.Default.Equals(TransitionValue, other.TransitionValue))
            {
                return false;
            }

            // There must be a matching number of parameters
            if (TypeParameters.Count != other.TypeParameters.Count)
            {
                return false;
            }

            // This transition shadows the other transition if every parameter is assignable from (is a base class of)
            // the corresponding parameter from the other transition
            for (var i = 0; i < TypeParameters.Count; i++)
            {
                if (!TypeParameters[i].IsAssignableFrom(other.TypeParameters[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
