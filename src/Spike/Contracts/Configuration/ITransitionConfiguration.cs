namespace Spike.Contracts.Configuration;

public interface ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    TState PreviousStateValue { get; }
    TState NextStateValue { get; }
    TTransition TransitionValue { get; }
    IReadOnlyList<Type> PreviousStateTypeParameters { get; }
    IReadOnlyList<Type> NextStateTypeParameters { get; }
}
