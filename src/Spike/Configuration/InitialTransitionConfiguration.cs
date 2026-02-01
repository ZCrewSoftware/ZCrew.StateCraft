using Spike.Contracts.Configuration;

namespace Spike.Configuration;

// 0 parameters (parameterless)
public class InitialTransitionConfiguration<TState, TTransition> : IInitialTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly ITransitionParameterlessPreviousStateConfiguration<TState> previousState;
    private readonly TTransition transitionValue;

    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.transitionValue = transition;
        this.previousState = new TransitionPreviousStateConfiguration<TState>(previousState);
    }

    public IParameterlessTransitionConfiguration<TState, TTransition> WithNoParameters()
    {
        return new ParameterlessTransitionConfiguration<TState, TTransition>(this.previousState, this.transitionValue);
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithParameters<TNext1, TNext2>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithParameters<
        TNext1,
        TNext2,
        TNext3
    >()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousState,
            this.transitionValue
        );
    }

    public IInitialTransitionConfiguration<TState, TTransition> If(Func<bool> condition)
    {
        this.previousState.Add(condition);
        return this;
    }
}

// 1 parameter
public class InitialTransitionConfiguration<TState, TTransition, T>
    : IInitialTransitionConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    private readonly ITransitionPreviousStateConfiguration<TState, T> previousState;
    private readonly TTransition transitionValue;

    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.transitionValue = transition;
        this.previousState = new TransitionPreviousStateConfiguration<TState, T>(previousState);
    }

    public IParameterlessTransitionConfiguration<TState, TTransition> WithNoParameters()
    {
        return new ParameterlessTransitionConfiguration<TState, TTransition>(this.previousState, this.transitionValue);
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithParameters<TNext1, TNext2>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithParameters<
        TNext1,
        TNext2,
        TNext3
    >()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousState,
            this.transitionValue
        );
    }

    public IInitialTransitionConfiguration<TState, TTransition, T> If(Func<T, bool> condition)
    {
        this.previousState.Add(condition);
        return this;
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T, TNext> map
    )
    {
        var mappingFunction = new MappingFunction<T, TNext>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<
        TNext1,
        TNext2
    >(Func<T, (TNext1, TNext2)> map)
    {
        var mappingFunction = new MappingFunctionValueTuple2<T, TNext1, TNext2>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T, (TNext1, TNext2, TNext3)> map)
    {
        var mappingFunction = new MappingFunctionValueTuple3<T, TNext1, TNext2, TNext3>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<
        TState,
        TTransition,
        TNext1,
        TNext2,
        TNext3,
        TNext4
    > WithMappedParameters<TNext1, TNext2, TNext3, TNext4>(Func<T, (TNext1, TNext2, TNext3, TNext4)> map)
    {
        var mappingFunction = new MappingFunctionValueTuple4<T, TNext1, TNext2, TNext3, TNext4>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }
}

// 2 parameters
public class InitialTransitionConfiguration<TState, TTransition, T1, T2>
    : IInitialTransitionConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    private readonly ITransitionPreviousStateConfiguration<TState, T1, T2> previousState;
    private readonly TTransition transitionValue;

    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.transitionValue = transition;
        this.previousState = new TransitionPreviousStateConfiguration<TState, T1, T2>(previousState);
    }

    public IParameterlessTransitionConfiguration<TState, TTransition> WithNoParameters()
    {
        return new ParameterlessTransitionConfiguration<TState, TTransition>(this.previousState, this.transitionValue);
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithParameters<TNext1, TNext2>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithParameters<
        TNext1,
        TNext2,
        TNext3
    >()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousState,
            this.transitionValue
        );
    }

    public IInitialTransitionConfiguration<TState, TTransition, T1, T2> If(Func<T1, T2, bool> condition)
    {
        this.previousState.Add(condition);
        return this;
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, TNext> map
    )
    {
        var mappingFunction = new MappingFunction<T1, T2, TNext>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<
        TNext1,
        TNext2
    >(Func<T1, T2, (TNext1, TNext2)> map)
    {
        var mappingFunction = new MappingFunction2ValueTuple2<T1, T2, TNext1, TNext2>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, (TNext1, TNext2, TNext3)> map)
    {
        var mappingFunction = new MappingFunction2ValueTuple3<T1, T2, TNext1, TNext2, TNext3>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<
        TState,
        TTransition,
        TNext1,
        TNext2,
        TNext3,
        TNext4
    > WithMappedParameters<TNext1, TNext2, TNext3, TNext4>(Func<T1, T2, (TNext1, TNext2, TNext3, TNext4)> map)
    {
        var mappingFunction = new MappingFunction2ValueTuple4<T1, T2, TNext1, TNext2, TNext3, TNext4>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }
}

// 3 parameters
public class InitialTransitionConfiguration<TState, TTransition, T1, T2, T3>
    : IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    private readonly ITransitionPreviousStateConfiguration<TState, T1, T2, T3> previousState;
    private readonly TTransition transitionValue;

    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.transitionValue = transition;
        this.previousState = new TransitionPreviousStateConfiguration<TState, T1, T2, T3>(previousState);
    }

    public IParameterlessTransitionConfiguration<TState, TTransition> WithNoParameters()
    {
        return new ParameterlessTransitionConfiguration<TState, TTransition>(this.previousState, this.transitionValue);
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithParameters<TNext1, TNext2>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithParameters<
        TNext1,
        TNext2,
        TNext3
    >()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousState,
            this.transitionValue
        );
    }

    public IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3> If(Func<T1, T2, T3, bool> condition)
    {
        this.previousState.Add(condition);
        return this;
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, T3, TNext> map
    )
    {
        var mappingFunction = new MappingFunction<T1, T2, T3, TNext>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<
        TNext1,
        TNext2
    >(Func<T1, T2, T3, (TNext1, TNext2)> map)
    {
        var mappingFunction = new MappingFunction3ValueTuple2<T1, T2, T3, TNext1, TNext2>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, T3, (TNext1, TNext2, TNext3)> map)
    {
        var mappingFunction = new MappingFunction3ValueTuple3<T1, T2, T3, TNext1, TNext2, TNext3>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<
        TState,
        TTransition,
        TNext1,
        TNext2,
        TNext3,
        TNext4
    > WithMappedParameters<TNext1, TNext2, TNext3, TNext4>(Func<T1, T2, T3, (TNext1, TNext2, TNext3, TNext4)> map)
    {
        var mappingFunction = new MappingFunction3ValueTuple4<T1, T2, T3, TNext1, TNext2, TNext3, TNext4>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }
}

// 4 parameters
public class InitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>
    : IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    private readonly ITransitionPreviousStateConfiguration<TState, T1, T2, T3, T4> previousState;
    private readonly TTransition transitionValue;

    public InitialTransitionConfiguration(TState previousState, TTransition transition)
    {
        this.transitionValue = transition;
        this.previousState = new TransitionPreviousStateConfiguration<TState, T1, T2, T3, T4>(previousState);
    }

    public IParameterlessTransitionConfiguration<TState, TTransition> WithNoParameters()
    {
        return new ParameterlessTransitionConfiguration<TState, TTransition>(this.previousState, this.transitionValue);
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithParameters<TNext1, TNext2>()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithParameters<
        TNext1,
        TNext2,
        TNext3
    >()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousState,
            this.transitionValue
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >()
    {
        return new ParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousState,
            this.transitionValue
        );
    }

    public IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> If(Func<T1, T2, T3, T4, bool> condition)
    {
        this.previousState.Add(condition);
        return this;
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, T3, T4, TNext> map
    )
    {
        var mappingFunction = new MappingFunction<T1, T2, T3, T4, TNext>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<
        TNext1,
        TNext2
    >(Func<T1, T2, T3, T4, (TNext1, TNext2)> map)
    {
        var mappingFunction = new MappingFunction4ValueTuple2<T1, T2, T3, T4, TNext1, TNext2>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, T3, T4, (TNext1, TNext2, TNext3)> map)
    {
        var mappingFunction = new MappingFunction4ValueTuple3<T1, T2, T3, T4, TNext1, TNext2, TNext3>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }

    public IParameterizedTransitionConfiguration<
        TState,
        TTransition,
        TNext1,
        TNext2,
        TNext3,
        TNext4
    > WithMappedParameters<TNext1, TNext2, TNext3, TNext4>(Func<T1, T2, T3, T4, (TNext1, TNext2, TNext3, TNext4)> map)
    {
        var mappingFunction = new MappingFunction4ValueTuple4<T1, T2, T3, T4, TNext1, TNext2, TNext3, TNext4>(map);
        return new MappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>(
            this.previousState,
            this.transitionValue,
            mappingFunction
        );
    }
}
