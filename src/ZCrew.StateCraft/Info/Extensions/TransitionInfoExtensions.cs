namespace ZCrew.StateCraft.Info.Extensions;

/// <summary>
///     Extension methods over <see cref="ITransitionInfo{TState, TTransition}"/> for relating a transition to its
///     source and destination states across every transition variant (direct, mapped, and inverted).
/// </summary>
public static class TransitionInfoExtensions
{
    extension<TState, TTransition>(ITransitionInfo<TState, TTransition> transitionInfo)
        where TState : notnull
        where TTransition : notnull
    {
        /// <summary>
        ///     Determines whether this transition can be taken from <paramref name="stateInfo"/>.
        /// </summary>
        /// <param name="stateInfo">The candidate source state.</param>
        /// <returns>
        ///     <see langword="true"/> if <paramref name="stateInfo"/> is one of this transition's source states;
        ///     otherwise <see langword="false"/>.
        /// </returns>
        public bool IsTransitionFrom(IStateInfo<TState, TTransition> stateInfo)
        {
            return transitionInfo.GetPreviousStates().Contains(stateInfo);
        }

        /// <summary>
        ///     Determines whether this transition can be taken from the state with the given identity.
        /// </summary>
        /// <param name="stateValue">The candidate source state value.</param>
        /// <param name="stateParameterTypes">The candidate source state's parameter types, in declaration order.</param>
        /// <returns>
        ///     <see langword="true"/> if a state with the given value and parameter types is one of this transition's
        ///     source states; otherwise <see langword="false"/>.
        /// </returns>
        public bool IsTransitionFrom(TState stateValue, IReadOnlyList<Type> stateParameterTypes)
        {
            var previousStates = transitionInfo.GetPreviousStates();
            return previousStates.Any(previousState => previousState.Equals(stateValue, stateParameterTypes));
        }

        /// <summary>
        ///     Determines whether this transition leads to <paramref name="stateInfo"/>.
        /// </summary>
        /// <param name="stateInfo">The candidate destination state.</param>
        /// <returns>
        ///     <see langword="true"/> if <paramref name="stateInfo"/> is one of this transition's destination states;
        ///     otherwise <see langword="false"/>.
        /// </returns>
        public bool IsTransitionTo(IStateInfo<TState, TTransition> stateInfo)
        {
            return transitionInfo.GetNextStates().Contains(stateInfo);
        }

        /// <summary>
        ///     Determines whether this transition leads to the state with the given identity.
        /// </summary>
        /// <param name="stateValue">The candidate destination state value.</param>
        /// <param name="stateParameterTypes">
        ///     The candidate destination state's parameter types, in declaration order.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if a state with the given value and parameter types is one of this transition's
        ///     destination states; otherwise <see langword="false"/>.
        /// </returns>
        public bool IsTransitionTo(TState stateValue, IReadOnlyList<Type> stateParameterTypes)
        {
            var nextStates = transitionInfo.GetNextStates();
            return nextStates.Any(nextState => nextState.Equals(stateValue, stateParameterTypes));
        }

        /// <summary>
        ///     Gets the source states this transition can be taken from. Direct and mapped transitions have a single
        ///     source; an inverted (<c>From</c>) transition expands to every configured state except those it excludes.
        /// </summary>
        /// <returns>The states this transition can be taken from.</returns>
        /// <exception cref="ArgumentNullException">This transition is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The transition is of an unrecognized variant.</exception>
        public IEnumerable<IStateInfo<TState, TTransition>> GetPreviousStates()
        {
            ArgumentNullException.ThrowIfNull(transitionInfo);

            switch (transitionInfo)
            {
                case IDirectTransitionInfo<TState, TTransition> directTransition:
                    return [directTransition.PreviousState];

                case IMappedTransitionInfo<TState, TTransition> mappedTransition:
                    return [mappedTransition.PreviousState];

                case IFromTransitionInfo<TState, TTransition> fromTransition:
                    var allStates = transitionInfo.StateMachine.States;
                    return allStates.Where(state =>
                        !fromTransition.ExcludedStates.Any(excludedState => excludedState.Equals(state))
                    );

                default:
                    throw new ArgumentException(
                        $"Unexpected transition info: {transitionInfo.GetType()}",
                        nameof(transitionInfo)
                    );
            }
        }

        /// <summary>
        ///     Gets the destination states this transition leads to.
        /// </summary>
        /// <returns>The states this transition leads to.</returns>
        /// <exception cref="ArgumentNullException">This transition is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The transition is of an unrecognized variant.</exception>
        public IEnumerable<IStateInfo<TState, TTransition>> GetNextStates()
        {
            ArgumentNullException.ThrowIfNull(transitionInfo);

            switch (transitionInfo)
            {
                case IDirectTransitionInfo<TState, TTransition> directTransition:
                    return [directTransition.NextState];

                case IMappedTransitionInfo<TState, TTransition> mappedTransition:
                    return [mappedTransition.NextState];

                case IFromTransitionInfo<TState, TTransition> fromTransition:
                    return [fromTransition.NextState];

                default:
                    throw new ArgumentException(
                        $"Unexpected transition info: {transitionInfo.GetType()}",
                        nameof(transitionInfo)
                    );
            }
        }
    }
}
