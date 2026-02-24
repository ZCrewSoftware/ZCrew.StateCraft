using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.StateMachines;

internal partial class StateMachine<TState, TTransition>
{
    /// <inheritdoc />
    public async Task<bool> CanTransition(TTransition transition, CancellationToken token = default)
    {
        using var _ = await this.stateMachineLock.LockAsync(token);
        if (!this.internalState.CanAcceptTransition)
        {
            return false;
        }

        if (this.internalState.IsEntering)
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
        if (!this.internalState.CanAcceptTransition)
        {
            return false;
        }

        if (this.internalState.IsEntering)
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
        if (!this.internalState.CanAcceptTransition)
        {
            return false;
        }

        if (this.internalState.IsEntering)
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
        if (!this.internalState.CanAcceptTransition)
        {
            return false;
        }

        if (this.internalState.IsEntering)
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
        if (!this.internalState.CanAcceptTransition)
        {
            return false;
        }

        if (this.internalState.IsEntering)
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
}
