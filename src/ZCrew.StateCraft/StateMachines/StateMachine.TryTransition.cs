using ZCrew.StateCraft.Extensions;
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
            var resolved = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (resolved == null)
            {
                Rollback();
                return false;
            }

            currentTransition = resolved;
        }
        catch
        {
            Rollback();
            throw;
        }

        try
        {
            NextState = currentTransition.Next.State;
            await ExitState(token);
            await ExecuteTransition(currentTransition, token);
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
            var resolved = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (resolved == null)
            {
                Rollback();
                return false;
            }

            currentTransition = resolved;
        }
        catch
        {
            Rollback();
            throw;
        }

        try
        {
            NextState = currentTransition.Next.State;
            await ExitState(token);
            await ExecuteTransition(currentTransition, token);
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
            var resolved = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (resolved == null)
            {
                Rollback();
                return false;
            }

            currentTransition = resolved;
        }
        catch
        {
            Rollback();
            throw;
        }

        try
        {
            NextState = currentTransition.Next.State;
            await ExitState(token);
            await ExecuteTransition(currentTransition, token);
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
            var resolved = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (resolved == null)
            {
                Rollback();
                return false;
            }

            currentTransition = resolved;
        }
        catch
        {
            Rollback();
            throw;
        }

        try
        {
            NextState = currentTransition.Next.State;
            await ExitState(token);
            await ExecuteTransition(currentTransition, token);
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
            var resolved = await PreviousState.GetTransitionOrDefault(transition, Parameters, token);
            if (resolved == null)
            {
                Rollback();
                return false;
            }

            currentTransition = resolved;
        }
        catch
        {
            Rollback();
            throw;
        }

        try
        {
            NextState = currentTransition.Next.State;
            await ExitState(token);
            await ExecuteTransition(currentTransition, token);
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
}
