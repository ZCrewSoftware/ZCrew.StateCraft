namespace Spike.Contracts.Configuration;

public interface IInitialTransitionConfiguration<TState, TTransition>
    : INextParameterTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IInitialTransitionConfiguration<TState, TTransition> If(Func<bool> condition);
}

public interface IInitialTransitionConfiguration<TState, TTransition, T>
    : INextParameterTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IInitialTransitionConfiguration<TState, TTransition, T> If(Func<T, bool> condition);

    IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(Func<T, TNext> map);
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T, (TNext1, TNext2)> map
    );
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T, (TNext1, TNext2, TNext3)> map);
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T, (TNext1, TNext2, TNext3, TNext4)> map);

    IParameterizedTransitionConfiguration<TState, TTransition, T> WithSameParameter()
    {
        return WithMappedParameter(previous => previous);
    }
}

public interface IInitialTransitionConfiguration<TState, TTransition, T1, T2>
    : INextParameterTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IInitialTransitionConfiguration<TState, TTransition, T1, T2> If(Func<T1, T2, bool> condition);

    IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, TNext> map
    );
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T1, T2, (TNext1, TNext2)> map
    );
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, (TNext1, TNext2, TNext3)> map);
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T1, T2, (TNext1, TNext2, TNext3, TNext4)> map);
    IParameterizedTransitionConfiguration<TState, TTransition, T1, T2> WithSameParameters()
    {
        return WithMappedParameters((parameter1, parameter2) => (parameter1, parameter2));
    }
}

public interface IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3>
    : INextParameterTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3> If(Func<T1, T2, T3, bool> condition);

    IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, T3, TNext> map
    );
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T1, T2, T3, (TNext1, TNext2)> map
    );
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, T3, (TNext1, TNext2, TNext3)> map);
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T1, T2, T3, (TNext1, TNext2, TNext3, TNext4)> map);
    IParameterizedTransitionConfiguration<TState, TTransition, T1, T2, T3> WithSameParameters()
    {
        return WithMappedParameters((parameter1, parameter2, parameter3) => (parameter1, parameter2, parameter3));
    }
}

public interface IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>
    : INextParameterTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> If(Func<T1, T2, T3, T4, bool> condition);

    IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, T3, T4, TNext> map
    );
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T1, T2, T3, T4, (TNext1, TNext2)> map
    );
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, T3, T4, (TNext1, TNext2, TNext3)> map);
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T1, T2, T3, T4, (TNext1, TNext2, TNext3, TNext4)> map);
    IParameterizedTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> WithSameParameters()
    {
        return WithMappedParameters(
            (parameter1, parameter2, parameter3, parameter4) => (parameter1, parameter2, parameter3, parameter4)
        );
    }
}

public interface INextParameterTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IParameterlessTransitionConfiguration<TState, TTransition> WithNoParameters();
    IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>();
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithParameters<TNext1, TNext2>();
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithParameters<
        TNext1,
        TNext2,
        TNext3
    >();
    IParameterizedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >();

    IFinalTransitionConfiguration<TState, TTransition> To(TState state)
    {
        return WithNoParameters().To(state);
    }
}
