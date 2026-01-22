using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
[DebuggerDisplay("{ToDisplayString()}")]
internal class FinalParameterizedTransitionConfiguration<TState, TTransition, TNext>
    : IFinalTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<bool>> previousConditions;
    private readonly IReadOnlyList<IAsyncFunc<TNext, bool>> nextConditions;

    public FinalParameterizedTransitionConfiguration(
        TState previousState,
        TTransition transition,
        TState nextState,
        IReadOnlyList<IAsyncFunc<bool>> previousConditions,
        IReadOnlyList<IAsyncFunc<TNext, bool>> nextConditions
    )
    {
        PreviousStateValue = previousState;
        TransitionValue = transition;
        NextStateValue = nextState;
        this.previousConditions = previousConditions;
        this.nextConditions = nextConditions;
    }

    /// <inheritdoc />
    public TState PreviousStateValue { get; }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public TState NextStateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> PreviousStateTypeParameters { get; } = [];

    /// <inheritdoc/>
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc />
    public bool IsConditional => this.previousConditions.Count > 0 || this.nextConditions.Count > 0;

    /// <inheritdoc />
    public ITransition<TState, TTransition> Build(IParameterlessState<TState, TTransition> state)
    {
        return new ParameterizedTransition<TState, TTransition, TNext>(
            state,
            TransitionValue,
            NextStateValue,
            this.previousConditions,
            this.nextConditions
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {ToDisplayString()}";
    }

    private string ToDisplayString()
    {
        return $"{TransitionValue}({PreviousStateValue}) → {NextStateValue}<{typeof(TNext).FriendlyName}>";
    }
}

/// <inheritdoc />
[DebuggerDisplay("{ToDisplayString()}")]
internal class FinalParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext>
    : IFinalTransitionConfiguration<TState, TTransition, TPrevious>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions;
    private readonly IReadOnlyList<IAsyncFunc<TNext, bool>> nextConditions;

    public FinalParameterizedTransitionConfiguration(
        TState previousState,
        TTransition transition,
        TState nextState,
        IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions,
        IReadOnlyList<IAsyncFunc<TNext, bool>> nextConditions
    )
    {
        PreviousStateValue = previousState;
        TransitionValue = transition;
        NextStateValue = nextState;
        this.previousConditions = previousConditions;
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
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [typeof(TNext)];

    /// <inheritdoc />
    public bool IsConditional => this.previousConditions.Count > 0 || this.nextConditions.Count > 0;

    /// <inheritdoc />
    public ITransition<TState, TTransition> Build(IParameterizedState<TState, TTransition, TPrevious> state)
    {
        return new ParameterizedTransition<TState, TTransition, TPrevious, TNext>(
            state,
            TransitionValue,
            NextStateValue,
            this.previousConditions,
            this.nextConditions
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {ToDisplayString()}";
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
