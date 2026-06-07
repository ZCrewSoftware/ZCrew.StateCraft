using System.Text;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.Identities.Extensions;

/// <summary>
///     Display and matching helpers for <see cref="IStateIdentity{TState}"/>. The single home for rendering a state's
///     label so every layer (runtime, info, configuration) forwards here instead of duplicating the logic.
/// </summary>
internal static class StateIdentityExtensions
{
    /// <param name="state">The state identity to render.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    extension<TState>(IStateIdentity<TState> state)
        where TState : notnull
    {
        /// <summary>
        ///     Renders the state's label: just the value for a parameterless state (e.g. <c>A</c>), or the value
        ///     followed by its parameter types for a parameterized state (e.g. <c>A&lt;int, string&gt;</c>).
        /// </summary>
        /// <returns>A non-<see langword="null"/> display string for the state.</returns>
        public string ToDisplayString()
        {
            if (state.StateParameterTypes.Count == 0)
            {
                return $"{state.StateValue}";
            }

            var builder = new StringBuilder();
            builder.Append(state.StateValue).Append('<');

            for (var i = 0; i < state.StateParameterTypes.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(state.StateParameterTypes[i].FriendlyName);
            }

            return builder.Append('>').ToString();
        }

        /// <summary>
        ///     Determines whether two state identities share the same identity — the same state value and the same
        ///     parameter types in the same order.
        /// </summary>
        /// <param name="other">The second state identity.</param>
        /// <returns>
        ///     <see langword="true"/> if the states share the same identity; otherwise <see langword="false"/>.
        /// </returns>
        public bool Matches(IStateIdentity<TState> other)
        {
            return StateIdentityEqualityComparer<TState>.Instance.Equals(state, other);
        }

        /// <summary>
        ///     Determines whether this state has the given identity — the same state value and the same parameter
        ///     types in the same order.
        /// </summary>
        /// <param name="stateValue">The state value to compare against.</param>
        /// <param name="stateParameterTypes">The parameter types to compare against, in declaration order.</param>
        /// <returns>
        ///     <see langword="true"/> if this state's value and parameter types match; otherwise
        ///     <see langword="false"/>.
        /// </returns>
        public bool Matches(TState stateValue, IReadOnlyList<Type> stateParameterTypes)
        {
            return state.Matches(StateIdentity.For(stateValue, stateParameterTypes));
        }

        /// <summary>
        ///     Determines whether a value matching this state could be supplied where <paramref name="other"/> is
        ///     expected — the state values are equal and each of this state's parameter types is assignable from the
        ///     corresponding parameter type of <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The state whose parameter types are treated as the supplied arguments.</param>
        /// <returns>
        ///     <see langword="true"/> if the states are the same instance, or the values match and this state's
        ///     parameter types are assignable from <paramref name="other"/>'s; otherwise <see langword="false"/>
        ///     (including when <paramref name="other"/> is <see langword="null"/>).
        /// </returns>
        public bool IsAssignableFrom(IStateIdentity<TState>? other)
        {
            if (ReferenceEquals(state, other))
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            return EqualityComparer<TState>.Default.Equals(state.StateValue, other.StateValue)
                && state.StateParameterTypes.IsAssignableFrom(other.StateParameterTypes);
        }

        /// <summary>
        ///     Determines whether a value with the given identity could be supplied where this state is expected — the
        ///     state values are equal and each of this state's parameter types is assignable from the corresponding
        ///     supplied parameter type.
        /// </summary>
        /// <param name="stateValue">The state value to compare against.</param>
        /// <param name="stateParameterTypes">The supplied parameter types, in declaration order.</param>
        /// <returns>
        ///     <see langword="true"/> if the values match and this state's parameter types are assignable from
        ///     <paramref name="stateParameterTypes"/>; otherwise <see langword="false"/>.
        /// </returns>
        public bool IsAssignableFrom(TState stateValue, IReadOnlyList<Type> stateParameterTypes)
        {
            return state.IsAssignableFrom(StateIdentity.For(stateValue, stateParameterTypes));
        }
    }
}
