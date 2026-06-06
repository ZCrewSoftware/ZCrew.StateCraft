namespace ZCrew.StateCraft.Identities;

/// <summary>
/// Equality comparison for <see cref="IStateIdentity{TState}"/>. Centralizing equality in a comparer (rather than
/// overriding equality members on each implementation) keeps it symmetric and consistent regardless of which
/// <see cref="IStateIdentity{TState}"/> implementation is compared.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
internal class StateIdentityEqualityComparer<TState> : IEqualityComparer<IStateIdentity<TState>>
    where TState : notnull
{
    public static readonly StateIdentityEqualityComparer<TState> Instance = new();

    /// <inheritdoc/>
    public bool Equals(IStateIdentity<TState>? x, IStateIdentity<TState>? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }
        if (x is null)
        {
            return false;
        }
        if (y is null)
        {
            return false;
        }
        return EqualityComparer<TState>.Default.Equals(x.StateValue, y.StateValue)
            && x.StateParameterTypes.SequenceEqual(y.StateParameterTypes);
    }

    /// <inheritdoc/>
    public int GetHashCode(IStateIdentity<TState> obj)
    {
        var hash = new HashCode();
        hash.Add(obj.StateValue);
        foreach (var type in obj.StateParameterTypes)
        {
            hash.Add(type);
        }
        return hash.ToHashCode();
    }
}
