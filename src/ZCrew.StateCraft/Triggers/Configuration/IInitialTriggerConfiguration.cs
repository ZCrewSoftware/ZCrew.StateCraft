using System.Threading.Channels;

namespace ZCrew.StateCraft;

/// <summary>
///     Represents the initial stage of trigger configuration where a signal to await is specified.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IInitialTriggerConfiguration<TState, TTransition> : ITriggerConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures the trigger to run once. Once the trigger has been fired it will not act again until the state
    ///     machine is deactivated and then reactivated.
    /// </summary>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    /// <remarks>
    ///     It is recommended to await a <see cref="TaskCompletionSource"/> with this, as the
    ///     <see cref="TaskCompletionSource"/> can only be set once.
    /// </remarks>
    IScheduledTriggerConfiguration<TState, TTransition> Once();

    /// <summary>
    ///     Configures the trigger to run multiple times. Once the trigger has been fired and the trigger effect has
    ///     finished, then the trigger will await the signal again. Once the signal has been set by an event then
    ///     subsequent events will not signal the trigger until the trigger has completed.
    /// </summary>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    /// <remarks>
    ///     It is recommended to await a <see cref="Channel{T}"/> or <see cref="AutoResetEvent"/> with this, as those
    ///     primitives can be set multiple times. The <see cref="Channel{T}"/> allows queuing events so the trigger will
    ///     fire for every event. The <see cref="AutoResetEvent"/> will ignore events after the first event until the
    ///     trigger has completed.
    /// </remarks>
    IScheduledTriggerConfiguration<TState, TTransition> Repeat();
}
