using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Mapping.Contracts;
using ZCrew.StateCraft.States;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
[DebuggerDisplay("{DisplayString}")]
internal class PartialMappedTransitionConfiguration<TState, TTransition, TNext>
    : IMappedTransitionConfiguration<TState, TTransition, TNext>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString =>
        $"{this.transitionValue}({this.previousStateConfiguration.StateValue}) → ?<{typeof(TNext).FriendlyName}>";

    private readonly IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration;
    private readonly IPartialNextStateConfiguration<TState, TTransition, TNext> nextStateConfiguration;
    private readonly TTransition transitionValue;
    private readonly IMappingFunction mappingFunction;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="PartialMappedTransitionConfiguration{TState, TTransition, TNext}"/> class.
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
        this.nextStateConfiguration = new PartialNextStateConfiguration<TState, TTransition, TNext>();
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
            this.mappingFunction
        );
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousStateConfiguration.StateValue);
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext> If(Func<TNext, bool> condition)
    {
        this.nextStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext> If(
        Func<TNext, CancellationToken, Task<bool>> condition
    )
    {
        this.nextStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IMappedTransitionConfiguration<TState, TTransition, TNext> If(
        Func<TNext, CancellationToken, ValueTask<bool>> condition
    )
    {
        this.nextStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Mapped Transition: {DisplayString}";
    }
}
