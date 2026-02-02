using System.Diagnostics;
using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc cref="IParameterizedTransitionConfiguration{TState,TTransition,TNext}"/>
[DebuggerDisplay("{DisplayString}")]
internal class ParameterizedTransitionConfiguration<TState, TTransition, TNext>
    : IParameterizedTransitionConfiguration<TState, TTransition, TNext>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString =>
        $"{this.transitionValue}({this.previousStateValue}) → ?<{typeof(TNext).FriendlyName}>";

    private readonly IReadOnlyList<IAsyncFunc<bool>> previousConditions;
    private readonly List<IAsyncFunc<TNext, bool>> nextConditions = [];
    private readonly TState previousStateValue;
    private readonly TTransition transitionValue;

    public ParameterizedTransitionConfiguration(
        TState previousState,
        TTransition transition,
        IReadOnlyList<IAsyncFunc<bool>> previousConditions
    )
    {
        this.previousStateValue = previousState;
        this.transitionValue = transition;
        this.previousConditions = previousConditions;
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> To(TState state)
    {
        return new FinalParameterizedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateValue,
            this.transitionValue,
            state,
            this.previousConditions,
            this.nextConditions
        );
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousStateValue);
    }

    /// <inheritdoc />
    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> If(Func<TNext, bool> condition)
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> If(
        Func<TNext, CancellationToken, Task<bool>> condition
    )
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> If(
        Func<TNext, CancellationToken, ValueTask<bool>> condition
    )
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {DisplayString}";
    }
}

/// <inheritdoc cref="IParameterizedTransitionConfiguration{TState,TTransition,TPrevious,TNext}"/>
[DebuggerDisplay("{DisplayString}")]
internal class ParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext>
    : IParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString =>
        $"{this.transitionValue}({this.previousStateValue}<{typeof(TPrevious).FriendlyName}>) → ?<{typeof(TNext).FriendlyName}>";

    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions;
    private readonly List<IAsyncFunc<TNext, bool>> nextConditions = [];
    private readonly TState previousStateValue;
    private readonly TTransition transitionValue;

    public ParameterizedTransitionConfiguration(
        TState previousState,
        TTransition transition,
        IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions
    )
    {
        this.previousStateValue = previousState;
        this.transitionValue = transition;
        this.previousConditions = previousConditions;
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> To(TState state)
    {
        return new FinalParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext>(
            this.previousStateValue,
            this.transitionValue,
            state,
            this.previousConditions,
            this.nextConditions
        );
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousStateValue);
    }

    /// <inheritdoc />
    public IParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext> If(Func<TNext, bool> condition)
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext> If(
        Func<TNext, CancellationToken, Task<bool>> condition
    )
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext> If(
        Func<TNext, CancellationToken, ValueTask<bool>> condition
    )
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {DisplayString}";
    }
}
