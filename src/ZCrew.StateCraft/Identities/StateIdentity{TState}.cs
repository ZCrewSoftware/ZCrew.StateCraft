using ZCrew.StateCraft.Identities.Extensions;

namespace ZCrew.StateCraft.Identities;

/// <inheritdoc />
internal class StateIdentity<TState> : IStateIdentity<TState>
    where TState : notnull
{
    public StateIdentity(TState stateValue, IReadOnlyList<Type> stateParameterTypes)
    {
        StateValue = stateValue;
        StateParameterTypes = stateParameterTypes;
    }

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> StateParameterTypes { get; }

    /// <inheritdoc cref="IIdentity.ToString" />
    public override string ToString()
    {
        return this.ToDisplayString();
    }
}
