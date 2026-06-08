using ZCrew.StateCraft.Identities;

namespace ZCrew.StateCraft.Validation;

internal static class StaticInitialStateValidator
{
    /// <summary>
    ///     Validates that the initial state, when provided statically from
    ///     <see cref="IInitialStateMachineConfiguration{TState,TTransition}.WithInitialState(TState)"/>
    ///     (and overloads), exists.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    public static void Validate<TState, TTransition>(StateMachineValidationContext<TState, TTransition> context)
        where TState : notnull
        where TTransition : notnull
    {
        if (context.Info.InitialState is IStaticInitialStateInfo<TState, TTransition> staticInitialState)
        {
            var initialState = StateIdentity.For(
                staticInitialState.InitialStateValue,
                staticInitialState.InitialParameterTypes
            );
            if (!context.Info.States.Contains(initialState, StateIdentityEqualityComparer<TState>.Instance))
            {
                // Check if there's a state with the right value but wrong parameter arity
                var matchByValueOnly = context
                    .Info.States.Where(s =>
                        EqualityComparer<TState>.Default.Equals(s.StateValue, initialState.StateValue)
                    )
                    .ToList();

                if (matchByValueOnly.Count == 1)
                {
                    var correctedInitialStateCall = GetCorrectedInitialStateCall(matchByValueOnly[0]);
                    context.ValidationErrors.Add(
                        $"Initial state: {initialState} was not found, "
                            + $"but a state was registered as {matchByValueOnly[0]}. "
                            + $"Specify the correct parameters like: {correctedInitialStateCall}"
                    );
                }
                else if (matchByValueOnly.Count > 1)
                {
                    var stateList = string.Join(", ", matchByValueOnly);
                    context.ValidationErrors.Add(
                        $"Initial state: {initialState} was not found, "
                            + $"but states with the same value were registered: {stateList}. "
                            + "Specify the correct parameters for the state you'd like to use as the initial state"
                    );
                }
                else
                {
                    context.ValidationErrors.Add($"Initial state: {initialState} was not found");
                }
            }
        }
    }

    private static string GetCorrectedInitialStateCall<TState>(IStateIdentity<TState> registeredState)
        where TState : notnull
    {
        if (registeredState.StateParameterTypes.Count == 0)
        {
            return $"WithInitialState({typeof(TState).Name}.{registeredState.StateValue})";
        }

        var typeParameters = string.Join(", ", registeredState.StateParameterTypes.Select(type => type.FriendlyName));
        return $"WithInitialState<{typeParameters}>({typeof(TState).Name}.{registeredState.StateValue}, ...)";
    }
}
