using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Extension methods for <see cref="IStateMachine{TState, TTransition}"/>.
/// </summary>
internal static class StateMachineExtensions
{
    extension<TState, TTransition>(IStateMachine<TState, TTransition> stateMachine)
        where TState : notnull
        where TTransition : notnull
    {
        /// <summary>
        ///     Gets the previous parameter from the state machine cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the parameter to.</typeparam>
        /// <returns>The previous parameter cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when <see cref="IStateMachine{TState, TTransition}.CurrentParameter"/> is <see langword="null"/>.
        /// </exception>
        [Obsolete($"Use {nameof(Parameters)} instead")]
        internal T GetPreviousParameter<T>()
        {
            if (stateMachine.PreviousParameter == null)
            {
                throw new InvalidOperationException($"Expected {nameof(stateMachine.PreviousParameter)} to be set");
            }
            return (T)stateMachine.PreviousParameter;
        }

        /// <summary>
        ///     Gets the current parameter from the state machine cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the parameter to.</typeparam>
        /// <returns>The current parameter cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when <see cref="IStateMachine{TState, TTransition}.CurrentParameter"/> is <see langword="null"/>.
        /// </exception>
        [Obsolete($"Use {nameof(Parameters)} instead")]
        internal T GetCurrentParameter<T>()
        {
            if (stateMachine.CurrentParameter == null)
            {
                throw new InvalidOperationException($"Expected {nameof(stateMachine.CurrentParameter)} to be set");
            }
            return (T)stateMachine.CurrentParameter;
        }

        /// <summary>
        ///     Gets the next parameter from the state machine cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the parameter to.</typeparam>
        /// <returns>The next parameter cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when <see cref="IStateMachine{TState, TTransition}.NextParameter"/> is <see langword="null"/>.
        /// </exception>
        [Obsolete($"Use {nameof(Parameters)} instead")]
        internal T GetNextParameter<T>()
        {
            if (stateMachine.NextParameter == null)
            {
                throw new InvalidOperationException($"Expected {nameof(stateMachine.NextParameter)} to be set");
            }
            return (T)stateMachine.NextParameter;
        }
    }
}
