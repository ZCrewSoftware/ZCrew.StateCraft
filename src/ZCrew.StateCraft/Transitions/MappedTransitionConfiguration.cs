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
        $"{this.transitionValue}({this.previousStateValue}<{typeof(TPrevious).FriendlyName}>) → ?<{typeof(TNext).FriendlyName}>";

    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions;
    private readonly Func<TPrevious, TNext> mappingFunction;
    private readonly List<IAsyncFunc<TNext, bool>> nextConditions = [];
    private readonly TState previousStateValue;
    private readonly TTransition transitionValue;

    public MappedTransitionConfiguration(
        TState previousState,
        TTransition transition,
        IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions,
        Func<TPrevious, TNext> mappingFunction
    )
    {
        this.previousStateValue = previousState;
        this.transitionValue = transition;
        this.previousConditions = previousConditions;
        this.mappingFunction = mappingFunction;
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> To(TState state)
    {
        return new FinalMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext>(
            this.previousStateValue,
            this.transitionValue,
            state,
            this.previousConditions,
            this.mappingFunction,
            this.nextConditions
        );
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousStateValue);
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
