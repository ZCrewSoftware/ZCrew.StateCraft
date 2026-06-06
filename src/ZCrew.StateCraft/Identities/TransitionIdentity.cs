using ZCrew.StateCraft.Identities.Extensions;

namespace ZCrew.StateCraft.Identities;

/// <inheritdoc />
internal class TransitionIdentity<TTransition> : ITransitionIdentity<TTransition>
    where TTransition : notnull
{
    public TransitionIdentity(TTransition transitionValue, IReadOnlyList<Type> transitionParameterTypes)
    {
        TransitionValue = transitionValue;
        TransitionParameterTypes = transitionParameterTypes;
    }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionParameterTypes { get; }

    /// <inheritdoc cref="IIdentity.ToString" />
    public override string ToString()
    {
        return this.ToDisplayString();
    }
}
