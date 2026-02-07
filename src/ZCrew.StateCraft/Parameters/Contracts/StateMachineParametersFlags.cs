namespace ZCrew.StateCraft.Parameters.Contracts;

/// <summary>
/// Flags indicating which parameter slots have been set in a <see cref="StateMachineParameters"/>.
/// </summary>
[Flags]
internal enum StateMachineParametersFlags
{
    /// <summary>
    /// No parameters are set.
    /// </summary>
    None = 0,

    /// <summary>
    /// The previous parameter slot has been set.
    /// This flag is set by <see cref="IStateMachineParameters.BeginTransition"/>.
    /// </summary>
    PreviousParametersSet = 1 << 0,

    /// <summary>
    /// The current parameter slot has been set.
    /// This flag is set by <see cref="IStateMachineParameters.CommitTransition"/> or
    /// <see cref="IStateMachineParameters.RollbackTransition"/>.
    /// </summary>
    CurrentParametersSet = 1 << 1,

    /// <summary>
    /// The next parameter slot has been set.
    /// This flag is set by <see cref="IStateMachineParameters.SetNextParameter{T}"/>.
    /// </summary>
    NextParametersSet = 1 << 2,
}
