using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.StateMachines;

/// <summary>
///     Non-generic base for the state machine to handle static fields.
/// </summary>
internal abstract class StateMachineBase
{
    private static long stateMachineId;
    private static readonly AsyncLocal<long[]?> asynchronousStateMachineIds = new();

    private readonly long id;

    protected StateMachineBase()
    {
        this.id = Interlocked.Increment(ref stateMachineId);
    }

    /// <summary>
    ///     Adds this state machine to the call-chain for monitoring self-referential asynchronous action calls. Once
    ///     added the state machine will then be able to check if the action is trying to transition the state machine
    ///     using either <see cref="IStateMachine{TState,TTransition}.Transition"/>,
    ///     <see cref="IStateMachine{TState,TTransition}.TryTransition"/>,
    ///     or <see cref="IStateMachine{TState,TTransition}.Deactivate"/>.
    /// </summary>
    protected void AddToAsynchronousCallChain()
    {
        var stateMachineIds = asynchronousStateMachineIds.Value ?? [];
        asynchronousStateMachineIds.Value = stateMachineIds.Append(this.id).ToArray();
    }

    /// <summary>
    ///     Checks if an action from a state machine is calling either
    ///     <see cref="IStateMachine{TState,TTransition}.Transition"/>,
    ///     <see cref="IStateMachine{TState,TTransition}.TryTransition"/>,
    ///     or <see cref="IStateMachine{TState,TTransition}.Deactivate"/> on it's own state machine. If it is, the
    ///     state machine is removed from the call-chain and the state machine can avoid deadlocking.
    /// </summary>
    /// <returns><see langword="true"/> if the current state machine is at the top of the call-chain.</returns>
    protected bool RemoveFromAsynchronousCallChain()
    {
        var stateMachineIds = asynchronousStateMachineIds.Value?.ToArray();
        if (stateMachineIds == null || stateMachineIds.Length == 0 || stateMachineIds[^1] != this.id)
        {
            return false;
        }

        asynchronousStateMachineIds.Value = stateMachineIds[..^1];
        return true;
    }
}
