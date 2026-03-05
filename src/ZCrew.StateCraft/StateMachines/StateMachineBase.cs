using System.Diagnostics;

namespace ZCrew.StateCraft.StateMachines;

/// <summary>
///     Non-generic base for the state machine to handle static fields.
/// </summary>
internal abstract class StateMachineBase
{
    private static long stateMachineId;
    private static readonly AsyncLocal<IEnumerable<long>?> asynchronousStateMachineIds = new();

    private readonly long id;

    protected StateMachineBase()
    {
        this.id = Interlocked.Increment(ref stateMachineId);
    }

    protected void AddToAsynchronousCallChain()
    {
        var stateMachineIds = asynchronousStateMachineIds.Value ?? [];
        asynchronousStateMachineIds.Value = stateMachineIds.Append(this.id);
    }

    protected void RemoveFromAsynchronousCallChain()
    {
        var stateMachineIds = asynchronousStateMachineIds.Value?.ToArray() ?? [];
        Debug.Assert(stateMachineIds.Length > 0);
        asynchronousStateMachineIds.Value = stateMachineIds[..^1];
    }

    protected bool IsAsynchronousActionCallingStateMachine()
    {
        var stateMachineIds = asynchronousStateMachineIds.Value?.ToArray() ?? [];
        return stateMachineIds.Length > 0 && stateMachineIds[^1] == this.id;
    }
}
