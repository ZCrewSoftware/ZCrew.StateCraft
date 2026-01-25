using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
[DebuggerDisplay("{ToDisplayString()}")]
internal class FinalParameterlessTransitionConfiguration<TState, TTransition>
    : IFinalTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<bool>> previousConditions;
    private readonly IReadOnlyList<IAsyncFunc<bool>> nextConditions;

    public FinalParameterlessTransitionConfiguration(
        TState previousState,
        TTransition transition,
        TState nextState,
        IReadOnlyList<IAsyncFunc<bool>> previousConditions,
        IReadOnlyList<IAsyncFunc<bool>> nextConditions
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
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [];

    /// <inheritdoc />
    public bool IsConditional => this.previousConditions.Count > 0 || this.nextConditions.Count > 0;

    /// <inheritdoc />
    public ITransition<TState, TTransition> Build(IParameterlessState<TState, TTransition> state)
    {
        var combinedConditions = this.previousConditions.Concat(this.nextConditions).ToList();
        return new ParameterlessTransition<TState, TTransition>(
            state,
            TransitionValue,
            NextStateValue,
            combinedConditions
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {ToDisplayString()}";
    }

    private string ToDisplayString()
    {
        if (PreviousStateValue.Equals(NextStateValue))
        {
            return $"{TransitionValue}({PreviousStateValue}) ↩";
        }

        return $"{TransitionValue}({PreviousStateValue}) → {NextStateValue}";
    }
}

/// <inheritdoc />
[DebuggerDisplay("{ToDisplayString()}")]
internal class FinalParameterlessTransitionConfiguration<TState, TTransition, TPrevious>
    : IFinalTransitionConfiguration<TState, TTransition, TPrevious>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions;
    private readonly IReadOnlyList<IAsyncFunc<bool>> nextConditions;

    public FinalParameterlessTransitionConfiguration(
        TState previousState,
        TTransition transition,
        TState nextState,
        IReadOnlyList<IAsyncFunc<TPrevious, bool>> previousConditions,
        IReadOnlyList<IAsyncFunc<bool>> nextConditions
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
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [];

    /// <inheritdoc />
    public bool IsConditional => this.previousConditions.Count > 0 || this.nextConditions.Count > 0;

    /// <inheritdoc />
    public ITransition<TState, TTransition> Build(IParameterizedState<TState, TTransition, TPrevious> state)
    {
        return new ParameterlessTransition<TState, TTransition, TPrevious>(
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
        return $"{TransitionValue}({PreviousStateValue}<{typeof(TPrevious).FriendlyName}>) → {NextStateValue}";
    }
}
