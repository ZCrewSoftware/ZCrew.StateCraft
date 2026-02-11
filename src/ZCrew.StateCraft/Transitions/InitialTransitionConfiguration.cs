using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Mapping;
using ZCrew.StateCraft.States;
using ZCrew.StateCraft.States.Configuration;

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
    private string DisplayString => $"{this.transitionValue}({this.previousStateConfiguration.StateValue}) → ?";

    private readonly IPartialPreviousStateConfiguration<TState, TTransition> previousStateConfiguration;
    private readonly TTransition transitionValue;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="InitialTransitionConfiguration{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="previousState">The previous state value for this transition.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.previousStateConfiguration = new PreviousStateConfiguration<TState, TTransition>(previousState);
        this.transitionValue = transition;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition> If(Func<bool> condition)
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition> If(Func<CancellationToken, Task<bool>> condition)
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition> If(Func<CancellationToken, ValueTask<bool>> condition)
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition> WithNoParameters()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, T1, T2> WithParameters<T1, T2>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, T1, T2>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, T1, T2, T3> WithParameters<T1, T2, T3>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, T1, T2, T3>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> WithParameters<T1, T2, T3, T4>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>(
            this.previousStateConfiguration,
            this.transitionValue
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
    private string DisplayString =>
        $"{this.transitionValue}({this.previousStateConfiguration.StateValue}<{typeof(TPrevious).FriendlyName}>) → ?";

    private readonly IPartialPreviousStateConfiguration<TState, TTransition, TPrevious> previousStateConfiguration;
    private readonly TTransition transitionValue;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="InitialTransitionConfiguration{TState, TTransition, TPrevious}"/> class.
    /// </summary>
    /// <param name="previousState">The previous state value for this transition.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.previousStateConfiguration = new PreviousStateConfiguration<TState, TTransition, TPrevious>(previousState);
        this.transitionValue = transition;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, TPrevious> If(Func<TPrevious, bool> condition)
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, TPrevious> If(
        Func<TPrevious, CancellationToken, Task<bool>> condition
    )
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, TPrevious> If(
        Func<TPrevious, CancellationToken, ValueTask<bool>> condition
    )
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition> WithNoParameters()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, T1, T2> WithParameters<T1, T2>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, T1, T2>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, T1, T2, T3> WithParameters<T1, T2, T3>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, T1, T2, T3>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> WithParameters<T1, T2, T3, T4>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<TPrevious, TNext> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunction<TPrevious, TNext>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {DisplayString}";
    }
}
