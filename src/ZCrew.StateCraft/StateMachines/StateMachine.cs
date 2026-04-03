using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Nito.AsyncEx;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Async;
using ZCrew.StateCraft.Async.Contracts;
using ZCrew.StateCraft.Extensions;
using ZCrew.StateCraft.Parameters;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Tracking;
using ZCrew.StateCraft.Tracking.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;
using ZCrew.StateCraft.Triggers.Contracts;

namespace ZCrew.StateCraft.StateMachines;

/// <inheritdoc/>
internal sealed partial class StateMachine<TState, TTransition> : IStateMachine<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly AsyncLock stateMachineLock = new();
    private readonly IStateMachineActivator<TState, TTransition> stateMachineActivator;
    private readonly IReadOnlyList<IAsyncAction<TState, TTransition, TState>> onStateChanges;
    private readonly StateMachineOptions options;
    private readonly IReadOnlyList<ITrigger> triggers;
    private InternalState internalState;
    private IBackgroundWorker? actionBackgroundWorker;

    /// <summary>
    ///     Creates a new <see cref="StateMachine{TState,TTransition}"/>.
    /// </summary>
    /// <param name="stateMachineActivator">The initial state producer.</param>
    /// <param name="onStateChanges">The handlers to invoke when the state changes.</param>
    /// <param name="triggers">The triggers that can control this state machine.</param>
    /// <param name="options">The options to enable certain features on this state machine.</param>
    /// <param name="exceptionBehavior">The exception behavior.</param>
    public StateMachine(
        IStateMachineActivator<TState, TTransition> stateMachineActivator,
        IReadOnlyList<IAsyncAction<TState, TTransition, TState>> onStateChanges,
        IReadOnlyList<ITrigger> triggers,
        StateMachineOptions options,
        IExceptionBehavior exceptionBehavior
    )
    {
        this.stateMachineActivator = stateMachineActivator;
        this.onStateChanges = onStateChanges;
        this.triggers = triggers;
        this.options = options;

        ExceptionBehavior = exceptionBehavior;

        SetupTracking();
    }

    /// <inheritdoc />
    public StateTable<TState, TTransition> StateTable { get; } = [];

    /// <inheritdoc />
    public ITracker<TState, TTransition>? Tracker { get; private set; }

    /// <inheritdoc />
    public IExceptionBehavior ExceptionBehavior { get; }

    /// <inheritdoc />
    public IState<TState, TTransition>? CurrentState { get; set; }

    /// <inheritdoc />
    public IState<TState, TTransition>? PreviousState { get; set; }

    /// <inheritdoc />
    public IState<TState, TTransition>? NextState { get; set; }

    /// <inheritdoc />
    public IStateMachineParameters Parameters { get; } = new StateMachineParameters();

    /// <inheritdoc />
    public IBackgroundDispatcher BackgroundDispatcher { get; } = new BackgroundDispatcher();

    /// <inheritdoc />
    public async Task Activate(CancellationToken token = default)
    {
        using var activationLock = await this.stateMachineLock.LockAsync(token);
        if (this.internalState.IsActivated)
        {
            throw new InvalidOperationException("The state machine has already been activated.");
        }

        try
        {
            await this.stateMachineActivator.Activate(this, token);
            Debug.Assert(NextState != null, $"Expected {nameof(NextState)} to be set.");

            await NextState.Activate(Parameters, token);
        }
        catch
        {
            // If OnActivate itself throws, reset fully without calling Deactivate
            Parameters.Clear();
            NextState = null;
            CurrentState = null;
            PreviousState = null;

            this.internalState = InternalState.Inactive;
            throw;
        }

        try
        {
            this.internalState = InternalState.Idle;
            await ActivateTriggers(token);
            await EnterState(activationLock, token);
        }
        catch when (CurrentState == null)
        {
            await DeactivateTriggers(CancellationToken.None);

            // TODO: this has prompted a conversation: https://github.com/ZCrewSoftware/ZCrew.StateCraft/discussions/141
            // OnActivate succeeded but a later step failed - unwind OnActivate.
            // Deactivate reads from the Previous parameters, so shift them over.
            Parameters.CommitTransition();
            Parameters.BeginTransition();
            await NextState!.Deactivate(Parameters, CancellationToken.None);

            // If enable triggers or activating the state throws, reset fully
            Parameters.Clear();
            NextState = null;
            CurrentState = null;
            PreviousState = null;

            this.internalState = InternalState.Inactive;
            throw;
        }
    }

    /// <inheritdoc />
    public async Task Deactivate(CancellationToken token = default)
    {
        using var _ = await this.stateMachineLock.LockAsync(token);
        if (this.internalState.IsInactive)
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState.IsEntering)
        {
            await DeactivateTriggers(token);
            if (PreviousState != null)
            {
                await PreviousState.Deactivate(Parameters, token);
            }
            Parameters.Clear();
            PreviousState = null;
            NextState = null;
            this.internalState = InternalState.Inactive;
            return;
        }

        try
        {
            BeginTransition();
            await ExitState(token);
            await DeactivateTriggers(token);
            await PreviousState.Deactivate(Parameters, token);

            // An action has deactivated the state machine and the token needs to be cleaned up before deactivating
            // The 'token' here may be the action's cancellation token
            await BackgroundDispatcher.Flush(token);

            Parameters.Clear();
            PreviousState = null;
            this.internalState = InternalState.Inactive;
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }
    }

    /// <inheritdoc />
    public async Task StateChange(
        TState previousState,
        TTransition transition,
        TState nextState,
        CancellationToken token
    )
    {
        foreach (var onStateChange in this.onStateChanges)
        {
            await ExceptionBehavior.CallOnStateChange(
                t => onStateChange.InvokeAsync(previousState, transition, nextState, t),
                token
            );
        }
    }

    /// <inheritdoc />
    public void AddState(IState<TState, TTransition> state)
    {
        StateTable.Add(state);
    }

    private async Task ExitState(CancellationToken token)
    {
        Debug.Assert(PreviousState != null, $"Expected {nameof(PreviousState)} to be set.");
        Debug.Assert(CurrentState == null, $"Expected {nameof(CurrentState)} to be null.");

        this.internalState = InternalState.Exiting;
        if (this.options.HasFlag(StateMachineOptions.RunActionsAsynchronously))
        {
            await DeactivateActions(token);
        }

        await PreviousState.Exit(Parameters, token);

        this.internalState = InternalState.Exited;
    }

    private async Task ExecuteTransition(ITransition<TState, TTransition> currentTransition, CancellationToken token)
    {
        Debug.Assert(PreviousState != null, $"Expected {nameof(PreviousState)} to be set.");
        Debug.Assert(NextState != null, $"Expected {nameof(NextState)} to be set.");

        this.internalState = InternalState.Transitioning;
        await currentTransition.Transition(Parameters, token);
        this.internalState = InternalState.Transitioned;
    }

    /// <param name="methodLock">The state machine lock to release.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">If this state machine is in an unexpected state.</exception>
    /// <remarks>
    ///     This method will also eagerly release the lock on this state machine once the state action has been started
    ///     asynchronously - regardless if the state machine will wait for the completion of the action. Therefore, any
    ///     work done after calling <see cref="EnterState"/> is inherently not thread-safe.
    /// </remarks>
    private async Task EnterState(IDisposable methodLock, CancellationToken token)
    {
        Debug.Assert(NextState != null, $"Expected {nameof(NextState)} to be set.");

        this.internalState = InternalState.Entering;
        await NextState.Enter(Parameters, token);
        this.internalState = InternalState.Entered;

        Parameters.CommitTransition();
        PreviousState = null;
        CurrentState = NextState;
        NextState = null;

        if (this.options.HasFlag(StateMachineOptions.RunActionsAsynchronously))
        {
            // Start the execution of the action in the background
            // Note: Current state + parameters will be grabbed before the task is awaited
            this.actionBackgroundWorker = await BackgroundDispatcher.Dispatch(ActivateActions, token);
            await BackgroundDispatcher.Flush(token);

            this.internalState = InternalState.Active;

            methodLock.Dispose();
        }
        else
        {
            // Start the execution of the action and allow the state machine to be transitioned or deactivated.
            // Even if this throws an exception then the caller should have a 'using' statement to ensure the lock is
            // always released.
            var action = CurrentState.Action(Parameters, token);
            this.internalState = InternalState.Active;
            methodLock.Dispose();

            // Then await the action - which is now free to transition the state machine without deadlocking
            await action;
        }
    }

    [MemberNotNull(nameof(PreviousState))]
    private void BeginTransition()
    {
        Debug.Assert(CurrentState != null, $"Expected {nameof(CurrentState)} to be set.");

        Parameters.BeginTransition();
        PreviousState = CurrentState;
        CurrentState = null;
        NextState = null;
    }

    private void Rollback()
    {
        Parameters.RollbackTransition();
        NextState = null;
        CurrentState = PreviousState;
        PreviousState = null;
    }

    /// <summary>
    ///     Retries entry for a state that is in <see cref="InternalState.Entering"/>. Parameters are
    ///     still in the next slot from the original transition, so <c>Enter</c> reads them naturally.
    ///     On success, commits state pointers and parameters, then starts the action with immediate
    ///     cancellation so that synchronous initialization code is guaranteed to run.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    private async Task RetryEntry(CancellationToken token)
    {
        Debug.Assert(NextState != null, $"Expected {nameof(NextState)} to be set.");

        await NextState.Enter(Parameters, token);
        Parameters.CommitTransition();
        PreviousState = null;
        CurrentState = NextState;
        NextState = null;
        this.internalState = InternalState.Active;

        // Use a canceled token here so that any async work is canceled immediately; but synchronous work (or work that
        // must run) can just ignore the cancellation token and execute before transitioning to the next state
        var canceledToken = new CancellationToken(true);
        var action = CurrentState.Action(Parameters, canceledToken);
        await action.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
    }

    private Task ActivateActions(CancellationToken token)
    {
        Debug.Assert(CurrentState != null, $"Expected {nameof(CurrentState)} to be set.");
        return CurrentState.Action(Parameters, token);
    }

    private Task DeactivateActions(CancellationToken token)
    {
        return this.actionBackgroundWorker?.Deactivate(token) ?? Task.CompletedTask;
    }

    private async Task ActivateTriggers(CancellationToken token)
    {
        foreach (var trigger in this.triggers)
        {
            await trigger.Activate(token);
        }
    }

    private async Task DeactivateTriggers(CancellationToken token)
    {
        foreach (var trigger in this.triggers)
        {
            await trigger.Deactivate(token);
        }
    }

    [Conditional("DEBUG")]
    private void SetupTracking()
    {
        Tracker = new DebugTracker<TState, TTransition>();
    }
}
