using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Options to enable certain features within a state machine.
/// </summary>
[Flags]
internal enum StateMachineOptions
{
    /// <summary>
    ///     Default option, which enables no additional features.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Run actions as an asynchronous task to allow the caller of
    ///     <see cref="IStateMachine{TState,TTransition}.Transition"/> (and other transition methods) to continue
    ///     without awaiting the completion of the action. Without this option the transition will await the completion
    ///     of the action, which may incur delays if the action is long-running.
    /// </summary>
    RunActionsAsynchronously = 1 << 0,
}
