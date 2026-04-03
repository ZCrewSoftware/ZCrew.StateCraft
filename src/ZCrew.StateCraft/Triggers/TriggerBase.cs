using System.Diagnostics;
using ZCrew.StateCraft.Async.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Triggers.Contracts;

namespace ZCrew.StateCraft.Triggers;

/// <summary>
///     Base class for triggers that provides the activation and deactivation lifecycle management.
///     Triggers are activated when the state machine is activated and deactivated when the state machine is deactivated.
/// </summary>
/// <typeparam name="TState">The type representing the states in the state machine.</typeparam>
/// <typeparam name="TTransition">The type representing the transitions in the state machine.</typeparam>
[DebuggerDisplay("TriggeredCount={TriggeredCount}")]
internal abstract class TriggerBase<TState, TTransition> : ITrigger
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The state machine that this trigger is associated with.
    /// </summary>
    protected readonly IStateMachine<TState, TTransition> StateMachine;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TriggerBase{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="stateMachine">The state machine that this trigger will be associated with.</param>
    protected TriggerBase(IStateMachine<TState, TTransition> stateMachine)
    {
        this.StateMachine = stateMachine;
    }

    /// <inheritdoc />
    public int TriggeredCount { get; protected set; }

    private IBackgroundWorker? worker;

    /// <inheritdoc />
    public async Task Activate(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        TriggeredCount = 0;
        this.worker = await this.StateMachine.BackgroundDispatcher.Dispatch(WrapExecute, token);
        return;

        [StackTraceHidden]
        Task WrapExecute(CancellationToken backgroundToken)
        {
            return this.StateMachine.ExceptionBehavior.CallTrigger(Execute, backgroundToken);
        }
    }

    /// <inheritdoc />
    public async Task Deactivate(CancellationToken token)
    {
        var handleRef = this.worker;
        this.worker = null;
        if (handleRef != null)
        {
            await handleRef.Deactivate(token);
        }
    }

    /// <summary>
    ///     Executes the trigger logic. This method is called in a background task when the trigger is activated.
    /// </summary>
    /// <param name="token">
    ///     The token to monitor for cancellation requests. This token is canceled when the trigger is deactivated.
    /// </param>
    /// <returns>A task representing the trigger execution.</returns>
    protected abstract Task Execute(CancellationToken token);
}
