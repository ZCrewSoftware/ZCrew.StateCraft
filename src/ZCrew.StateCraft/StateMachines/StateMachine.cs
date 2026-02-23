using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Nito.AsyncEx;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Parameters;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Tracking;
using ZCrew.StateCraft.Tracking.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;
using ZCrew.StateCraft.Triggers.Contracts;

namespace ZCrew.StateCraft.StateMachines;

/// <inheritdoc />
internal sealed class StateMachine<TState, TTransition> : IStateMachine<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IStateMachineActivator<TState, TTransition> stateMachineActivator;
    private readonly IReadOnlyList<IAsyncAction<TState, TTransition, TState>> onStateChanges;
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
    public ITransition<TState, TTransition>? CurrentTransition { get; set; }

    private async Task ExitState(CancellationToken token)
    {
        if (this.internalState is not InternalState.Active and not InternalState.Recovery)
        {
            return;
        }
        Debug.Assert(PreviousState != null, $"Expected {nameof(PreviousState)} to be set.");
        Debug.Assert(CurrentState == null, $"Expected {nameof(CurrentState)} to be null.");

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

        await PreviousState.Exit(Parameters, token);

        this.internalState = InternalState.Exited;
    }

    private async Task Transition(CancellationToken token)
    {
        if (this.internalState is not InternalState.Exited)
        {
            return;
        }

        Debug.Assert(PreviousState != null, $"Expected {nameof(PreviousState)} to be set.");
        Debug.Assert(CurrentState == null, $"Expected {nameof(CurrentState)} to be null.");
        Debug.Assert(CurrentTransition != null, $"Expected {nameof(CurrentTransition)} to be set.");
        Debug.Assert(NextState != null, $"Expected {nameof(NextState)} to be set.");

        this.internalState = InternalState.Transitioning;
        await CurrentTransition.Transition(Parameters, token);
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
        Debug.Assert(CurrentState == null, $"Expected {nameof(CurrentState)} to be set.");
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
            var actionCts = new CancellationTokenSource();

            // Start the execution of the action and allow the state machine to be transitioned or deactivated.
            // Even if this throws an exception then the caller should have a 'using' statement to ensure the lock is
            // always released.
            var action = CurrentState.Action(Parameters, actionCts.Token);
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

        CurrentTransition = null;
    }

    private void Rollback()
    {
        Debug.Assert(CurrentState == null, $"Expected {nameof(CurrentState)} to be null.");

        Parameters.RollbackTransition();
        NextState = null;
        CurrentState = PreviousState;
        PreviousState = null;

        CurrentTransition = null;
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
        Debug.Assert(
            this.internalState is InternalState.Entering,
            $"Expected {nameof(this.internalState)} to be {InternalState.Entering}."
        );

        await NextState.Enter(Parameters, token);
        Parameters.CommitTransition();
        PreviousState = null;
        CurrentState = NextState;
        NextState = null;
        CurrentTransition = null;
        this.internalState = InternalState.Active;

        // Use a canceled token here so that any async work is canceled immediately; but synchronous work (or work that
        // must run) can just ignore the cancellation token and execute before transitioning to the next state
        var canceledToken = new CancellationToken(true);
        var action = CurrentState.Action(Parameters, canceledToken);
        await action.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
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

            await NextState.Activate(Parameters, token);
            this.internalState = InternalState.Idle;
            await ActivateTriggers(token);
            await EnterState(activationLock, token);
        }
        catch when (CurrentState == null)
        {
            // If there was an exception during activation the state machine is forced to reset fully
            Parameters.Clear();
            NextState = null;
            CurrentState = null;
            PreviousState = null;

            CurrentTransition = null;
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

        if (this.internalState is InternalState.Entering)
        {
            await DeactivateTriggers(token);
            Parameters.Clear();
            PreviousState = null;
            NextState = null;
            CurrentTransition = null;
            this.internalState = InternalState.Inactive;
            return;
        }

        try
        {
            BeginTransition();
            await ExitState(token);
            await DeactivateTriggers(token);

            await PreviousState.Deactivate(Parameters, token);

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
    public async Task Transition(TTransition transition, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetEmptyNextParameters();
            CurrentTransition = await PreviousState.GetTransition(transition, Parameters, token);
            NextState = CurrentTransition.Next.State;
            await ExitState(token);
            await Transition(token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
    }

    /// <inheritdoc />
    public async Task Transition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameter(parameter);
            CurrentTransition = await PreviousState.GetTransition(transition, Parameters, token);
            NextState = CurrentTransition.Next.State;
            await ExitState(token);
            await Transition(token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
    }

    /// <inheritdoc />
    public async Task Transition<T1, T2>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        CancellationToken token = default
    )
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2);
            CurrentTransition = await PreviousState.GetTransition(transition, Parameters, token);
            NextState = CurrentTransition.Next.State;
            await ExitState(token);
            await Transition(token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
    }

    /// <inheritdoc />
    public async Task Transition<T1, T2, T3>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        CancellationToken token = default
    )
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2, parameter3);
            CurrentTransition = await PreviousState.GetTransition(transition, Parameters, token);
            NextState = CurrentTransition.Next.State;
            await ExitState(token);
            await Transition(token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
    }

    /// <inheritdoc />
    public async Task Transition<T1, T2, T3, T4>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4,
        CancellationToken token = default
    )
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2, parameter3, parameter4);
            CurrentTransition = await PreviousState.GetTransition(transition, Parameters, token);
            NextState = CurrentTransition.Next.State;
            await ExitState(token);
            await Transition(token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
    }

    /// <inheritdoc />
    public async Task<bool> CanTransition(TTransition transition, CancellationToken token = default)
    {
        using var _ = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            return false;
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetEmptyNextParameters();
            var nextTransition = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            return nextTransition != null;
        }
        finally
        {
            Rollback();
        }
    }

    /// <inheritdoc />
    public async Task<bool> CanTransition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        using var _ = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            return false;
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameter(parameter);
            var nextTransition = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            return nextTransition != null;
        }
        finally
        {
            Rollback();
        }
    }

    /// <inheritdoc />
    public async Task<bool> CanTransition<T1, T2>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        CancellationToken token = default
    )
    {
        using var _ = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            return false;
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2);
            var nextTransition = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            return nextTransition != null;
        }
        finally
        {
            Rollback();
        }
    }

    /// <inheritdoc />
    public async Task<bool> CanTransition<T1, T2, T3>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        CancellationToken token = default
    )
    {
        using var _ = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            return false;
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2, parameter3);
            var nextTransition = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            return nextTransition != null;
        }
        finally
        {
            Rollback();
        }
    }

    /// <inheritdoc />
    public async Task<bool> CanTransition<T1, T2, T3, T4>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4,
        CancellationToken token = default
    )
    {
        using var _ = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            return false;
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2, parameter3, parameter4);
            var nextTransition = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            return nextTransition != null;
        }
        finally
        {
            Rollback();
        }
    }

    /// <inheritdoc />
    public async Task<bool> TryTransition(TTransition transition, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            return false;
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetEmptyNextParameters();
            CurrentTransition = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (CurrentTransition == null)
            {
                Rollback();
                return false;
            }
        }
        catch
        {
            Rollback();
            throw;
        }

        try
        {
            NextState = CurrentTransition.Next.State;
            await ExitState(token);
            await Transition(token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> TryTransition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            return false;
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameter(parameter);
            CurrentTransition = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (CurrentTransition == null)
            {
                Rollback();
                return false;
            }
        }
        catch
        {
            Rollback();
            throw;
        }

        try
        {
            NextState = CurrentTransition.Next.State;
            await ExitState(token);
            await Transition(token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> TryTransition<T1, T2>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        CancellationToken token = default
    )
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            return false;
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2);
            CurrentTransition = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (CurrentTransition == null)
            {
                Rollback();
                return false;
            }
        }
        catch
        {
            Rollback();
            throw;
        }

        try
        {
            NextState = CurrentTransition.Next.State;
            await ExitState(token);
            await Transition(token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> TryTransition<T1, T2, T3>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        CancellationToken token = default
    )
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            return false;
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2, parameter3);
            CurrentTransition = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (CurrentTransition == null)
            {
                Rollback();
                return false;
            }
        }
        catch
        {
            Rollback();
            throw;
        }

        try
        {
            NextState = CurrentTransition.Next.State;
            await ExitState(token);
            await Transition(token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> TryTransition<T1, T2, T3, T4>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4,
        CancellationToken token = default
    )
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (
            this.internalState is not InternalState.Active and not InternalState.Recovery and not InternalState.Entering
        )
        {
            return false;
        }

        if (this.internalState is InternalState.Entering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2, parameter3, parameter4);
            CurrentTransition = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (CurrentTransition == null)
            {
                Rollback();
                return false;
            }
        }
        catch
        {
            Rollback();
            throw;
        }

        try
        {
            NextState = CurrentTransition.Next.State;
            await ExitState(token);
            await Transition(token);
        }
        catch when (CurrentState == null)
        {
            Rollback();
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
        return true;
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
