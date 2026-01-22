using System.Diagnostics;
using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
[DebuggerDisplay("{DisplayString}")]
internal class ParameterlessTransitionConfiguration<TState, TTransition>
    : IParameterlessTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString => $"{TransitionValue}({PreviousStateValue}) → ?";

    private readonly IReadOnlyList<IAsyncFunc<bool>> previousConditions;
    private readonly List<IAsyncFunc<bool>> nextConditions = [];

    public ParameterlessTransitionConfiguration(
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
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc/>
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [];

    /// <inheritdoc/>
    public bool IsConditional => this.previousConditions.Count > 0 || this.nextConditions.Count > 0;

    /// <inheritdoc />
    public IFinalTransitionConfiguration<TState, TTransition> To(TState state)
    {
        return new FinalParameterlessTransitionConfiguration<TState, TTransition>(
            PreviousStateValue,
            TransitionValue,
            state,
            this.previousConditions,
            this.nextConditions
        );
    }

    /// <inheritdoc />
    public IParameterlessTransitionConfiguration<TState, TTransition> If(Func<bool> condition)
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessTransitionConfiguration<TState, TTransition> If(Func<CancellationToken, Task<bool>> condition)
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessTransitionConfiguration<TState, TTransition> If(
        Func<CancellationToken, ValueTask<bool>> condition
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

/// <inheritdoc cref="IParameterlessTransitionConfiguration{TState,TTransition,TPrevious}"/>
[DebuggerDisplay("{DisplayString}")]
internal class ParameterlessTransitionConfiguration<TState, TTransition, TPrevious>
    : IParameterlessTransitionConfiguration<TState, TTransition, TPrevious>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString => $"{TransitionValue}({PreviousStateValue}<{typeof(TPrevious).FriendlyName}>) → ?";

    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions;
    private readonly List<IAsyncFunc<bool>> nextConditions = [];

    public ParameterlessTransitionConfiguration(
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
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc/>
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [];

    /// <inheritdoc/>
    public bool IsConditional => this.previousConditions.Count > 0 || this.nextConditions.Count > 0;

    /// <inheritdoc />
    public IFinalTransitionConfiguration<TState, TTransition, TPrevious> To(TState state)
    {
        return new FinalParameterlessTransitionConfiguration<TState, TTransition, TPrevious>(
            PreviousStateValue,
            TransitionValue,
            state,
            this.previousConditions,
            this.nextConditions
        );
    }

    /// <inheritdoc />
    public IParameterlessTransitionConfiguration<TState, TTransition, TPrevious> If(Func<bool> condition)
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessTransitionConfiguration<TState, TTransition, TPrevious> If(
        Func<CancellationToken, Task<bool>> condition
    )
    {
        this.nextConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessTransitionConfiguration<TState, TTransition, TPrevious> If(
        Func<CancellationToken, ValueTask<bool>> condition
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
