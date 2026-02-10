using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     <para>
///     Defines the initial state, available states, and any optional behaviors to construct a state machine.
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
    ///     Configures the initial state of the machine to <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState(TState state);

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState(Func<TState> stateProvider);

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState(
        Func<CancellationToken, Task<TState>> stateProvider
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState(
        Func<CancellationToken, ValueTask<TState>> stateProvider
    );

    /// <summary>
    ///     Configures the initial state of the machine to <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter">The parameter for the initial state.</param>
    /// <typeparam name="T">The type of the initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(TState state, T parameter);

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameter.</param>
    /// <typeparam name="T">The type of the initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(Func<(TState, T)> stateProvider);

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state.</param>
    /// <typeparam name="T">The type of the initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(
        Func<CancellationToken, Task<(TState, T)>> stateProvider
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state as
    ///     the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state.</param>
    /// <typeparam name="T">The type of the initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(
        Func<CancellationToken, ValueTask<(TState, T)>> stateProvider
    );

    /// <summary>
    ///     Configures the initial state of the machine to <paramref name="state"/> with two parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first parameter for the initial state.</param>
    /// <param name="parameter2">The second parameter for the initial state.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        TState state,
        T1 parameter1,
        T2 parameter2
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and two parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(Func<(TState, T1, T2)> stateProvider);

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and two parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<CancellationToken, Task<(TState, T1, T2)>> stateProvider
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and two parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<CancellationToken, ValueTask<(TState, T1, T2)>> stateProvider
    );

    /// <summary>
    ///     Configures the initial state of the machine to <paramref name="state"/> with three parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first parameter for the initial state.</param>
    /// <param name="parameter2">The second parameter for the initial state.</param>
    /// <param name="parameter3">The third parameter for the initial state.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        TState state,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and three parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<(TState, T1, T2, T3)> stateProvider
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and three parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<CancellationToken, Task<(TState, T1, T2, T3)>> stateProvider
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and three parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<CancellationToken, ValueTask<(TState, T1, T2, T3)>> stateProvider
    );

    /// <summary>
    ///     Configures the initial state of the machine to <paramref name="state"/> with four parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first parameter for the initial state.</param>
    /// <param name="parameter2">The second parameter for the initial state.</param>
    /// <param name="parameter3">The third parameter for the initial state.</param>
    /// <param name="parameter4">The fourth parameter for the initial state.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        TState state,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and four parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<(TState, T1, T2, T3, T4)> stateProvider
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and four parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<CancellationToken, Task<(TState, T1, T2, T3, T4)>> stateProvider
    );

    /// <summary>
    ///     Configures the <paramref name="stateProvider"/> delegate which will be called to fetch the initial state
    ///     and four parameters as the state machine is initialized to.
    /// </summary>
    /// <param name="stateProvider">The delegate to fetch the initial state and parameters.</param>
    /// <typeparam name="T1">The type of the first initial state parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial state parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial state parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth initial state parameter.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<CancellationToken, ValueTask<(TState, T1, T2, T3, T4)>> stateProvider
    );

    /// <summary>
    ///     Configures a <see cref="handler"/> delegate which will be called when the state changes. The parameters to
    ///     the <paramref name="handler"/> are: the previous state, the transition, and the next state.
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
    ///     Configures a <see cref="handler"/> delegate which will be called when the state changes. The parameters to
    ///     the <paramref name="handler"/> are: the previous state, the transition, the next state, and a token to
    ///     monitor for cancellation.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is changed.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> OnStateChange(
        Func<TState, TTransition, TState, CancellationToken, Task> handler
    );

    /// <summary>
    ///     Configures a <see cref="handler"/> delegate which will be called when the state changes. The parameters to
    ///     the <paramref name="handler"/> are: the previous state, the transition, the next state, and a token to
    ///     monitor for cancellation.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is changed.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> OnStateChange(
        Func<TState, TTransition, TState, CancellationToken, ValueTask> handler
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when an exception is thrown during
    ///     state lifecycle operations (OnActivate, OnEntry, OnExit, OnDeactivate, OnStateChange handlers).
    /// </summary>
    /// <param name="handler">
    ///     The delegate to call when an exception occurs. Return an <see cref="ExceptionResult"/> to indicate
    ///     how the exception should be handled.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    /// <remarks>
    ///     <para>
    ///     Exception handlers are invoked in registration order. Return values determine behavior:
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="ExceptionResult.Rethrow"/> - The exception is rethrown with its original stack trace.
    ///         </item>
    ///         <item><see cref="ExceptionResult.Throw"/> - A different exception is thrown instead.</item>
    ///         <item>
    ///             <see cref="ExceptionResult.Continue"/> - The next handler is invoked. If no more handlers exist, the
    ///             exception is rethrown.
    ///         </item>
    ///     </list>
    ///     </para>
    ///     <para>
    ///     If a handler itself throws, that exception propagates immediately.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code>
    ///     StateMachine
    ///         .Configure&lt;State, Transition&gt;()
    ///         .OnException(ex => ex switch
    ///         {
    ///             InvalidOperationException => ExceptionResult.Rethrow(),
    ///             _ => ExceptionResult.Continue()
    ///         });
    ///     </code>
    /// </example>
    IStateMachineConfiguration<TState, TTransition> OnException(Func<Exception, ExceptionResult> handler);

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when an exception is thrown during
    ///     state lifecycle operations (OnActivate, OnEntry, OnExit, OnDeactivate, OnStateChange handlers).
    /// </summary>
    /// <param name="handler">The delegate to call when an exception occurs.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> OnException(
        Func<Exception, CancellationToken, Task<ExceptionResult>> handler
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when an exception is thrown during
    ///     state lifecycle operations (OnActivate, OnEntry, OnExit, OnDeactivate, OnStateChange handlers).
    /// </summary>
    /// <param name="handler">The delegate to call when an exception occurs.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IStateMachineConfiguration<TState, TTransition> OnException(
        Func<Exception, CancellationToken, ValueTask<ExceptionResult>> handler
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
            IFinalTriggerConfiguration<TState, TTransition>
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
    ///     The states in this configuration.
    /// </summary>
    internal IEnumerable<IStateConfiguration<TState, TTransition>> States { get; }
}
