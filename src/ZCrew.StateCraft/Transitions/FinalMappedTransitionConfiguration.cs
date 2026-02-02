using System.Diagnostics;
using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
[DebuggerDisplay("{ToDisplayString()}")]
internal class FinalMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext>
    : ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions;
    private readonly Func<TPrevious, TNext> mappingFunction;
    private readonly IReadOnlyList<IAsyncFunc<TNext, bool>> nextConditions;

    public FinalMappedTransitionConfiguration(
        TState previousState,
        TTransition transition,
        TState nextState,
        IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions,
        Func<TPrevious, TNext> mappingFunction,
        IReadOnlyList<IAsyncFunc<TNext, bool>> nextConditions
    )
    {
        PreviousStateValue = previousState;
        TransitionValue = transition;
        NextStateValue = nextState;
        this.previousConditions = previousConditions;
        this.mappingFunction = mappingFunction;
        this.nextConditions = nextConditions;
    }

    /// <inheritdoc />
    public TState PreviousStateValue { get; }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public TState NextStateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> PreviousStateTypeParameters { get; } = [typeof(TPrevious)];

    /// <inheritdoc/>
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc />
    public bool IsConditional => this.previousConditions.Count > 0 || this.nextConditions.Count > 0;

    /// <inheritdoc />
    public ITransition<TState, TTransition> Build(IState<TState, TTransition> state)
    {
        return new MappedTransition<TState, TTransition, TPrevious, TNext>(
            (IParameterizedState<TState, TTransition, TPrevious>)state,
            TransitionValue,
            NextStateValue,
            this.previousConditions,
            this.mappingFunction,
            this.nextConditions
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Mapped Transition: {ToDisplayString()}";
    }

    private string ToDisplayString()
    {
        if (PreviousStateValue.Equals(NextStateValue) && typeof(TPrevious) == typeof(TNext))
        {
            return $"{TransitionValue}({PreviousStateValue}<{typeof(TPrevious).FriendlyName}>) ↩";
        }

        return $"{TransitionValue}({PreviousStateValue}<{typeof(TPrevious).FriendlyName}>) → {NextStateValue}<{typeof(TNext).FriendlyName}>";
    }
}
