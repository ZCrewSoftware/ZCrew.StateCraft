using System.Diagnostics;
using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
/// <remarks>
///     This is meant to configure the conditions for this transition before deciding the implementation of the
///     transition using <see cref="WithNoParameters"/> or <see cref="WithParameter{TNext}"/>. There is a short-circuit
///     route where the user may want to configure a parameterless transition using
///     <see cref="IInitialTransitionConfiguration{TState,TTransition}.To"/> which avoids calling
///     <see cref="WithNoParameters"/>.
/// </remarks>
[DebuggerDisplay("{DisplayString}")]
internal class InitialTransitionConfiguration<TState, TTransition>
    : IInitialTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString => $"{TransitionValue}({PreviousStateValue}) → ?";

    private readonly List<IAsyncFunc<bool>> previousConditions = [];

    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        PreviousStateValue = previousState;
        TransitionValue = transition;
    }

    /// <inheritdoc />
    public TState PreviousStateValue { get; }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public TState? NextStateValue { get; } = default;

    /// <inheritdoc />
    public IReadOnlyList<Type> PreviousStateTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [];

    /// <inheritdoc />
    public bool IsConditional => this.previousConditions.Count > 0;

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition> If(Func<bool> condition)
    {
        this.previousConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition> If(Func<CancellationToken, Task<bool>> condition)
    {
        this.previousConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition> If(Func<CancellationToken, ValueTask<bool>> condition)
    {
        this.previousConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessTransitionConfiguration<TState, TTransition> WithNoParameters()
    {
        return new ParameterlessTransitionConfiguration<TState, TTransition>(
            PreviousStateValue,
            TransitionValue,
            this.previousConditions
        );
    }

    /// <inheritdoc />
    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext>(
            PreviousStateValue,
            TransitionValue,
            this.previousConditions
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {DisplayString}";
    }
}

/// <inheritdoc />
/// <remarks>
///     This is meant to configure the conditions for this transition before deciding the implementation of the
///     transition using <see cref="WithNoParameters"/> or <see cref="WithParameter{TNext}"/>. There is a short-circuit
///     route where the user may want to configure a parameterless transition using
///     <see cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.To"/> which avoids calling
///     <see cref="WithNoParameters"/>.
/// </remarks>
[DebuggerDisplay("{DisplayString}")]
internal class InitialTransitionConfiguration<TState, TTransition, TPrevious>
    : IInitialTransitionConfiguration<TState, TTransition, TPrevious>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString => $"{TransitionValue}({PreviousStateValue}<{typeof(TPrevious).FriendlyName}>) → ?";

    private readonly List<IAsyncFunc<TPrevious, bool>> previousConditions = [];

    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        PreviousStateValue = previousState;
        TransitionValue = transition;
    }

    /// <inheritdoc />
    public TState PreviousStateValue { get; }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public TState? NextStateValue { get; } = default;

    /// <inheritdoc />
    public IReadOnlyList<Type> PreviousStateTypeParameters { get; } = [typeof(TPrevious)];

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionTypeParameters { get; } = [];

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters { get; } = [];

    /// <inheritdoc />
    public bool IsConditional => this.previousConditions.Count > 0;

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, TPrevious> If(Func<TPrevious, bool> condition)
    {
        this.previousConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, TPrevious> If(
        Func<TPrevious, CancellationToken, Task<bool>> condition
    )
    {
        this.previousConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, TPrevious> If(
        Func<TPrevious, CancellationToken, ValueTask<bool>> condition
    )
    {
        this.previousConditions.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessTransitionConfiguration<TState, TTransition, TPrevious> WithNoParameters()
    {
        return new ParameterlessTransitionConfiguration<TState, TTransition, TPrevious>(
            PreviousStateValue,
            TransitionValue,
            this.previousConditions
        );
    }

    /// <inheritdoc />
    public IParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext> WithParameter<TNext>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext>(
            PreviousStateValue,
            TransitionValue,
            this.previousConditions
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext> WithMappedParameter<TNext>(
        Func<TPrevious, TNext> map
    )
    {
        return new MappedTransitionConfiguration<TState, TTransition, TPrevious, TNext>(
            PreviousStateValue,
            TransitionValue,
            this.previousConditions,
            map
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {DisplayString}";
    }
}
