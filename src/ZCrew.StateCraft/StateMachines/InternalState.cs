namespace ZCrew.StateCraft.StateMachines;

/// <summary>
///     The internal state of the state machine.
/// </summary>
internal enum InternalState
{
    /// <summary>
    ///     The state machine has not been activated.
    /// </summary>
    Inactive,

    /// <summary>
    ///     The state machine is active and ready for transitions.
    /// </summary>
    Active,

    /// <summary>
    ///     The state machine has completed activation and is ready to enter the next state.
    /// </summary>
    Idle,

    /// <summary>
    ///     The state machine is exiting a state.
    /// </summary>
    Exiting,

    /// <summary>
    ///     The state machine has exited the state.
    /// </summary>
    Exited,

    /// <summary>
    ///     The state machine is in the process of transitioning between states.
    /// </summary>
    Transitioning,

    /// <summary>
    ///     The state machine has transitioned: setting the next state, setting the next parameter, and has invoked
    ///     the state change handlers. The state machine is ready to enter the next state.
    /// </summary>
    Transitioned,

    /// <summary>
    ///     The state machine is entering a state.
    /// </summary>
    Entering,

    /// <summary>
    ///     The state machine has entered the state and the state action is ready to be ran.
    /// </summary>
    Entered,

    /// <summary>
    ///     The state machine failed to transition. It was rolled-back to it's previous state but did not re-enter
    ///     or restart the actions for that state.
    /// </summary>
    Recovery,
}
