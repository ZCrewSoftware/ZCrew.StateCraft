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
        : base("No transition path exists from this state to the other state.") { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnreachableStateException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public UnreachableStateException(string message)
        : base(message) { }

    /// <summary>
    ///     Creates a new <see cref="UnreachableStateException"/> when trying to find a path between
    ///     <paramref name="from"/> and <paramref name="to"/>.
    /// </summary>
    /// <param name="from">The initial state.</param>
    /// <param name="to">The target state.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <returns>An exception representing this error.</returns>
    public static UnreachableStateException ForPath<TState>(IStateIdentity<TState> from, IStateIdentity<TState> to)
        where TState : notnull
    {
        return new UnreachableStateException($"No transition path exists from {from} to {to}");
    }
}
