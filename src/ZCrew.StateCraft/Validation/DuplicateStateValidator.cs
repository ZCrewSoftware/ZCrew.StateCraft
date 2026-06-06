using ZCrew.StateCraft.Identities;

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
        var seenStates = new HashSet<IStateInfo<TState, TTransition>>(StateIdentityEqualityComparer<TState>.Instance);

        foreach (var state in context.Info.States)
        {
            if (!seenStates.Add(state))
            {
                context.ValidationErrors.Add($"State: {state} is duplicated");
            }
        }
    }
}
