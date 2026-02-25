namespace OrderProcessor.Domain.Models;

public enum OrderState
{
    /// <summary>
    ///     The initial state for orders. Once activated the order will become <see cref="Processing"/>. Alternatively,
    ///     the order may be canceled by transitioning to <see cref="Canceling"/>.
    /// </summary>
    Queued,

    /// <summary>
    ///     The order is processing. The next possible states are:
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="Completed"/> because every line is <see cref="LineState.Completed"/>
    ///         </item>
    ///         <item>
    ///             <see cref="Suspending"/> because the user has requested the order to start suspending
    ///         </item>
    ///         <item>
    ///             <see cref="Suspended"/> because the user has requested the remaining lines that are not
    ///             <see cref="LineState.Completed"/> or <see cref="LineState.Canceled"/> to be suspended individually
    ///         </item>
    ///     </list>
    /// </summary>
    Processing,

    /// <summary>
    ///     The order has been requested to pause. The order will now start suspending each line. Once each line is
    ///     <see cref="LineState.Suspended"/> then the order will become <see cref="Suspended"/>.
    /// </summary>
    Suspending,

    /// <summary>
    ///     All lines are <see cref="LineState.Suspended"/>, <see cref="LineState.Completed"/>, or
    ///     <see cref="LineState.Canceled"/>, and at least one line is <see cref="LineState.Suspended"/>. If the order
    ///     is resumed then this can transition to <see cref="Queued"/>. If the order is canceled then this can
    ///     transition to <see cref="Canceling"/>. If the remaining suspended lines are canceled individually then the
    ///     order can transition to <see cref="Canceled"/>.
    /// </summary>
    Suspended,

    /// <summary>
    ///     The order has been requested to cancel. The order will now start canceling each line. Once each line is
    ///     <see cref="LineState.Canceled"/> then the order will become <see cref="Canceled"/>.
    /// </summary>
    Canceling,

    /// <summary>
    ///     All lines are <see cref="LineState.Completed"/> or <see cref="LineState.Canceled"/>, and at least one line
    ///     is <see cref="LineState.Canceled"/>. This is a terminal state.
    /// </summary>
    Canceled,

    /// <summary>
    ///     All lines are <see cref="LineState.Completed"/>. This is a terminal state.
    /// </summary>
    Completed,
}
