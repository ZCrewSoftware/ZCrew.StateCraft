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
        /// <remarks>
        ///     Uses an exhaustive switch expression so that adding a new <see cref="InternalState"/>
        ///     member produces a compiler warning (CS8524), forcing a deliberate decision.
        /// </remarks>
        public bool CanAcceptTransition => state switch
        {
            InternalState.Active => true,
            InternalState.Recovery => true,
            InternalState.Entering => true,
            InternalState.Inactive => false,
            InternalState.Idle => false,
            InternalState.Exiting => false,
            InternalState.Exited => false,
            InternalState.Transitioning => false,
            InternalState.Transitioned => false,
            InternalState.Entered => false,
            _ => false,
        };

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
