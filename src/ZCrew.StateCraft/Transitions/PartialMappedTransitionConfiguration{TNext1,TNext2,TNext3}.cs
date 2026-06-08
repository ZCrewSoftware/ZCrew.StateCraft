using System.Runtime.CompilerServices;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Async.Contracts;
using ZCrew.StateCraft.Extensions;
using ZCrew.StateCraft.Mapping.Contracts;
using ZCrew.StateCraft.States;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
internal class PartialMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>
    : IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration;
    private readonly IPartialNextStateConfiguration<TState, TTransition, TNext1, TNext2, TNext3> nextStateConfiguration;
    private readonly TTransition transitionValue;
    private readonly IMappingFunction mappingFunction;
    private readonly List<INextParametersHandler> onTransitionHandlers = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="PartialMappedTransitionConfiguration{TState, TTransition, TNext1, TNext2, TNext3}"/> class.
    /// </summary>
    /// <param name="previousStateConfiguration">The configuration for the previous state.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    /// <param name="mappingFunction">The mapping function that transforms the previous parameter.</param>
    public PartialMappedTransitionConfiguration(
        IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration,
        TTransition transition,
        IMappingFunction mappingFunction
    )
    {
        this.previousStateConfiguration = previousStateConfiguration;
        this.nextStateConfiguration = new PartialNextStateConfiguration<TState, TTransition, TNext1, TNext2, TNext3>();
        this.transitionValue = transition;
        this.mappingFunction = mappingFunction;
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> To(TState state)
    {
        return new MappedTransitionConfiguration<TState, TTransition>(
            this.previousStateConfiguration,
            this.nextStateConfiguration.WithState(state),
            this.transitionValue,
            this.mappingFunction,
            this.onTransitionHandlers
        );
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousStateConfiguration.StateValue);
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> If(
        Func<TNext1, TNext2, TNext3, bool> condition,
        [CallerArgumentExpression(nameof(condition))] string? descriptor = null
    )
    {
        this.nextStateConfiguration.Add(condition.AsAsyncFunc().AsAsyncCondition(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> If(
        Func<TNext1, TNext2, TNext3, CancellationToken, Task<bool>> condition,
        [CallerArgumentExpression(nameof(condition))] string? descriptor = null
    )
    {
        this.nextStateConfiguration.Add(condition.AsAsyncFunc().AsAsyncCondition(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> If(
        Func<TNext1, TNext2, TNext3, CancellationToken, ValueTask<bool>> condition,
        [CallerArgumentExpression(nameof(condition))] string? descriptor = null
    )
    {
        this.nextStateConfiguration.Add(condition.AsAsyncFunc().AsAsyncCondition(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> OnTransition(
        Action<TNext1, TNext2, TNext3> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onTransitionHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor).AsNextParametersHandler());
        return this;
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> OnTransition(
        Func<TNext1, TNext2, TNext3, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onTransitionHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor).AsNextParametersHandler());
        return this;
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> OnTransition(
        Func<TNext1, TNext2, TNext3, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onTransitionHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor).AsNextParametersHandler());
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.transitionValue}({this.previousStateConfiguration}) → {this.nextStateConfiguration}";
    }
}
