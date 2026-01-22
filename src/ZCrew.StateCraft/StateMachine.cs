using ZCrew.StateCraft.StateMachines;

namespace ZCrew.StateCraft;

/// <summary>
///     <para>
///     Represents the starting point for configuring a state machine. This configuration defines the initial state,
///     available states, and any optional behaviors to construct a state machine.
///     </para>
///     <para>
///     Once configuration is complete, call <see cref="IStateMachineConfiguration{TState,TTransition}.Build()"/> to
///     produce a new state machine. The configuration is reusable and may be used to create independent state machine
///     instances that all share the same configuration.
///     </para>
/// </summary>
public static class StateMachine
{
    /// <summary>
    ///     Configure a standard state machine. This is the recommended type for most scenarios.
    /// </summary>
    /// <typeparam name="TState">
    ///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
    ///     machine behaves as expected.
    /// </typeparam>
    /// <typeparam name="TTransition">
    ///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
    ///     state machine behaves as expected.
    /// </typeparam>
    /// <returns>The state machine configuration.</returns>
    /// <example>
    ///     <para>
    ///     This example demonstrates a Terran Marine combat state machine with parameterized states for movement and
    ///     targeting, conditional transitions, and lifecycle handlers.
    ///     </para>
    ///     <code>
    ///     // Define state and transition enums
    ///     enum MarineState { Idle, MovingToPosition, Attacking, Stimpacked }
    ///     enum MarineTransition { Move, Arrive, EngageTarget, TargetDestroyed, UseStimpack, StimpackExpired }
    ///     <br/>
    ///     // Configure the state machine
    ///     var stateMachine = StateMachine
    ///         .Configure&lt;MarineState, MarineTransition&gt;()
    ///         .WithInitialState(MarineState.Idle)
    ///         .OnStateChange((from, transition, to) =&gt;
    ///             Console.WriteLine($"Marine: {from} --[{transition}]--&gt; {to}"))
    ///     <br/>
    ///         // Idle state - awaiting orders
    ///         .WithState(MarineState.Idle, state =&gt; state
    ///             .OnEntry(() =&gt; Console.WriteLine("Marine standing by."))
    ///             .WithTransition&lt;Position&gt;(MarineTransition.Move, MarineState.MovingToPosition)
    ///             .WithTransition(MarineTransition.EngageTarget, t =&gt; t
    ///                 .WithParameter&lt;EnemyUnit&gt;()
    ///                 .If(target =&gt; target.Health &gt; 0)
    ///                 .To(MarineState.Attacking))
    ///             .WithTransition(MarineTransition.UseStimpack, t =&gt; t
    ///                 .If(() =&gt; marineHealth &gt; 50)
    ///                 .To(MarineState.Stimpacked)))
    ///     <br/>
    ///         // MovingToPosition state - parameterized with destination
    ///         .WithState&lt;Position&gt;(MarineState.MovingToPosition, state =&gt; state
    ///             .WithParameter&lt;Position&gt;()
    ///             .OnEntry(pos =&gt; Console.WriteLine($"Moving to ({pos.X}, {pos.Y})"))
    ///             .WithAction(action => action.Invoke(pos =&gt; MoveToPosition(pos)))
    ///             .OnExit(pos =&gt; Console.WriteLine($"Arrived at ({pos.X}, {pos.Y})"))
    ///             .WithTransition(MarineTransition.Arrive, t =&gt; t.To(MarineState.Idle)))
    ///     <br/>
    ///         // Attacking state - parameterized with target
    ///         .WithState&lt;EnemyUnit&gt;(MarineState.Attacking, state =&gt; state
    ///             .WithParameter&lt;EnemyUnit&gt;()
    ///             .OnEntry(target =&gt; Console.WriteLine($"Engaging {target.Name}!"))
    ///             .WithAction(action => action.Invoke(async (target, token) =&gt; await AttackTargetAsync(target, token)))
    ///             .OnExit(target =&gt; Console.WriteLine($"{target.Name} neutralized."))
    ///             .WithTransition(MarineTransition.TargetDestroyed, t =&gt; t.To(MarineState.Idle)))
    ///     <br/>
    ///         // Stimpacked state - enhanced combat mode
    ///         .WithState(MarineState.Stimpacked, state =&gt; state
    ///             .OnEntry(() =&gt; Console.WriteLine("STIMPACK ACTIVATED!"))
    ///             .OnExit(() =&gt; Console.WriteLine("Stimpack effect expired."))
    ///             .WithTransition&lt;EnemyUnit&gt;(MarineTransition.EngageTarget, MarineState.Attacking)
    ///             .WithTransition(MarineTransition.StimpackExpired, MarineState.Idle))
    ///     <br/>
    ///         .Build();
    ///     </code>
    /// </example>
    public static IStateMachineConfiguration<TState, TTransition> Configure<TState, TTransition>()
        where TState : notnull
        where TTransition : notnull
    {
        return new StateMachineConfiguration<TState, TTransition>();
    }
}
