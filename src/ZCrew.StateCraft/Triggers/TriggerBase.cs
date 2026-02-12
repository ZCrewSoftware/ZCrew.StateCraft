using System.Diagnostics;
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
    private readonly object executeLock = new();
    private Task? executeTask;
    private CancellationTokenSource? executionCancellationTokenSource;

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

    /// <inheritdoc />
    public Task Activate(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        lock (this.executeLock)
        {
            if (this.executeTask != null)
            {
                return Task.CompletedTask;
            }

            // Start the trigger task in the background - the cancellation token is used to signal the deactivation of
            // the trigger
            this.executionCancellationTokenSource = new CancellationTokenSource();
            this.executeTask = this.StateMachine.ExceptionBehavior.CallTrigger(
                Execute,
                this.executionCancellationTokenSource.Token
            );
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task Deactivate(CancellationToken token)
    {
        // Capture these to local variables to avoid async locking and race conditions
        CancellationTokenSource cts;
        Task task;
        lock (this.executeLock)
        {
            if (this.executeTask == null)
            {
                return;
            }

            cts = this.executionCancellationTokenSource!;
            task = this.executeTask;

            this.executionCancellationTokenSource = null;
            this.executeTask = null;
            TriggeredCount = 0;
        }

        // Dispose of the CTS then await the completion of the task. If the trigger task threw an exception then it is
        // just ignored but still sent to the exception handlers
        await cts.CancelAsync();
        cts.Dispose();
        await task.WaitAsync(token).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
    }

    /// <summary>
    ///     Executes the trigger logic. This method is called in a background task when the trigger is activated.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests. This token is cancelled when the trigger is deactivated.</param>
    /// <returns>A task representing the trigger execution.</returns>
    protected abstract Task Execute(CancellationToken token);
}
