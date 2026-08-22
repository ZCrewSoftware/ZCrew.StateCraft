using ZCrew.StateCraft.Extensions;
using ZCrew.StateCraft.Tracking;

namespace ZCrew.StateCraft.StateMachines;

internal partial class StateMachine<TState, TTransition>
{
    /// <inheritdoc />
    public async Task Transition(TTransition transition, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (!this.internalState.CanAcceptTransition)
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState.IsEntering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetEmptyNextParameters();
            Tracker?.TransitionQuerying(
                TransitionQueryKind.Transition,
                transition,
                PreviousState,
                Parameters.CaptureNext()
            );
            var currentTransition = await PreviousState.GetTransition(transition, Parameters, token);
            NextState = currentTransition.Next.State;
            await ExitState(token);
            await ExecuteTransition(currentTransition, token);
        }
        catch (Exception exception) when (CurrentState == null)
        {
            Rollback(exception);
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
    }

    /// <inheritdoc />
    public async Task Transition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (!this.internalState.CanAcceptTransition)
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState.IsEntering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameter(parameter);
            Tracker?.TransitionQuerying(
                TransitionQueryKind.Transition,
                transition,
                PreviousState,
                Parameters.CaptureNext()
            );
            var currentTransition = await PreviousState.GetTransition(transition, Parameters, token);
            NextState = currentTransition.Next.State;
            await ExitState(token);
            await ExecuteTransition(currentTransition, token);
        }
        catch (Exception exception) when (CurrentState == null)
        {
            Rollback(exception);
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
        if (!this.internalState.CanAcceptTransition)
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState.IsEntering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2);
            Tracker?.TransitionQuerying(
                TransitionQueryKind.Transition,
                transition,
                PreviousState,
                Parameters.CaptureNext()
            );
            var currentTransition = await PreviousState.GetTransition(transition, Parameters, token);
            NextState = currentTransition.Next.State;
            await ExitState(token);
            await ExecuteTransition(currentTransition, token);
        }
        catch (Exception exception) when (CurrentState == null)
        {
            Rollback(exception);
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
        if (!this.internalState.CanAcceptTransition)
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState.IsEntering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2, parameter3);
            Tracker?.TransitionQuerying(
                TransitionQueryKind.Transition,
                transition,
                PreviousState,
                Parameters.CaptureNext()
            );
            var currentTransition = await PreviousState.GetTransition(transition, Parameters, token);
            NextState = currentTransition.Next.State;
            await ExitState(token);
            await ExecuteTransition(currentTransition, token);
        }
        catch (Exception exception) when (CurrentState == null)
        {
            Rollback(exception);
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
        if (!this.internalState.CanAcceptTransition)
        {
            throw new InvalidOperationException("The state machine has not been activated.");
        }

        if (this.internalState.IsEntering)
        {
            await RetryEntry(token);
        }

        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2, parameter3, parameter4);
            Tracker?.TransitionQuerying(
                TransitionQueryKind.Transition,
                transition,
                PreviousState,
                Parameters.CaptureNext()
            );
            var currentTransition = await PreviousState.GetTransition(transition, Parameters, token);
            NextState = currentTransition.Next.State;
            await ExitState(token);
            await ExecuteTransition(currentTransition, token);
        }
        catch (Exception exception) when (CurrentState == null)
        {
            Rollback(exception);
            this.internalState = InternalState.Recovery;
            throw;
        }

        await EnterState(transitionLock, token);
    }
}
