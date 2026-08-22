using ZCrew.StateCraft.Extensions;
using ZCrew.StateCraft.Tracking;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.StateMachines;

internal partial class StateMachine<TState, TTransition>
{
    /// <inheritdoc />
    public async Task<bool> TryTransition(TTransition transition, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (!this.internalState.CanAcceptTransition)
        {
            return false;
        }

        if (this.internalState.IsEntering)
        {
            await RetryEntry(token);
        }

        ITransition<TState, TTransition> currentTransition;
        try
        {
            BeginTransition();
            Parameters.SetEmptyNextParameters();
            Tracker?.TransitionQuerying(
                TransitionQueryKind.TryTransition,
                transition,
                PreviousState,
                Parameters.CaptureNext()
            );
            var resolved = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (resolved == null)
            {
                Tracker?.TransitionNotFound(
                    TransitionQueryKind.TryTransition,
                    transition,
                    PreviousState,
                    Parameters.CaptureNext()
                );
                Rollback();
                return false;
            }

            currentTransition = resolved;
        }
        catch (Exception exception)
        {
            Rollback(exception);
            throw;
        }

        try
        {
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
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> TryTransition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        using var transitionLock = await this.stateMachineLock.LockAsync(token);
        if (!this.internalState.CanAcceptTransition)
        {
            return false;
        }

        if (this.internalState.IsEntering)
        {
            await RetryEntry(token);
        }

        ITransition<TState, TTransition> currentTransition;
        try
        {
            BeginTransition();
            Parameters.SetNextParameter(parameter);
            Tracker?.TransitionQuerying(
                TransitionQueryKind.TryTransition,
                transition,
                PreviousState,
                Parameters.CaptureNext()
            );
            var resolved = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (resolved == null)
            {
                Tracker?.TransitionNotFound(
                    TransitionQueryKind.TryTransition,
                    transition,
                    PreviousState,
                    Parameters.CaptureNext()
                );
                Rollback();
                return false;
            }

            currentTransition = resolved;
        }
        catch (Exception exception)
        {
            Rollback(exception);
            throw;
        }

        try
        {
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
        if (!this.internalState.CanAcceptTransition)
        {
            return false;
        }

        if (this.internalState.IsEntering)
        {
            await RetryEntry(token);
        }

        ITransition<TState, TTransition> currentTransition;
        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2);
            Tracker?.TransitionQuerying(
                TransitionQueryKind.TryTransition,
                transition,
                PreviousState,
                Parameters.CaptureNext()
            );
            var resolved = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (resolved == null)
            {
                Tracker?.TransitionNotFound(
                    TransitionQueryKind.TryTransition,
                    transition,
                    PreviousState,
                    Parameters.CaptureNext()
                );
                Rollback();
                return false;
            }

            currentTransition = resolved;
        }
        catch (Exception exception)
        {
            Rollback(exception);
            throw;
        }

        try
        {
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
        if (!this.internalState.CanAcceptTransition)
        {
            return false;
        }

        if (this.internalState.IsEntering)
        {
            await RetryEntry(token);
        }

        ITransition<TState, TTransition> currentTransition;
        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2, parameter3);
            Tracker?.TransitionQuerying(
                TransitionQueryKind.TryTransition,
                transition,
                PreviousState,
                Parameters.CaptureNext()
            );
            var resolved = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (resolved == null)
            {
                Tracker?.TransitionNotFound(
                    TransitionQueryKind.TryTransition,
                    transition,
                    PreviousState,
                    Parameters.CaptureNext()
                );
                Rollback();
                return false;
            }

            currentTransition = resolved;
        }
        catch (Exception exception)
        {
            Rollback(exception);
            throw;
        }

        try
        {
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
        if (!this.internalState.CanAcceptTransition)
        {
            return false;
        }

        if (this.internalState.IsEntering)
        {
            await RetryEntry(token);
        }

        ITransition<TState, TTransition> currentTransition;
        try
        {
            BeginTransition();
            Parameters.SetNextParameters(parameter1, parameter2, parameter3, parameter4);
            Tracker?.TransitionQuerying(
                TransitionQueryKind.TryTransition,
                transition,
                PreviousState,
                Parameters.CaptureNext()
            );
            var resolved = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (resolved == null)
            {
                Tracker?.TransitionNotFound(
                    TransitionQueryKind.TryTransition,
                    transition,
                    PreviousState,
                    Parameters.CaptureNext()
                );
                Rollback();
                return false;
            }

            currentTransition = resolved;
        }
        catch (Exception exception)
        {
            Rollback(exception);
            throw;
        }

        try
        {
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
        return true;
    }
}
