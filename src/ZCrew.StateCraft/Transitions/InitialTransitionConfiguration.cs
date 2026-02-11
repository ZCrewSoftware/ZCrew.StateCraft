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
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2> WithMappedParameters<TN1, TN2>(
        Func<TPrevious, (TN1, TN2)> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple2<TPrevious, TN1, TN2>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithMappedParameters<TN1, TN2, TN3>(
        Func<TPrevious, (TN1, TN2, TN3)> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple3<TPrevious, TN1, TN2, TN3>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithMappedParameters<
        TN1,
        TN2,
        TN3,
        TN4
    >(Func<TPrevious, (TN1, TN2, TN3, TN4)> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple4<TPrevious, TN1, TN2, TN3, TN4>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {DisplayString}";
    }
}

/// <inheritdoc />
[DebuggerDisplay("{DisplayString}")]
internal class InitialTransitionConfiguration<TState, TTransition, T1, T2>
    : IInitialTransitionConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString =>
        $"{this.transitionValue}({this.previousStateConfiguration.StateValue}<{typeof(T1).FriendlyName}, {typeof(T2).FriendlyName}>) → ?";

    private readonly IPartialPreviousStateConfiguration<TState, TTransition, T1, T2> previousStateConfiguration;
    private readonly TTransition transitionValue;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="InitialTransitionConfiguration{TState, TTransition, T1, T2}"/> class.
    /// </summary>
    /// <param name="previousState">The previous state value for this transition.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.previousStateConfiguration = new PreviousStateConfiguration<TState, TTransition, T1, T2>(previousState);
        this.transitionValue = transition;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T1, T2> If(Func<T1, T2, bool> condition)
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T1, T2> If(
        Func<T1, T2, CancellationToken, Task<bool>> condition
    )
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T1, T2> If(
        Func<T1, T2, CancellationToken, ValueTask<bool>> condition
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
    public IDirectTransitionConfiguration<TState, TTransition, TN1, TN2> WithParameters<TN1, TN2>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TN1, TN2>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithParameters<TN1, TN2, TN3>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithParameters<TN1, TN2, TN3, TN4>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, TNext> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunction<T1, T2, TNext>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2> WithMappedParameters<TN1, TN2>(
        Func<T1, T2, (TN1, TN2)> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple2<T1, T2, TN1, TN2>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithMappedParameters<TN1, TN2, TN3>(
        Func<T1, T2, (TN1, TN2, TN3)> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple3<T1, T2, TN1, TN2, TN3>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithMappedParameters<
        TN1,
        TN2,
        TN3,
        TN4
    >(Func<T1, T2, (TN1, TN2, TN3, TN4)> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple4<T1, T2, TN1, TN2, TN3, TN4>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {DisplayString}";
    }
}

/// <inheritdoc />
[DebuggerDisplay("{DisplayString}")]
internal class InitialTransitionConfiguration<TState, TTransition, T1, T2, T3>
    : IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString =>
        $"{this.transitionValue}({this.previousStateConfiguration.StateValue}<{typeof(T1).FriendlyName}, {typeof(T2).FriendlyName}, {typeof(T3).FriendlyName}>) → ?";

    private readonly IPartialPreviousStateConfiguration<TState, TTransition, T1, T2, T3> previousStateConfiguration;
    private readonly TTransition transitionValue;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="InitialTransitionConfiguration{TState, TTransition, T1, T2, T3}"/> class.
    /// </summary>
    /// <param name="previousState">The previous state value for this transition.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.previousStateConfiguration = new PreviousStateConfiguration<TState, TTransition, T1, T2, T3>(
            previousState
        );
        this.transitionValue = transition;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3> If(Func<T1, T2, T3, bool> condition)
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3> If(
        Func<T1, T2, T3, CancellationToken, Task<bool>> condition
    )
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3> If(
        Func<T1, T2, T3, CancellationToken, ValueTask<bool>> condition
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
    public IDirectTransitionConfiguration<TState, TTransition, TN1, TN2> WithParameters<TN1, TN2>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TN1, TN2>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithParameters<TN1, TN2, TN3>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithParameters<TN1, TN2, TN3, TN4>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, T3, TNext> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunction<T1, T2, T3, TNext>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2> WithMappedParameters<TN1, TN2>(
        Func<T1, T2, T3, (TN1, TN2)> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple2<T1, T2, T3, TN1, TN2>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithMappedParameters<TN1, TN2, TN3>(
        Func<T1, T2, T3, (TN1, TN2, TN3)> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple3<T1, T2, T3, TN1, TN2, TN3>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithMappedParameters<
        TN1,
        TN2,
        TN3,
        TN4
    >(Func<T1, T2, T3, (TN1, TN2, TN3, TN4)> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple4<T1, T2, T3, TN1, TN2, TN3, TN4>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {DisplayString}";
    }
}

/// <inheritdoc />
[DebuggerDisplay("{DisplayString}")]
internal class InitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>
    : IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString =>
        $"{this.transitionValue}({this.previousStateConfiguration.StateValue}<{typeof(T1).FriendlyName}, {typeof(T2).FriendlyName}, {typeof(T3).FriendlyName}, {typeof(T4).FriendlyName}>) → ?";

    private readonly IPartialPreviousStateConfiguration<TState, TTransition, T1, T2, T3, T4> previousStateConfiguration;
    private readonly TTransition transitionValue;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="InitialTransitionConfiguration{TState, TTransition, T1, T2, T3, T4}"/> class.
    /// </summary>
    /// <param name="previousState">The previous state value for this transition.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.previousStateConfiguration = new PreviousStateConfiguration<TState, TTransition, T1, T2, T3, T4>(
            previousState
        );
        this.transitionValue = transition;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> If(Func<T1, T2, T3, T4, bool> condition)
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> If(
        Func<T1, T2, T3, T4, CancellationToken, Task<bool>> condition
    )
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> If(
        Func<T1, T2, T3, T4, CancellationToken, ValueTask<bool>> condition
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
    public IDirectTransitionConfiguration<TState, TTransition, TN1, TN2> WithParameters<TN1, TN2>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TN1, TN2>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithParameters<TN1, TN2, TN3>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithParameters<TN1, TN2, TN3, TN4>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, T3, T4, TNext> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunction<T1, T2, T3, T4, TNext>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2> WithMappedParameters<TN1, TN2>(
        Func<T1, T2, T3, T4, (TN1, TN2)> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple2<T1, T2, T3, T4, TN1, TN2>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithMappedParameters<TN1, TN2, TN3>(
        Func<T1, T2, T3, T4, (TN1, TN2, TN3)> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple3<T1, T2, T3, T4, TN1, TN2, TN3>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithMappedParameters<
        TN1,
        TN2,
        TN3,
        TN4
    >(Func<T1, T2, T3, T4, (TN1, TN2, TN3, TN4)> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple4<T1, T2, T3, T4, TN1, TN2, TN3, TN4>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {DisplayString}";
    }
}
