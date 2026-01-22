using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.Triggers;

/// <summary>
///     A trigger that executes repeatedly each time the signal is received until the state machine is deactivated.
///     After each trigger action completes, the trigger waits for the next signal.
/// </summary>
/// <typeparam name="TState">The type representing the states in the state machine.</typeparam>
/// <typeparam name="TTransition">The type representing the transitions in the state machine.</typeparam>
internal class RepeatingTrigger<TState, TTransition> : TriggerBase<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IAsyncAction signal;
    private readonly IAsyncAction<IStateMachine<TState, TTransition>> trigger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RepeatingTrigger{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="stateMachine">The state machine that this trigger will be associated with.</param>
    /// <param name="signal">The signal to await before each execution of the trigger action.</param>
    /// <param name="trigger">The action to execute each time the signal is received.</param>
    public RepeatingTrigger(
        IStateMachine<TState, TTransition> stateMachine,
        IAsyncAction signal,
        IAsyncAction<IStateMachine<TState, TTransition>> trigger
    )
        : base(stateMachine)
    {
        this.signal = signal;
        this.trigger = trigger;
    }

    /// <inheritdoc />
    protected override async Task Execute(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await this.signal.InvokeAsync(token);
            await this.trigger.InvokeAsync(this.StateMachine, token);
            TriggeredCount++;
        }
    }
}
