using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Mapping;
using ZCrew.StateCraft.States;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
internal class InitialTransitionConfiguration<TState, TTransition, T1, T2>
    : IInitialTransitionConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
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
    public IDirectTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithParameters<TNext1, TNext2>()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithParameters<
        TNext1,
        TNext2,
        TNext3
    >()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousStateConfiguration,
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >()
    {
        return new PartialDirectTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
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
    public IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, CancellationToken, Task<TNext>> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunction<T1, T2, TNext>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, CancellationToken, ValueTask<TNext>> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunction<T1, T2, TNext>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T1, T2, (TNext1, TNext2)> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple2<T1, T2, TNext1, TNext2>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T1, T2, CancellationToken, Task<(TNext1, TNext2)>> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple2<T1, T2, TNext1, TNext2>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T1, T2, CancellationToken, ValueTask<(TNext1, TNext2)>> map
    )
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple2<T1, T2, TNext1, TNext2>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, (TNext1, TNext2, TNext3)> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple3<T1, T2, TNext1, TNext2, TNext3>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, CancellationToken, Task<(TNext1, TNext2, TNext3)>> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple3<T1, T2, TNext1, TNext2, TNext3>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, CancellationToken, ValueTask<(TNext1, TNext2, TNext3)>> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple3<T1, T2, TNext1, TNext2, TNext3>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T1, T2, (TNext1, TNext2, TNext3, TNext4)> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple4<T1, T2, TNext1, TNext2, TNext3, TNext4>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T1, T2, CancellationToken, Task<(TNext1, TNext2, TNext3, TNext4)>> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple4<T1, T2, TNext1, TNext2, TNext3, TNext4>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T1, T2, CancellationToken, ValueTask<(TNext1, TNext2, TNext3, TNext4)>> map)
    {
        return new PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousStateConfiguration,
            this.transitionValue,
            new MappingFunctionValueTuple4<T1, T2, TNext1, TNext2, TNext3, TNext4>(map.AsAsyncFunc())
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.transitionValue}({this.previousStateConfiguration}) → ?";
    }
}
