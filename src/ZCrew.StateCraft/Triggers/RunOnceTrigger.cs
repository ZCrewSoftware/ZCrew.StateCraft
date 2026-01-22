using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.Triggers;

/// <summary>
///     A trigger that executes once per activation cycle when the signal is received.
///     After the trigger action completes, the trigger remains dormant until the state machine is deactivated and reactivated.
/// </summary>
/// <typeparam name="TState">The type representing the states in the state machine.</typeparam>
/// <typeparam name="TTransition">The type representing the transitions in the state machine.</typeparam>
internal class RunOnceTrigger<TState, TTransition> : TriggerBase<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IAsyncAction signal;
    private readonly IAsyncAction<IStateMachine<TState, TTransition>> trigger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RunOnceTrigger{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="stateMachine">The state machine that this trigger will be associated with.</param>
    /// <param name="signal">The signal to await before executing the trigger action.</param>
    /// <param name="trigger">The action to execute when the signal is received.</param>
    public RunOnceTrigger(
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
        await this.signal.InvokeAsync(token);
        await this.trigger.InvokeAsync(this.StateMachine, token);
        TriggeredCount++;
    }
}
