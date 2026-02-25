namespace OrderProcessor.Domain.Models;

public enum LineState
{
    /// <summary>
    ///     Entry state for lines. All lines stay incomplete until either <see cref="Suspending"/> or
    ///     <see cref="Completed"/>.
    /// </summary>
    Incomplete,

    /// <summary>
    ///     The line has been requested to suspend either by the user or by the order. The only valid transition is to
    ///     <see cref="Suspended"/>.
    /// </summary>
    Suspending,

    /// <summary>
    ///     The line has finished suspending. It can now be resumed and marked <see cref="Incomplete"/> or requested to
    ///     cancel and start <see cref="Canceling"/>.
    /// </summary>
    Suspended,

    /// <summary>
    ///     The line has been requested to cancel either by the user or by the order. The only valid transition is to
    ///     <see cref="Canceled"/>.
    /// </summary>
    Canceling,

    /// <summary>
    ///     Terminal state. The line was not <see cref="Completed"/> successfully.
    /// </summary>
    Canceled,

    /// <summary>
    ///     Terminal state. The line was <see cref="Completed"/> successfully.
    /// </summary>
    Completed,
}
