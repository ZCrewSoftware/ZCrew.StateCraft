namespace ZCrew.StateCraft.Validation;

internal static class DuplicateStateValidator
{
    /// <summary>
    ///     Validates that no two states share the same state value and type parameters.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    public static void Validate<TState, TTransition>(StateMachineValidationContext<TState, TTransition> context)
        where TState : notnull
        where TTransition : notnull
    {
        var seenStates = new HashSet<StateIdentity<TState>>();

        foreach (var state in context.Configuration.States)
        {
            var identity = new StateIdentity<TState> { State = state.State, TypeParameters = state.TypeParameters };

            if (!seenStates.Add(identity))
            {
                context.ValidationErrors.Add($"State: {state} is duplicated");
            }
        }
    }

    private readonly record struct StateIdentity<TState>
        where TState : notnull
    {
        public required TState State { get; init; }
        public required IReadOnlyList<Type> TypeParameters { get; init; }

        /// <inheritdoc />
        public bool Equals(StateIdentity<TState> other)
        {
            // There must be a matching state
            if (!EqualityComparer<TState>.Default.Equals(State, other.State))
            {
                return false;
            }

            // There must be a matching number of parameters
            if (TypeParameters.Count != other.TypeParameters.Count)
            {
                return false;
            }

            // Each parameter must match
            for (var i = 0; i < TypeParameters.Count; i++)
            {
                if (TypeParameters[i] != other.TypeParameters[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(State);
        }
    }
}
