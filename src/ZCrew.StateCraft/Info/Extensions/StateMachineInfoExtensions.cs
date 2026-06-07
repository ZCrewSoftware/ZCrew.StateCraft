using ZCrew.StateCraft.Identities.Extensions;

namespace ZCrew.StateCraft.Info.Extensions;

/// <summary>
///     Extension methods over <see cref="IStateMachineInfo{TState, TTransition}"/> for looking up the states and
///     transitions configured on a machine by their value and parameter types.
/// </summary>
public static class StateMachineInfoExtensions
{
    extension<TState, TTransition>(IStateMachineInfo<TState, TTransition> stateMachineInfo)
        where TState : notnull
        where TTransition : notnull
    {
        /// <summary>
        ///     Gets the configured state matching <paramref name="stateValue"/> and the given parameter types,
        ///     throwing if no such state exists.
        /// </summary>
        /// <param name="stateValue">The state value to look up.</param>
        /// <param name="stateParameterTypes">
        ///     The parameter types of the state to look up, in declaration order. Omit for a parameterless state.
        /// </param>
        /// <returns>The matching state.</returns>
        /// <exception cref="InvalidOperationException">No configured state matches the given value and parameters.</exception>
        public IStateInfo<TState, TTransition> GetState(TState stateValue, params Type[] stateParameterTypes)
        {
            return stateMachineInfo.GetState(StateIdentity.For(stateValue, stateParameterTypes));
        }

        /// <summary>
        ///     Gets the configured state matching <paramref name="stateValue"/> and the given parameter types, or
        ///     <see langword="null"/> if no such state exists.
        /// </summary>
        /// <param name="stateValue">The state value to look up.</param>
        /// <param name="stateParameterTypes">
        ///     The parameter types of the state to look up, in declaration order. Omit for a parameterless state.
        /// </param>
        /// <returns>The matching state, or <see langword="null"/> if none matches.</returns>
        public IStateInfo<TState, TTransition>? GetStateOrDefault(TState stateValue, params Type[] stateParameterTypes)
        {
            return stateMachineInfo.GetStateOrDefault(StateIdentity.For(stateValue, stateParameterTypes));
        }

        /// <summary>
        ///     Gets the configured state matching <paramref name="state"/>, or <see langword="null"/> if no such state
        ///     exists.
        /// </summary>
        /// <param name="state">The state identity to look up.</param>
        /// <returns>The matching state, or <see langword="null"/> if none matches.</returns>
        public IStateInfo<TState, TTransition> GetState(IStateIdentity<TState> state)
        {
            return stateMachineInfo.GetStateOrDefault(state)
                ?? throw new InvalidOperationException(
                    $"The state machine does not contain any state matching: " + $"{state.ToDisplayString()}"
                );
        }

        /// <summary>
        ///     Gets the configured state matching <paramref name="state"/>, or <see langword="null"/> if no such state
        ///     exists.
        /// </summary>
        /// <param name="state">The state identity to look up.</param>
        /// <returns>The matching state, or <see langword="null"/> if none matches.</returns>
        public IStateInfo<TState, TTransition>? GetStateOrDefault(IStateIdentity<TState> state)
        {
            return stateMachineInfo.States.FirstOrDefault(stateInfo => stateInfo.Matches(state));
        }
    }
}
