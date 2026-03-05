using System.Collections.Immutable;

namespace ZCrew.StateCraft.StateMachines;

/// <summary>
///     Non-generic base for the state machine to handle static fields.
/// </summary>
internal class StateMachineBase
{
    protected static int StateMachineId = 0;
    protected static readonly AsyncLocal<ImmutableHashSet<int>?> AsynchronousStateMachineIds = new();
}
