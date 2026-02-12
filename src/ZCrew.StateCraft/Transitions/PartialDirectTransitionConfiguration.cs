using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.States;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
internal class PartialDirectTransitionConfiguration<TState, TTransition>
    : IDirectTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration;
    private readonly IPartialNextStateConfiguration<TState, TTransition> nextStateConfiguration;
    private readonly TTransition transitionValue;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="PartialDirectTransitionConfiguration{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="previousStateConfiguration">The configuration for the previous state.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    public PartialDirectTransitionConfiguration(
        IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration,
        TTransition transition
    )
    {
        this.previousStateConfiguration = previousStateConfiguration;
        this.nextStateConfiguration = new PartialNextStateConfiguration<TState, TTransition>();
        this.transitionValue = transition;
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> To(TState state)
    {
        return new DirectTransitionConfiguration<TState, TTransition>(
            this.previousStateConfiguration,
            this.nextStateConfiguration.WithState(state),
            this.transitionValue
        );
    }

    /// <inheritdoc />
    public ITransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousStateConfiguration.StateValue);
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition> If(Func<bool> condition)
    {
        this.nextStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition> If(Func<CancellationToken, Task<bool>> condition)
    {
        this.nextStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public IDirectTransitionConfiguration<TState, TTransition> If(Func<CancellationToken, ValueTask<bool>> condition)
    {
        this.nextStateConfiguration.Add(condition.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.transitionValue}({this.previousStateConfiguration}) → {this.nextStateConfiguration}";
    }
}
