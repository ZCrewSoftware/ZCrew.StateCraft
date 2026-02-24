using ZCrew.StateCraft.StateMachines;

namespace ZCrew.StateCraft.Extensions;

/// <summary>
///     Provides convenience properties for checking common <see cref="InternalState"/> patterns.
/// </summary>
internal static class InternalStateExtensions
{
    extension(InternalState state)
    {
        /// <summary>
        ///     Gets a value indicating whether the state machine can accept a new transition.
        /// </summary>
        public bool CanAcceptTransition =>
            state is InternalState.Active or InternalState.Recovery or InternalState.Entering;

        /// <summary>
        ///     Gets a value indicating whether the state machine is entering a state.
        /// </summary>
        public bool IsEntering => state is InternalState.Entering;

        /// <summary>
        ///     Gets a value indicating whether the state machine has been activated.
        /// </summary>
        public bool IsActivated => state is not InternalState.Inactive;

        /// <summary>
        ///     Gets a value indicating whether the state machine is inactive.
        /// </summary>
        public bool IsInactive => state is InternalState.Inactive;
    }
}
