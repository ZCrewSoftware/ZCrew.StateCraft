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
///     <see cref="IInitialTransitionConfiguration{TState,TTransition,T}.To"/> which avoids calling
///     <see cref="WithNoParameters"/>.
/// </remarks>
internal class InitialTransitionConfiguration<TState, TTransition, T>
    : IInitialTransitionConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IPartialPreviousStateConfiguration<TState, TTransition, T> previousStateConfiguration;
    private readonly TTransition transitionValue;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="InitialTransitionConfiguration{TState, TTransition, T}"/> class.
    /// </summary>
    /// <param name="previousState">The previous state value for this transition.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.previousStateConfiguration = new PreviousStateConfiguration<TState, TTransition, T>(previousState);
        this.transitionValue = transition;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T> If(Func<T, bool> condition)
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T> If(Func<T, CancellationToken, Task<bool>> condition)
    {
        this.previousStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IInitialTransitionConfiguration<TState, TTransition, T> If(
        Func<T, CancellationToken, ValueTask<bool>> condition
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
    public IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(Func<T, TNext> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunction<T, TNext>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T, CancellationToken, Task<TNext>> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunction<T, TNext>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T, CancellationToken, ValueTask<TNext>> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunction<T, TNext>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T, (TNext1, TNext2)> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple2<T, TNext1, TNext2>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T, CancellationToken, Task<(TNext1, TNext2)>> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple2<T, TNext1, TNext2>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T, CancellationToken, ValueTask<(TNext1, TNext2)>> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple2<T, TNext1, TNext2>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T, (TNext1, TNext2, TNext3)> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple3<T, TNext1, TNext2, TNext3>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T, CancellationToken, Task<(TNext1, TNext2, TNext3)>> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple3<T, TNext1, TNext2, TNext3>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T, CancellationToken, ValueTask<(TNext1, TNext2, TNext3)>> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple3<T, TNext1, TNext2, TNext3>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T, (TNext1, TNext2, TNext3, TNext4)> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple4<T, TNext1, TNext2, TNext3, TNext4>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T, CancellationToken, Task<(TNext1, TNext2, TNext3, TNext4)>> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple4<T, TNext1, TNext2, TNext3, TNext4>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T, CancellationToken, ValueTask<(TNext1, TNext2, TNext3, TNext4)>> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple4<T, TNext1, TNext2, TNext3, TNext4>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.transitionValue}({this.previousStateConfiguration}) → ?";
    }
}
