namespace ZCrew.StateCraft.Triggers.Contracts;

/// <summary>
///     Represents a trigger that can be activated and deactivated alongside a state machine.
///     Triggers wait for a signal and then execute functionality when the signal is received.
/// </summary>
internal interface ITrigger
{
    /// <summary>
    ///     Gets the number of times this trigger has been executed since the last activation.
    ///     This count is reset when the trigger is deactivated.
    /// </summary>
    int TriggeredCount { get; }

    /// <summary>
    ///     Activates the trigger, starting its background execution loop.
    ///     If the trigger is already activated, this method returns immediately.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the trigger has been activated.</returns>
    Task Activate(CancellationToken token);

    /// <summary>
    ///     Deactivates the trigger, cancelling its background execution and awaiting completion.
    ///     If the trigger is not activated, this method returns immediately.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the trigger has been fully deactivated.</returns>
    Task Deactivate(CancellationToken token);
}
