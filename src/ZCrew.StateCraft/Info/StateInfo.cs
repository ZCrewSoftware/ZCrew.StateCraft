namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal class StateInfo<TState, TTransition> : IStateInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StateInfo(
        IStateMachineInfo<TState, TTransition> stateMachine,
        TState stateValue,
        IReadOnlyList<Type> stateParameterTypes
    )
    {
        StateMachine = stateMachine;
        StateValue = stateValue;
        StateParameterTypes = stateParameterTypes;
    }

    /// <inheritdoc />
    public IStateMachineInfo<TState, TTransition> StateMachine { get; }

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> StateParameterTypes { get; }

    /// <summary>
    ///     Determines whether <paramref name="obj"/> is a state with the same identity — the same state value and the
    ///     same parameter types in the same order.
    /// </summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns><see langword="true"/> if the states share the same identity; otherwise <see langword="false"/>.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is StateInfo<TState, TTransition> other
            && EqualityComparer<TState>.Default.Equals(StateValue, other.StateValue)
            && StateParameterTypes.SequenceEqual(other.StateParameterTypes);
    }

    /// <summary>
    ///     Returns a hash code derived from the state value and parameter types, consistent with
    ///     <see cref="Equals(object?)"/>.
    /// </summary>
    /// <returns>A hash code for the current state.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(StateValue);
        foreach (var type in StateParameterTypes)
        {
            hash.Add(type);
        }

        return hash.ToHashCode();
    }
}
