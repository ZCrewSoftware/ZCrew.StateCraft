using ZCrew.StateCraft.Identities;

namespace ZCrew.StateCraft.Validation;

internal static class DuplicateExcludedStateValidator
{
    /// <summary>
    ///     Validates that no state was excluded multiple times in a single
    ///     <see cref="IFromTransitionConfiguration{TState,TTransition}"/>.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    public static void Validate<TState, TTransition>(StateMachineValidationContext<TState, TTransition> context)
        where TState : notnull
        where TTransition : notnull
    {
        var seenStates = new HashSet<IStateInfo<TState, TTransition>>(StateIdentityEqualityComparer<TState>.Instance);
        var fromTransitions = context.Info.Transitions.OfType<IFromTransitionInfo<TState, TTransition>>();
        foreach (var transition in fromTransitions)
        {
            // Re-use same hashset just to avoid allocations
            seenStates.Clear();

            foreach (var excludedState in transition.ExcludedStates)
            {
                if (!seenStates.Add(excludedState))
                {
                    context.ValidationErrors.Add(
                        $"Excluded state: {excludedState} has already been excluded for transition: {transition}"
                    );
                }
            }
        }
    }
}
