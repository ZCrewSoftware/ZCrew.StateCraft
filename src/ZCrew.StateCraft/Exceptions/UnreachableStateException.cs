namespace ZCrew.StateCraft;

/// <summary>
///     The exception thrown when a path between two states is requested but no sequence of transitions connects them.
/// </summary>
public sealed class UnreachableStateException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnreachableStateException"/> class.
    /// </summary>
    public UnreachableStateException()
        : base("No transition path exists from this state to the other state.")
    {
        // TODO: clarify exception when it isn't tedious to get string version of states
    }
}
