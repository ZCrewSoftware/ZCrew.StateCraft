using Nito.AsyncEx;
using OrderProcessor.Application.Context.Contracts;
using OrderProcessor.Domain.Models;
using ZCrew.StateCraft;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace OrderProcessor.Application.Context;

/// <summary>
///     Manages the state machine for a single order line. Demonstrates parameterized states,
///     state actions with simulated async work, and entry callbacks that signal the parent
///     <see cref="OrderContext"/> via async events.
/// </summary>
internal class LineContext : ILineContext
{
    private readonly Line line;
    private readonly IStateMachine<LineState, LineTransition> stateMachine;
    private readonly Func<CancellationToken, ValueTask> saveOrder;

    public LineContext(Line line, Func<CancellationToken, ValueTask> saveOrder)
    {
        this.line = line;
        this.saveOrder = saveOrder;
        // csharpier-ignore-start
        this.stateMachine = StateMachine
            .Configure<LineState, LineTransition>()
            .WithInitialState(LineState.Incomplete)

            // Lifecycle callback: persists the line state to the database after every transition
            .OnStateChange(PersistLineState)

            // Incomplete: parameterized transitions carry a reason, Complete is parameterless
            .WithState(LineState.Incomplete, state => state
                .WithTransition<string?>(LineTransition.Suspend, LineState.Suspending)
                .WithTransition<string?>(LineTransition.Cancel, LineState.Canceling)
                .WithTransition(LineTransition.Complete, LineState.Completed))

            // Suspending: parameterized state with a state action that simulates async work,
            // then self-transitions to Suspended
            .WithState(LineState.Suspending, state => state
                .WithParameter<string?>()
                .WithAction(action => action.Invoke(SuspendLine))
                .WithTransition(LineTransition.Suspended, LineState.Suspended))

            // Suspended: entry callback signals the parent OrderContext
            .WithState(LineState.Suspended, state => state
                .OnEntry(() => LineSuspended.Set())
                .WithTransition(LineTransition.Resume, LineState.Incomplete)
                .WithTransition<string?>(LineTransition.Cancel, LineState.Canceling))

            // Canceling: parameterized state with a state action that simulates async work,
            // then self-transitions to Canceled
            .WithState(LineState.Canceling, state => state
                .WithParameter<string?>()
                .WithAction(action => action.Invoke(CancelLine))
                .WithTransition(LineTransition.Canceled, LineState.Canceled))

            // Terminal states: entry callbacks signal the parent OrderContext that this line is closed
            .WithState(LineState.Canceled, state => state
                .OnEntry(() => LineClosed.Set()))
            .WithState(LineState.Completed, state => state
                .OnEntry(() => LineClosed.Set()))

            .Build();
        // csharpier-ignore-end
    }

    public AsyncManualResetEvent LineClosed { get; } = new();

    public AsyncAutoResetEvent LineSuspended { get; } = new();

    public Guid LineId => this.line.Id;
    public Line Line => this.line;

    public Task Activate(CancellationToken token)
    {
        return this.stateMachine.Activate(token);
    }

    public Task Suspend(string? reason, CancellationToken token)
    {
        return this.stateMachine.Transition(LineTransition.Suspend, reason, token);
    }

    public async Task Resume(CancellationToken token)
    {
        this.line.SuspensionReason = null;
        await this.stateMachine.Transition(LineTransition.Resume, token);
    }

    public Task Cancel(string? reason, CancellationToken token)
    {
        return this.stateMachine.Transition(LineTransition.Cancel, reason, token);
    }

    public Task Complete(CancellationToken token)
    {
        return this.stateMachine.Transition(LineTransition.Complete, token);
    }

    private async Task SuspendLine(string? reason, CancellationToken token)
    {
        this.line.SuspensionReason = reason;

        // Small delay to simulate some work
        await Task.Delay(TimeSpan.FromSeconds(1), token);
        await this.stateMachine.Transition(LineTransition.Suspended, token);
    }

    private async Task CancelLine(string? reason, CancellationToken token)
    {
        this.line.CancellationReason = reason;

        // Small delay to simulate some work
        await Task.Delay(TimeSpan.FromSeconds(1), token);
        await this.stateMachine.Transition(LineTransition.Canceled, token);
    }

    private async Task PersistLineState(
        LineState from,
        LineTransition transition,
        LineState to,
        CancellationToken token
    )
    {
        this.line.State = to;
        await this.saveOrder(token);
    }

    private enum LineTransition
    {
        Suspend,
        Suspended,
        Resume,
        Cancel,
        Canceled,
        Complete,
    }
}
