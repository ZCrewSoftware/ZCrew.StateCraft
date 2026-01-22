using System.Diagnostics;
using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
[DebuggerDisplay("{DisplayString}")]
internal class MappedTransitionConfiguration<TState, TTransition, TPrevious, TNext>
    : IMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString =>
        $"{TransitionValue}({PreviousStateValue}<{typeof(TPrevious).FriendlyName}>) → ?<{typeof(TNext).FriendlyName}>";

    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions;
    private readonly Func<TPrevious, TNext> mappingFunction;
    private readonly List<IAsyncFunc<TNext, bool>> nextConditions = [];

    public MappedTransitionConfiguration(
        TState previousState,
        TTransition transition,
        IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions,
        Func<TPrevious, TNext> mappingFunction
    )
    {
        PreviousStateValue = previousState;
        TransitionValue = transition;
        this.previousConditions = previousConditions;
        this.mappingFunction = mappingFunction;
    }

    /// <inheritdoc/>
    public TState PreviousStateValue { get; }

    /// <inheritdoc/>
    public TTransition TransitionValue { get; }

    /// <inheritdoc/>
    public TState? NextStateValue { get; } = default;

    /// <inheritdoc/>
    public IReadOnlyList<Type> PreviousStateTypeParameters { get; } = [typeof(TPrevious)];

    /// <inheritdoc/>
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc/>
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc/>
    public bool IsConditional => this.previousConditions.Count > 0 || this.nextConditions.Count > 0;

    /// <inheritdoc />
    public IFinalTransitionConfiguration<TState, TTransition, TPrevious> To(TState state)
    {
        return new FinalMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext>(
            PreviousStateValue,
            TransitionValue,
            state,
            this.previousConditions,
            this.mappingFunction,
            this.nextConditions
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext> If(Func<TNext, bool> condition)
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext> If(
        Func<TNext, CancellationToken, Task<bool>> condition
    )
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext> If(
        Func<TNext, CancellationToken, ValueTask<bool>> condition
    )
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Mapped Transition: {DisplayString}";
    }
}
