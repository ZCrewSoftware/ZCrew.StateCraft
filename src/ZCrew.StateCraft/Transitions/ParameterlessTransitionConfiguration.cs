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
    private string DisplayString => $"{this.transitionValue}({this.previousStateValue}) → ?";

    private readonly IReadOnlyList<IAsyncFunc<bool>> previousConditions;
    private readonly List<IAsyncFunc<bool>> nextConditions = [];
    private readonly TState previousStateValue;
    private readonly TTransition transitionValue;

    public ParameterlessTransitionConfiguration(
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
        return new FinalParameterlessTransitionConfiguration<TState, TTransition>(
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
    private string DisplayString =>
        $"{this.transitionValue}({this.previousStateValue}<{typeof(TPrevious).FriendlyName}>) → ?";

    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions;
    private readonly List<IAsyncFunc<bool>> nextConditions = [];
    private readonly TState previousStateValue;
    private readonly TTransition transitionValue;

    public ParameterlessTransitionConfiguration(
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
        return new FinalParameterlessTransitionConfiguration<TState, TTransition, TPrevious>(
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
