using OrderProcessor.Application.Context.Contracts;
using OrderProcessor.Domain.Contracts.Data;
using OrderProcessor.Domain.Models;
using ZCrew.StateCraft;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace OrderProcessor.Application.Context;

/// <summary>
///     Manages the state machine for a single order. Demonstrates parameterized states, guard
///     conditions, state actions, and triggers that coordinate with child <see cref="LineContext"/>s.
/// </summary>
internal class OrderContext : IOrderContext
{
    private readonly Order order;
    private readonly IOrderDataService dataService;
    private readonly List<LineContext> lineContexts;
    private readonly IStateMachine<OrderState, OrderTransition> stateMachine;

    public OrderContext(Order order, IOrderDataService dataService)
    {
        this.order = order;
        this.dataService = dataService;

        this.lineContexts = order
            .Lines.Select(line => new LineContext(line, token => dataService.SaveOrder(order, token)))
            .ToList();
        // csharpier-ignore-start
        this.stateMachine = StateMachine
            .Configure<OrderState, OrderTransition>()
            .WithInitialState(OrderState.Queued)

            // Lifecycle callback: persists the order state to the database after every transition
            .OnStateChange(PersistOrderState)

            // Queued: order hasn't been started. Can be opened or immediately canceled
            .WithState(OrderState.Queued, state => state
                .WithTransition(OrderTransition.Open, OrderState.Processing)
                .WithTransition<string?>(OrderTransition.Cancel, OrderState.Canceling)
                // Guard: only allows this when every line is canceled
                .WithTransition(OrderTransition.Close, t => t
                    .If(AllLinesClosedWithAnyCanceled)
                    .To(OrderState.Canceled)))

            // Processing: guard conditions control when certain transitions are allowed
            .WithState(OrderState.Processing, state => state
                .WithTransition<string?>(OrderTransition.Suspend, OrderState.Suspending)
                // Guard: only allows this transition when all lines are suspended or closed
                .WithTransition(OrderTransition.Suspended, t => t
                    .If(AllLinesSuspendedOrClosed)
                    .To(OrderState.Suspended))
                // Guard: only allows this when every line is completed or canceled and at least one is canceled
                .WithTransition(OrderTransition.Close, t => t
                    .If(AllLinesClosedWithAnyCanceled)
                    .To(OrderState.Canceled))
                // Guard: only allows completion when every line is completed
                .WithTransition(OrderTransition.Close, t => t
                    .If(AllLinesCompleted)
                    .To(OrderState.Completed)))

            // Suspending: parameterized state that accepts a suspension reason
            .WithState(OrderState.Suspending, state => state
                .WithParameter<string?>()
                // State action: cascades the suspend command to all incomplete lines
                .WithAction(action => action.Invoke(SuspendLines))
                .WithTransition(OrderTransition.Suspended, OrderState.Suspended))

            // Suspended: can resume or cancel
            .WithState(OrderState.Suspended, state => state
                .WithTransition(OrderTransition.Resume, OrderState.Processing)
                .WithTransition<string?>(OrderTransition.Cancel, OrderState.Canceling))

            // Canceling: parameterized state that accepts a cancellation reason
            .WithState(OrderState.Canceling, state => state
                .WithParameter<string?>()
                // State action: cascades the cancel command to all non-closed lines
                .WithAction(action => action.Invoke(CancelLines))
                .WithTransition(OrderTransition.Close, t => t
                    .WithNoParameters()
                    .If(AllLinesClosedWithAnyCanceled)
                    .To(OrderState.Canceled)))

            // Terminal states: no outbound transitions
            .WithState(OrderState.Canceled, state => state)
            .WithState(OrderState.Completed, state => state)

            // Repeating trigger: fires each time any line enters Suspended, then attempts
            // the order-level Suspended transition (which is guarded, so it may be a no-op)
            .WithTrigger(trigger => trigger
                .Repeat()
                .Await(AnyLineSuspended)
                .ThenInvoke(OnLineSuspended))

            // One-shot trigger: fires once all lines reach a terminal state, then closes
            // the order and deactivates the state machine. This will either complete or
            // cancel the order
            .WithTrigger(trigger => trigger
                .Once()
                .Await(AllLinesClosed)
                .ThenInvoke(OnAllLinesClosed))
            .Build();
        // csharpier-ignore-end
    }

