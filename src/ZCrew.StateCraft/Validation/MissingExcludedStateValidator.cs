using ZCrew.StateCraft.Info.Extensions;

namespace ZCrew.StateCraft.Validation;

internal static class MissingExcludedStateValidator
{
    /// <summary>
    ///     Validates that every excluded state on every <see cref="IFromTransitionConfiguration{TState,TTransition}"/>
    ///     actually exists.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    public static void Validate<TState, TTransition>(StateMachineValidationContext<TState, TTransition> context)
        where TState : notnull
        where TTransition : notnull
    {
        var fromTransitions = context.Info.Transitions.OfType<IFromTransitionInfo<TState, TTransition>>();
        foreach (var transition in fromTransitions)
        {
            foreach (var excludedState in transition.ExcludedStates)
            {
                if (context.Info.GetStateOrDefault(excludedState) == null)
                {
                    // Check if there's a state with the right value but wrong parameter arity
                    var matchByValueOnly = context
                        .Info.States.Where(s =>
                            EqualityComparer<TState>.Default.Equals(s.StateValue, excludedState.StateValue)
                        )
                        .ToList();

                    if (matchByValueOnly.Count == 1)
                    {
                        var correctedExceptCall = GetCorrectedExceptCall(matchByValueOnly[0]);
                        context.ValidationErrors.Add(
                            $"Transition: {transition} excluded state '{excludedState.StateValue}' was not found, "
                                + $"but a state was registered as {matchByValueOnly[0]}. "
                                + $"Specify the correct parameters like: {correctedExceptCall}"
                        );
                    }
                    else if (matchByValueOnly.Count > 1)
                    {
                        var stateList = string.Join(", ", matchByValueOnly);
                        context.ValidationErrors.Add(
                            $"Transition: {transition} excluded state '{excludedState.StateValue}' was not found, "
                                + $"but states with the same value were registered: {stateList}. "
                                + "Specify the correct parameters for the state you'd like to exclude"
                        );
                    }
                    else
                    {
                        context.ValidationErrors.Add($"Excluded state: {excludedState} was not found");
                    }
                }
            }
        }
    }

    private static string GetCorrectedExceptCall<TState>(IStateIdentity<TState> registeredState)
        where TState : notnull
    {
        if (registeredState.StateParameterTypes.Count == 0)
        {
            return $"Except({typeof(TState).Name}.{registeredState.StateValue})";
        }

        var typeParameters = string.Join(", ", registeredState.StateParameterTypes.Select(type => type.FriendlyName));
        return $"Except<{typeParameters}>({typeof(TState).Name}.{registeredState.StateValue})";
    }
}
