using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Triggers.Contracts;

namespace ZCrew.StateCraft.Triggers;

/// <summary>
///     Configuration for the <see cref="RunOnceTrigger{TState,TTransition}"/>.
/// </summary>
internal class RunOnceTriggerConfiguration<TState, TTransition>
    : IScheduledTriggerConfiguration<TState, TTransition>,
        IAwaitingTriggerConfiguration<TState, TTransition>,
        IFinalTriggerConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private IAsyncAction? configuredSignal;
    private IAsyncAction<IStateMachine<TState, TTransition>>? configuredTrigger;

    /// <inheritdoc />
    public ITrigger Build(IStateMachine<TState, TTransition> stateMachine)
    {
        Debug.Assert(this.configuredSignal != null, $"{nameof(this.configuredSignal)} was not configured");
        Debug.Assert(this.configuredTrigger != null, $"{nameof(this.configuredTrigger)} was not configured");
        return new RunOnceTrigger<TState, TTransition>(stateMachine, this.configuredSignal, this.configuredTrigger);
    }

    /// <inheritdoc />
    public IAwaitingTriggerConfiguration<TState, TTransition> Await(Action signal)
    {
        this.configuredSignal = signal.AsAsyncAction();
        return this;
    }

    /// <inheritdoc />
    public IAwaitingTriggerConfiguration<TState, TTransition> Await(Func<CancellationToken, Task> signal)
    {
        this.configuredSignal = signal.AsAsyncAction();
        return this;
    }

    /// <inheritdoc />
    public IAwaitingTriggerConfiguration<TState, TTransition> Await(Func<CancellationToken, ValueTask> signal)
    {
        this.configuredSignal = signal.AsAsyncAction();
        return this;
    }

    /// <inheritdoc />
    public IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(Action trigger)
    {
        return ThenInvoke(_ => trigger());
    }

    /// <inheritdoc />
    public IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(Func<CancellationToken, Task> trigger)
    {
        return ThenInvoke((_, token) => trigger(token));
    }

    /// <inheritdoc />
    public IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(Func<CancellationToken, ValueTask> trigger)
    {
        return ThenInvoke((_, token) => trigger(token));
    }

    /// <inheritdoc />
    public IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(
        Action<IStateMachine<TState, TTransition>> trigger
    )
    {
        this.configuredTrigger = trigger.AsAsyncAction();
        return this;
    }

    /// <inheritdoc />
    public IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(
        Func<IStateMachine<TState, TTransition>, CancellationToken, Task> trigger
    )
    {
        this.configuredTrigger = trigger.AsAsyncAction();
        return this;
    }

    /// <inheritdoc />
    public IFinalTriggerConfiguration<TState, TTransition> ThenInvoke(
        Func<IStateMachine<TState, TTransition>, CancellationToken, ValueTask> trigger
    )
    {
        this.configuredTrigger = trigger.AsAsyncAction();
        return this;
    }
}
