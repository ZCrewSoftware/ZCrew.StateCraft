using ZCrew.StateCraft.Extensions;

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
            var currentTransition = await PreviousState.GetTransition(transition, Parameters, token);
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
            var currentTransition = await PreviousState.GetTransition(transition, Parameters, token);
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
            var currentTransition = await PreviousState.GetTransition(transition, Parameters, token);
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
            var currentTransition = await PreviousState.GetTransition(transition, Parameters, token);
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
            var currentTransition = await PreviousState.GetTransition(transition, Parameters, token);
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
    }
}
