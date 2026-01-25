using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Represents the stage of trigger configuration where the trigger action and execution mode are specified.
///     This stage is reached after configuring the signal to await.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IAwaitingTriggerConfiguration<TState, TTransition> : ITriggerConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures the delegate that is called when the trigger is signaled.
    /// </summary>
    /// <param name="trigger">The synchronous action to execute when triggered.</param>
    /// <returns>The final stage of trigger configuration.</returns>
    IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(Action trigger);

    /// <summary>
    ///     Configures the delegate that is called when the trigger is signaled.
    /// </summary>
    /// <param name="trigger">The asynchronous function to execute when triggered.</param>
    /// <returns>The final stage of trigger configuration.</returns>
    IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(Func<CancellationToken, Task> trigger);

    /// <summary>
    ///     Configures the delegate that is called when the trigger is signaled.
    /// </summary>
    /// <param name="trigger">The asynchronous function to execute when triggered.</param>
    /// <returns>The final stage of trigger configuration.</returns>
    IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(Func<CancellationToken, ValueTask> trigger);

    /// <summary>
    ///     Configures the delegate that is called when the trigger is signaled, with access to the state machine.
    /// </summary>
    /// <param name="trigger">
    ///     The synchronous action to execute when triggered, receiving the state machine as a parameter.
    /// </param>
    /// <returns>The final stage of trigger configuration.</returns>
    IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(Action<IStateMachine<TState, TTransition>> trigger);

    /// <summary>
    ///     Configures the delegate that is called when the trigger is signaled, with access to the state machine.
    /// </summary>
    /// <param name="trigger">
    ///     The asynchronous function to execute when triggered, receiving the state machine as a parameter.
    /// </param>
    /// <returns>The final stage of trigger configuration.</returns>
    IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(
        Func<IStateMachine<TState, TTransition>, CancellationToken, Task> trigger
    );

    /// <summary>
    ///     Configures the delegate that is called when the trigger is signaled, with access to the state machine.
    /// </summary>
    /// <param name="trigger">
    ///     The asynchronous function to execute when triggered, receiving the state machine as a parameter.
    /// </param>
    /// <returns>The final stage of trigger configuration.</returns>
    IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(
        Func<IStateMachine<TState, TTransition>, CancellationToken, ValueTask> trigger
    );
}
