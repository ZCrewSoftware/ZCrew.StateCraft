using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using Nito.AsyncEx;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Tracking;
using ZCrew.StateCraft.Tracking.Contracts;
using ZCrew.StateCraft.Triggers.Contracts;

namespace ZCrew.StateCraft.StateMachines;

/// <inheritdoc />
internal sealed class StateMachine<TState, TTransition> : IStateMachine<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IStateMachineActivator<TState, TTransition> stateMachineActivator;
    private readonly IReadOnlyList<IAsyncAction<TState, TTransition, TState>> onStateChanges;
    private readonly IReadOnlyList<IAsyncFunc<Exception, ExceptionResult>> onExceptionHandlers;
    private readonly StateMachineOptions options;
    private readonly IReadOnlyList<ITrigger> triggers;
    private InternalState internalState;

    private readonly AsyncLock stateMachineLock = new();
    private Task? actionTask;
    private CancellationTokenSource? actionCancellationTokenSource;

    /// <summary>
    ///     Creates a new <see cref="StateMachine{TState,TTransition}"/>.
    /// </summary>
    /// <param name="stateMachineActivator">The initial state producer.</param>
    /// <param name="onStateChanges">The handlers to invoke when the state changes.</param>
    /// <param name="onExceptionHandlers">The handlers to invoke when an exception is thrown.</param>
    /// <param name="stateTable">The state table.</param>
    /// <param name="triggers">The triggers that can control this state machine.</param>
    /// <param name="options">The options to enable certain features on this state machine.</param>
    public StateMachine(
        IStateMachineActivator<TState, TTransition> stateMachineActivator,
        IReadOnlyList<IAsyncAction<TState, TTransition, TState>> onStateChanges,
        IReadOnlyList<IAsyncFunc<Exception, ExceptionResult>> onExceptionHandlers,
        StateTable<TState, TTransition> stateTable,
        IReadOnlyList<ITrigger> triggers,
        StateMachineOptions options
    )
    {
        this.stateMachineActivator = stateMachineActivator;
        this.onStateChanges = onStateChanges;
        this.onExceptionHandlers = onExceptionHandlers;
        StateTable = stateTable;
        this.triggers = triggers;
        this.options = options;

        SetupTracking();
    }

    /// <inheritdoc />
    public StateTable<TState, TTransition> StateTable { get; }

    /// <inheritdoc />
    public ITracker<TState, TTransition>? Tracker { get; private set; }

    /// <inheritdoc />
    public IState<TState, TTransition>? CurrentState { get; set; }

    /// <inheritdoc />
    public IState<TState, TTransition>? PreviousState { get; set; }

    /// <inheritdoc />
    public IState<TState, TTransition>? NextState { get; set; }

    /// <inheritdoc />
    public object? CurrentParameter { get; set; }

    /// <inheritdoc />
    public object? PreviousParameter { get; set; }

    /// <inheritdoc />
    public object? NextParameter { get; set; }

    /// <inheritdoc />
    public ITransition<TState, TTransition>? CurrentTransition { get; set; }

    private async Task ExitState(CancellationToken token)
    {
        if (this.internalState is not InternalState.Active and not InternalState.Recovery)
        {
            return;
        }
        Debug.Assert(PreviousParameter == null, $"Expected {nameof(PreviousParameter)} to be null.");
        Debug.Assert(PreviousState == null, $"Expected {nameof(PreviousState)} to be null.");
        Debug.Assert(CurrentState != null, $"Expected {nameof(CurrentState)} to be set.");

        PreviousParameter = CurrentParameter;
        PreviousState = CurrentState;
        CurrentParameter = null;
        CurrentState = null;

        this.internalState = InternalState.Exiting;
        if (this.options.HasFlag(StateMachineOptions.RunActionsAsynchronously))
        {
            // Clean up the CTS if it wasn't already disposed of
            var actionCts = this.actionCancellationTokenSource;
            if (actionCts != null)
            {
                await actionCts.CancelAsync();
                actionCts.Dispose();
                this.actionCancellationTokenSource = null;
            }

            var task = this.actionTask;
            if (task != null)
            {
                // Avoid catching exceptions here again if the action threw an exception. This will often fail due
                // to an OperationCanceledException, as the action's cancellation token was disposed of
                await task.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
                this.actionTask = null;
            }
        }

        await PreviousState.Exit(token);

        this.internalState = InternalState.Exited;
    }

    private async Task Transition(CancellationToken token)
    {
        if (this.internalState is not InternalState.Exited)
        {
            return;
        }

        Debug.Assert(PreviousState != null, $"Expected {nameof(PreviousParameter)} to be set.");
        Debug.Assert(CurrentParameter == null, $"Expected {nameof(CurrentParameter)} to be null.");
        Debug.Assert(CurrentState == null, $"Expected {nameof(CurrentState)} to be null.");
        Debug.Assert(CurrentTransition != null, $"Expected {nameof(CurrentTransition)} to be set.");
        Debug.Assert(NextState != null, $"Expected {nameof(NextParameter)} to be set.");

        this.internalState = InternalState.Transitioning;
        await CurrentTransition.Transition(token);
        this.internalState = InternalState.Transitioned;

        CurrentTransition = null;
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
        if (this.internalState is not InternalState.Idle and not InternalState.Transitioned)
        {
            return;
        }
        Debug.Assert(CurrentParameter == null, $"Expected {nameof(CurrentParameter)} to be null.");
        Debug.Assert(CurrentState == null, $"Expected {nameof(CurrentState)} to be set.");
        Debug.Assert(NextState != null, $"Expected {nameof(NextState)} to be set.");

        this.internalState = InternalState.Entering;
        await NextState.Enter(token);
        this.internalState = InternalState.Entered;

        PreviousParameter = null;
        PreviousState = null;
        CurrentParameter = NextParameter;
        CurrentState = NextState;
        NextParameter = null;
        NextState = null;

        if (this.options.HasFlag(StateMachineOptions.RunActionsAsynchronously))
        {
            var actionCts = new CancellationTokenSource();

            // Start the execution of the action and allow the state machine to be transitioned or deactivated.
            // Even if this throws an exception then the caller should have a 'using' statement to ensure the lock is
            // always released.
            var action = CurrentState.Action(actionCts.Token);
            this.internalState = InternalState.Active;
            methodLock.Dispose();

            // Either completed already or threw an exception
            if (action.IsCompleted)
            {
                actionCts.Dispose();
                await action;
            }
            else
            {
                // Only store the action CTS if the action isn't already completed
                this.actionCancellationTokenSource = actionCts;
                this.actionTask = action;
            }
        }
        else
        {
            // Start the execution of the action and allow the state machine to be transitioned or deactivated.
            // Even if this throws an exception then the caller should have a 'using' statement to ensure the lock is
            // always released.
            var action = CurrentState.Action(token);
            this.internalState = InternalState.Active;
            methodLock.Dispose();

            // Then await the action - which is now free to transition the state machine without deadlocking
            await action;
        }
    }

    private void Rollback()
    {
        Debug.Assert(CurrentState == null, $"Expected {nameof(CurrentState)} to be null.");

        NextParameter = null;
        NextState = null;
        CurrentParameter = PreviousParameter;
        CurrentState = PreviousState;
        PreviousParameter = null;
        PreviousState = null;

        CurrentTransition = null;
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

    /// <inheritdoc />
    public async Task Activate(CancellationToken token = default)
    {
        using var activationLock = await this.stateMachineLock.LockAsync(token);
        if (this.internalState is not InternalState.Inactive)
        {
            throw new InvalidOperationException("The state machine has already been activated.");
        }

        try
        {
            await this.stateMachineActivator.Activate(this, token);
            Debug.Assert(NextState != null, $"Expected {nameof(NextState)} to be set.");

            await NextState.Activate(token);
            this.internalState = InternalState.Idle;
            await ActivateTriggers(token);
            await EnterState(activationLock, token);
        }
        catch when (CurrentState == null)
        {
            // If there was an exception during activation the state machine is forced to reset fully
            Rollback();
            this.internalState = InternalState.Inactive;
            await DeactivateTriggers(CancellationToken.None);
            throw;
        }
        catch
        {
            await DeactivateTriggers(CancellationToken.None);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task Deactivate(CancellationToken token = default)
    {
        using var _ = await this.stateMachineLock.LockAsync(token);
        if (this.internalState is InternalState.Inactive)
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        try
        {
            await ExitState(token);
            await DeactivateTriggers(token);
            Debug.Assert(PreviousState != null, $"Expected {nameof(CurrentState)} to be set.");

            await PreviousState.Deactivate(token);

            PreviousState = null;
            PreviousParameter = null;
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
    public async Task Transition(TTransition transition, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (this.internalState is not InternalState.Active and not InternalState.Recovery)
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        Debug.Assert(CurrentState != null, $"Expected {nameof(CurrentState)} to be set.");

        try
        {
            CurrentTransition = await CurrentState.GetTransition(transition, token);
            NextParameter = null;
            NextState = CurrentTransition.NextState;
            await ExitState(token);
            await Transition(token);
            await EnterState(transitionLock, token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }
    }

    /// <inheritdoc />
    public async Task Transition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (this.internalState is not InternalState.Active and not InternalState.Recovery)
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        Debug.Assert(CurrentState != null, $"Expected {nameof(CurrentState)} to be set.");

        try
        {
            CurrentTransition = await CurrentState.GetTransition(transition, parameter, token);
            NextParameter = parameter;
            NextState = CurrentTransition.NextState;
            await ExitState(token);
            await Transition(token);
            await EnterState(transitionLock, token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> CanTransition(TTransition transition, CancellationToken token = default)
    {
        using var _ = await this.stateMachineLock.LockAsync(token);
        if (this.internalState is not InternalState.Active and not InternalState.Recovery)
        {
            return false;
        }

        Debug.Assert(CurrentState != null, $"Expected {nameof(CurrentState)} to be set.");

        var nextTransition = await CurrentState.GetTransitionOrDefault(transition, token);
        return nextTransition != null;
    }

    /// <inheritdoc />
    public async Task<bool> CanTransition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        using var _ = await this.stateMachineLock.LockAsync(token);
        if (this.internalState is not InternalState.Active and not InternalState.Recovery)
        {
            return false;
        }

        Debug.Assert(CurrentState != null, $"Expected {nameof(CurrentState)} to be set.");

        var nextTransition = await CurrentState.GetTransitionOrDefault(transition, parameter, token);
        return nextTransition != null;
    }

    /// <inheritdoc />
    public async Task<bool> TryTransition(TTransition transition, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (this.internalState is not InternalState.Active and not InternalState.Recovery)
        {
            return false;
        }

        Debug.Assert(CurrentState != null, $"Expected {nameof(CurrentState)} to be set.");

        CurrentTransition = await CurrentState.GetTransitionOrDefault(transition, token);
        if (CurrentTransition == null)
        {
            return false;
        }

        try
        {
            NextParameter = null;
            NextState = CurrentTransition.NextState;
            await ExitState(token);
            await Transition(token);
            await EnterState(transitionLock, token);
            return true;
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> TryTransition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (this.internalState is not InternalState.Active and not InternalState.Recovery)
        {
            return false;
        }

        Debug.Assert(CurrentState != null, $"Expected {nameof(CurrentState)} to be set.");

        CurrentTransition = await CurrentState.GetTransitionOrDefault(transition, parameter, token);
        if (CurrentTransition == null)
        {
            return false;
        }

        try
        {
            NextParameter = parameter;
            NextState = CurrentTransition.NextState;
            await ExitState(token);
            await Transition(token);
            await EnterState(transitionLock, token);
            return true;
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
            await RunWithExceptionHandling(
                () => onStateChange.InvokeAsync(previousState, transition, nextState, token),
                token
            );
        }
    }

    /// <inheritdoc />
    public Task RunWithExceptionHandling(Func<Task> action, CancellationToken token)
    {
        return RunWithExceptionHandling(action, throwOnCancellation: true, token);
    }

    /// <inheritdoc />
    public async Task RunWithExceptionHandling(Func<Task> action, bool throwOnCancellation, CancellationToken token)
    {
        try
        {
            await action();
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested && !throwOnCancellation)
        {
            // Skip calling handlers and suppress the exception
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Skip calling handlers
            throw;
        }
        catch (Exception ex)
        {
            await HandleException(ExceptionDispatchInfo.Capture(ex), token);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<T> RunWithExceptionHandling<T>(Func<Task<T>> action, CancellationToken token)
    {
        try
        {
            return await action();
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Skip calling handlers
            throw;
        }
        catch (Exception ex)
        {
            await HandleException(ExceptionDispatchInfo.Capture(ex), token);
            throw;
        }
    }

    [DoesNotReturn]
    private async Task HandleException(ExceptionDispatchInfo exceptionInfo, CancellationToken token)
    {
        foreach (var handler in this.onExceptionHandlers)
        {
            var result = await handler.InvokeAsync(exceptionInfo.SourceException, token);
            switch (result)
            {
                case ExceptionResult.RethrowResult:
                    exceptionInfo.Throw();
                    break;
                case ExceptionResult.ThrowResult { Exception: var exception }:
                    throw exception;
                case ExceptionResult.ContinueResult:
                    continue;
            }
        }

        // No handler made a decision, rethrow with original stack trace
        exceptionInfo.Throw();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        var actionCts = this.actionCancellationTokenSource;

        this.actionTask = null;
        this.actionCancellationTokenSource = null;

        actionCts?.Cancel();
        actionCts?.Dispose();
    }

    [Conditional("DEBUG")]
    private void SetupTracking()
    {
        Tracker = new DebugTracker<TState, TTransition>();
    }

    /// <summary>
    ///     The internal state of the state machine.
    /// </summary>
    private enum InternalState
    {
        /// <summary>
        ///     The state machine has not been activated.
        /// </summary>
        Inactive,

        /// <summary>
        ///     The state machine is active and ready for transitions.
        /// </summary>
        Active,

        /// <summary>
        ///     The state machine has completed activation and is ready to enter the next state.
        /// </summary>
        Idle,

        /// <summary>
        ///     The state machine is exiting a state.
        /// </summary>
        Exiting,

        /// <summary>
        ///     The state machine has exited the state.
        /// </summary>
        Exited,

        /// <summary>
        ///     The state machine is in the process of transitioning between states.
        /// </summary>
        Transitioning,

        /// <summary>
        ///     The state machine has transitioned: setting the next state, setting the next parameter, and has invoked
        ///     the state change handlers. The state machine is ready to enter the next state.
        /// </summary>
        Transitioned,

        /// <summary>
        ///     The state machine is entering a state.
        /// </summary>
        Entering,

        /// <summary>
        ///     The state machine has entered the state and the state action is ready to be ran.
        /// </summary>
        Entered,

        /// <summary>
        ///     The state machine failed to transition. It was rolled-back to it's previous state but did not re-enter
        ///     or restart the actions for that state.
        /// </summary>
        Recovery,
    }
}
