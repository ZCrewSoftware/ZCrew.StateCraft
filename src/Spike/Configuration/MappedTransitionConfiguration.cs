using Spike.Contracts;
using Spike.Contracts.Configuration;

namespace Spike.Configuration;

public class MappedTransitionConfiguration<TState, TTransition, T>
    : IParameterizedTransitionConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TransitionNextStateConfigurationBuilder<TState, T> nextStateBuilder = new();
    private readonly ITransitionPreviousStateConfiguration<TState> previousState;
    private readonly TTransition transitionValue;
    private readonly IMappingFunction mappingFunction;

    public MappedTransitionConfiguration(
        ITransitionPreviousStateConfiguration<TState> previousState,
        TTransition transitionValue,
        IMappingFunction mappingFunction
    )
    {
        this.previousState = previousState;
        this.transitionValue = transitionValue;
        this.mappingFunction = mappingFunction;
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, T> If(Func<T, bool> condition)
    {
        this.nextStateBuilder.Add(condition);
        return this;
    }

    public IFinalTransitionConfiguration<TState, TTransition> To(TState state)
    {
        var nextState = this.nextStateBuilder.WithState(state);
        return new FinalMappedTransitionConfiguration<TState, TTransition>(
            this.previousState,
            nextState,
            this.transitionValue,
            this.mappingFunction
        );
    }

    public IFinalTransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousState.StateValue);
    }
}

public class MappedTransitionConfiguration<TState, TTransition, T1, T2>
    : IParameterizedTransitionConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TransitionNextStateConfigurationBuilder<TState, T1, T2> nextStateBuilder = new();
    private readonly ITransitionPreviousStateConfiguration<TState> previousState;
    private readonly TTransition transitionValue;
    private readonly IMappingFunction mappingFunction;

    public MappedTransitionConfiguration(
        ITransitionPreviousStateConfiguration<TState> previousState,
        TTransition transitionValue,
        IMappingFunction mappingFunction
    )
    {
        this.previousState = previousState;
        this.transitionValue = transitionValue;
        this.mappingFunction = mappingFunction;
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, T1, T2> If(Func<T1, T2, bool> condition)
    {
        this.nextStateBuilder.Add(condition);
        return this;
    }

    public IFinalTransitionConfiguration<TState, TTransition> To(TState state)
    {
        var nextState = this.nextStateBuilder.WithState(state);
        return new FinalMappedTransitionConfiguration<TState, TTransition>(
            this.previousState,
            nextState,
            this.transitionValue,
            this.mappingFunction
        );
    }

    public IFinalTransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousState.StateValue);
    }
}

public class MappedTransitionConfiguration<TState, TTransition, T1, T2, T3>
    : IParameterizedTransitionConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TransitionNextStateConfigurationBuilder<TState, T1, T2, T3> nextStateBuilder = new();
    private readonly ITransitionPreviousStateConfiguration<TState> previousState;
    private readonly TTransition transitionValue;
    private readonly IMappingFunction mappingFunction;

    public MappedTransitionConfiguration(
        ITransitionPreviousStateConfiguration<TState> previousState,
        TTransition transitionValue,
        IMappingFunction mappingFunction
    )
    {
        this.previousState = previousState;
        this.transitionValue = transitionValue;
        this.mappingFunction = mappingFunction;
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, T1, T2, T3> If(Func<T1, T2, T3, bool> condition)
    {
        this.nextStateBuilder.Add(condition);
        return this;
    }

    public IFinalTransitionConfiguration<TState, TTransition> To(TState state)
    {
        var nextState = this.nextStateBuilder.WithState(state);
        return new FinalMappedTransitionConfiguration<TState, TTransition>(
            this.previousState,
            nextState,
            this.transitionValue,
            this.mappingFunction
        );
    }

    public IFinalTransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousState.StateValue);
    }
}

public class MappedTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>
    : IParameterizedTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TransitionNextStateConfigurationBuilder<TState, T1, T2, T3, T4> nextStateBuilder = new();
    private readonly ITransitionPreviousStateConfiguration<TState> previousState;
    private readonly TTransition transitionValue;
    private readonly IMappingFunction mappingFunction;

    public MappedTransitionConfiguration(
        ITransitionPreviousStateConfiguration<TState> previousState,
        TTransition transitionValue,
        IMappingFunction mappingFunction
    )
    {
        this.previousState = previousState;
        this.transitionValue = transitionValue;
        this.mappingFunction = mappingFunction;
    }

    public IParameterizedTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> If(
        Func<T1, T2, T3, T4, bool> condition
    )
    {
        this.nextStateBuilder.Add(condition);
        return this;
    }

    public IFinalTransitionConfiguration<TState, TTransition> To(TState state)
    {
        var nextState = this.nextStateBuilder.WithState(state);
        return new FinalMappedTransitionConfiguration<TState, TTransition>(
            this.previousState,
            nextState,
            this.transitionValue,
            this.mappingFunction
        );
    }

    public IFinalTransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousState.StateValue);
    }
}