    public IEnumerable<ILineContext> LineContexts => this.lineContexts;

    public async Task ActivateAsync(CancellationToken token)
    {
        foreach (var lc in this.lineContexts)
        {
            await lc.Activate(token);
        }

        await this.stateMachine.Activate(token);
    }

    public Task Open(CancellationToken token)
    {
        return this.stateMachine.Transition(OrderTransition.Open, token);
    }

    public Task Suspend(string reason, CancellationToken token)
    {
        return this.stateMachine.Transition(OrderTransition.Suspend, reason, token);
    }

    public async Task Resume(CancellationToken token)
    {
        this.order.SuspensionReason = null;

        // This could be done as an on-entry action on Processing or with a Resuming state
        // Alternatively, this could use TryTransition to avoid checking the line state too
        foreach (var lc in this.lineContexts.Where(l => l.Line.State == LineState.Suspended))
        {
            await lc.Resume(token);
        }

        await this.stateMachine.Transition(OrderTransition.Resume, token);
    }

    public Task Cancel(string reason, CancellationToken token)
    {
        return this.stateMachine.Transition(OrderTransition.Cancel, reason, token);
    }

    private async Task PersistOrderState(
        OrderState from,
        OrderTransition transition,
        OrderState to,
        CancellationToken token
    )
    {
        this.order.State = to;
        await this.dataService.SaveOrder(this.order, token);
    }

    private async Task SuspendLines(string? reason, CancellationToken token)
    {
        this.order.SuspensionReason = reason;
        foreach (var lc in this.lineContexts.Where(l => l.Line.State == LineState.Incomplete))
        {
            await lc.Suspend(reason ?? "Order suspended", token);
        }
    }

    private async Task CancelLines(string? reason, CancellationToken token)
    {
        this.order.CancellationReason = reason;
        foreach (var lc in this.lineContexts.Where(l => l.Line.State is LineState.Suspended or LineState.Incomplete))
        {
            await lc.Cancel(reason ?? "Order canceled", token);
        }
    }

    private Task AnyLineSuspended(CancellationToken token)
    {
        return Task.WhenAny(this.lineContexts.Select(lc => lc.LineSuspended.WaitAsync(token)));
    }

    private async Task OnLineSuspended(IStateMachine<OrderState, OrderTransition> sm, CancellationToken token)
    {
        await sm.TryTransition(OrderTransition.Suspended, token);
    }

    private Task AllLinesClosed(CancellationToken token)
    {
        return Task.WhenAll(this.lineContexts.Select(lc => lc.LineClosed.WaitAsync(token)));
    }

    private async Task OnAllLinesClosed(IStateMachine<OrderState, OrderTransition> sm, CancellationToken token)
    {
        await sm.Transition(OrderTransition.Close, token);
        await sm.Deactivate(token);
    }

    private bool AllLinesSuspendedOrClosed()
    {
        return this.lineContexts.All(lc =>
                lc.Line.State is LineState.Suspended or LineState.Completed or LineState.Canceled
            ) && this.lineContexts.Any(lc => lc.Line.State == LineState.Suspended);
    }

    private bool AllLinesCompleted()
    {
        return this.lineContexts.All(lc => lc.Line.State == LineState.Completed);
    }

    private bool AllLinesClosedWithAnyCanceled()
    {
        return this.lineContexts.All(lc => lc.Line.State is LineState.Completed or LineState.Canceled)
            && this.lineContexts.Any(lc => lc.Line.State == LineState.Canceled);
    }

    private enum OrderTransition
    {
        Open,
        Suspend,
        Suspended,
        Resume,
        Cancel,
        Close,
    }
}
