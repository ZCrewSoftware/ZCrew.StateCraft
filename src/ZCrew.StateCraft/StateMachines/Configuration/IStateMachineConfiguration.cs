using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     <para>
///     Defines the available states and any optional behaviors to construct a state machine.
///     </para>
///     <para>
///     Once configuration is complete, call <see cref="Build()"/> to produce a new state machine. This configuration is
///     reusable and may be used to create independent state machine instances that all share the same configuration.
///     </para>
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IStateMachineConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures the state machine to run actions as an asynchronous task to allow the caller of
    ///     <see cref="IStateMachine{TState,TTransition}.Transition"/> (and other transition methods) to continue
    ///     without awaiting the completion of the action. Without this option the transition will await the completion
    ///     of the action, which may incur delays if the action is long-running.
    /// </summary>
    /// <remarks>
    ///     Asynchronous actions receive a <see cref="CancellationToken"/> that is canceled when the state
    ///     machine transitions to a different state or is deactivated. Actions <b>must</b> observe this
    ///     token — once canceled, the action's work is no longer relevant. Failing to observe it can lead
    ///     to memory leaks, deadlocks, or invalid operations where a stale action continues to interact
    ///     with the state machine after the state has already changed.
    /// </remarks>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    /// <example>
    /// <code>
    ///     var stateMachine = StateMachine.Configure&lt;string, string&gt;()
    ///         .WithAsynchronousActions()
    ///         .WithInitialState("A")
    ///         .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
    ///         .WithState("B", state => state.WithAction(a =>
    ///             a.Invoke(() =>
    ///             {
    ///                 Thread.Sleep(5000);
    ///                 Console.WriteLine("Action done");
    ///             })))
    ///         .Build();
    ///     await s.Activate();
    ///     await s.Transition("To B");
    ///     <br/>
    ///     // Since the action is long running (delayed 5 seconds) this log message will appear first:
    ///     Console.WriteLine("Transition done");
    /// </code>
    /// </example>
    IStateMachineConfiguration<TState, TTransition> WithAsynchronousActions();

    /// <summary>
    ///     Configures a custom <see cref="IExceptionBehavior"/> provider. This will be called each time
    ///     <see cref="Build()"/> is called.
    /// </summary>
    /// <param name="exceptionBehaviorProvider">
    ///     The provider that creates an implementation of the <see cref="IExceptionBehavior"/>.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    /// <remarks>
    ///     A default exception behavior, <see cref="RethrowExceptionBehavior"/>, has been provided with
    ///     <see langword="virtual"/> methods which can be overriden as necessary.
    /// </remarks>
    IStateMachineConfiguration<TState, TTransition> WithExceptionBehavior(
        Func<IEnumerable<IAsyncAction<ExceptionContext>>, IExceptionBehavior> exceptionBehaviorProvider
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when the state changes. The parameters
    ///     to the <paramref name="handler"/> are: the previous state, the transition, and the next state.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is changed.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    /// <example>
    ///     <code>
    ///     StateMachine
    ///         .Configure&lt;State, Transition&gt;()
    ///         .OnStateChange((from, transition, to)
    ///             => Console.WriteLine($"Changing to {to} from {from} with {transition}"));
    ///     </code>
    /// </example>
    IStateMachineConfiguration<TState, TTransition> OnStateChange(Action<TState, TTransition, TState> handler);

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when the state changes. The parameters
    ///     to the <paramref name="handler"/> are: the previous state, the transition, the next state, and a token to
    ///     monitor for cancellation.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is changed.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> OnStateChange(
        Func<TState, TTransition, TState, CancellationToken, Task> handler
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when the state changes. The parameters
    ///     to the <paramref name="handler"/> are: the previous state, the transition, the next state, and a token to
    ///     monitor for cancellation.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is changed.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> OnStateChange(
        Func<TState, TTransition, TState, CancellationToken, ValueTask> handler
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when an exception is thrown during
    ///     state machine operations (lifecycle, conditions, mapping, actions, and triggers).
    /// </summary>
    /// <param name="handler">The delegate to call when an exception occurs.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> OnException(Action<ExceptionContext> handler);

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when an exception is thrown during
    ///     state machine operations (lifecycle, conditions, mapping, actions, and triggers).
    /// </summary>
    /// <param name="handler">The delegate to call when an exception occurs.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> OnException(
        Func<ExceptionContext, CancellationToken, Task> handler
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when an exception is thrown during
    ///     state machine operations (lifecycle, conditions, mapping, actions, and triggers).
    /// </summary>
    /// <param name="handler">The delegate to call when an exception occurs.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> OnException(
        Func<ExceptionContext, CancellationToken, ValueTask> handler
    );

    /// <summary>
    ///     Configures a new state for the state machine.
    /// </summary>
    /// <param name="state">The state to configure.</param>
    /// <param name="configureState">The configuration setup.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithState(
        TState state,
        Func<IInitialStateConfiguration<TState, TTransition>, IStateConfiguration<TState, TTransition>> configureState
    );

    /// <summary>
    ///     Configures a trigger for this state machine. Triggers are activated when the state machine is activated
    ///     and deactivated when the state machine is deactivated. They wait for a signal and then execute functionality.
    /// </summary>
    /// <param name="configureTrigger">The configuration setup.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    /// <remarks>
    ///     Triggers are mostly used when a component of a system needs to transition the state machine; but, it is not
    ///     possible or desirable to directly call upon the state machine. For example, a service may internally use a
    ///     state machine and configure a trigger to call a specific 'Cancel' transition when a cancellation token is
    ///     canceled. In this way, the caller of the service can cancel the token to transition the state machine into a
    ///     'Canceled' state.
    /// </remarks>
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
    IStateMachineConfiguration<TState, TTransition> WithTrigger(
        Func<
            IInitialTriggerConfiguration<TState, TTransition>,
            ITriggerConfiguration<TState, TTransition>
        > configureTrigger
    );

    /// <summary>
    ///     Builds a new <see cref="IStateMachine{TState,TTransition}"/> based on the current configuration. This can be
    ///     called multiple times to produce independent state machines instances as necessary.
    /// </summary>
    /// <returns>
    ///     A new instance of a <see cref="IStateMachine{TState,TTransition}"/> based on the current configuration.
    /// </returns>
    IStateMachine<TState, TTransition> Build();

    /// <summary>
    ///     Builds a new <see cref="IStateMachine{TState,TTransition}"/> based on the current configuration. This can be
    ///     called multiple times to produce independent state machines instances as necessary. The
    ///     <paramref name="options"/> can enable features during building such as validation.
    /// </summary>
    /// <returns>
    ///     A new instance of a <see cref="IStateMachine{TState,TTransition}"/> based on the current configuration.
    /// </returns>
    IStateMachine<TState, TTransition> Build(StateMachineBuildOptions options);

    /// <summary>
    ///     Query information about this state machine, states, and transitions.
    /// </summary>
    /// <returns>Information about this configuration.</returns>
    /// <remarks>
    ///     Changes to this configuration will not update the information returned here previously.
    ///     If you make changes to this configuration then requery the information.
    /// </remarks>
    IStateMachineInfo<TState, TTransition> GetInfo();

    /// <summary>
    ///     The states in this configuration.
    /// </summary>
    internal IEnumerable<IStateConfiguration<TState, TTransition>> States { get; }
}
