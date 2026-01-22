using System.Threading.Channels;

namespace ZCrew.StateCraft;

public interface IScheduledTriggerConfiguration<TState, TTransition> : ITriggerConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures the trigger to await a synchronous signal before executing.
    /// </summary>
    /// <param name="signal">
    ///     The synchronous action to await. This is typically a blocking operation such as waiting on a
    ///     <see cref="TaskCompletionSource"/> or <see cref="Channel"/>.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IAwaitingTriggerConfiguration<TState, TTransition> Await(Action signal);

    /// <summary>
    ///     Configures the trigger to await an asynchronous signal before executing.
    /// </summary>
    /// <param name="signal">
    ///     The asynchronous function to await. This is typically waiting on a <see cref="TaskCompletionSource"/>
    ///     or <see cref="Channel"/>.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    /// <example>
    ///     With a <see cref="TaskCompletionSource"/> named <c>notificationReceived</c> a signal could be configured:
    ///     <code>
    ///     StateMachine.Configure&lt;string, string&gt;()
    ///         .WithTrigger(trigger =&gt;
    ///             trigger.Once()
    ///                 .Await(token =&gt; notificationReceived.Task.WaitAsync(token))
    ///                 .ThenInvoke((stateMachine, token) =&gt; stateMachine.Transition("Process", token)));
    ///     </code>
    ///     This would await the <c>notificationReceived</c> result and then transition the state machine with the
    ///     <c>"Process"</c> transition.
    /// </example>
    IAwaitingTriggerConfiguration<TState, TTransition> Await(Func<CancellationToken, Task> signal);

    /// <summary>
    ///     Configures the trigger to await an asynchronous signal before executing.
    /// </summary>
    /// <param name="signal">
    ///     The asynchronous function to await. This is typically waiting on a <see cref="TaskCompletionSource"/>
    ///     or <see cref="Channel"/>.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IAwaitingTriggerConfiguration<TState, TTransition> Await(Func<CancellationToken, ValueTask> signal);
}
