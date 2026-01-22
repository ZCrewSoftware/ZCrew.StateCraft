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
    private string DisplayString => $"{TransitionValue}({PreviousStateValue}) → ?<{typeof(TNext).FriendlyName}>";

    private readonly IReadOnlyList<IAsyncFunc<bool>> previousConditions;
    private readonly List<IAsyncFunc<TNext, bool>> nextConditions = [];

    public ParameterizedTransitionConfiguration(
        TState previousState,
        TTransition transition,
        IReadOnlyList<IAsyncFunc<bool>> previousConditions
    )
    {
        PreviousStateValue = previousState;
        TransitionValue = transition;
        this.previousConditions = previousConditions;
    }

    /// <inheritdoc/>
    public TState PreviousStateValue { get; }

    /// <inheritdoc/>
    public TTransition TransitionValue { get; }

    /// <inheritdoc/>
    public TState? NextStateValue { get; } = default;

    /// <inheritdoc/>
    public IReadOnlyList<Type> PreviousStateTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc/>
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc/>
    public bool IsConditional => this.previousConditions.Count > 0 || this.nextConditions.Count > 0;

    /// <inheritdoc />
    public IFinalTransitionConfiguration<TState, TTransition> To(TState state)
    {
        return new FinalParameterizedTransitionConfiguration<TState, TTransition, TNext>(
            PreviousStateValue,
            TransitionValue,
            state,
            this.previousConditions,
            this.nextConditions
        );
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
        $"{TransitionValue}({PreviousStateValue}<{typeof(TPrevious).FriendlyName}>) → ?<{typeof(TNext).FriendlyName}>";

    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions;
    private readonly List<IAsyncFunc<TNext, bool>> nextConditions = [];

    public ParameterizedTransitionConfiguration(
        TState previousState,
        TTransition transition,
        IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions
    )
    {
        PreviousStateValue = previousState;
        TransitionValue = transition;
        this.previousConditions = previousConditions;
    }

    /// <inheritdoc/>
    public TState PreviousStateValue { get; }

    /// <inheritdoc/>
    public TTransition TransitionValue { get; }

    /// <inheritdoc/>
    public TState? NextStateValue { get; } = default;

    /// <inheritdoc/>
    public IReadOnlyList<Type> PreviousStateTypeParameters { get; } = [typeof(TPrevious)];

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc/>
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc/>
    public bool IsConditional => this.previousConditions.Count > 0 || this.nextConditions.Count > 0;

    /// <inheritdoc />
    public IFinalTransitionConfiguration<TState, TTransition, TPrevious> To(TState state)
    {
        return new FinalParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext>(
            PreviousStateValue,
            TransitionValue,
            state,
            this.previousConditions,
            this.nextConditions
        );
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
