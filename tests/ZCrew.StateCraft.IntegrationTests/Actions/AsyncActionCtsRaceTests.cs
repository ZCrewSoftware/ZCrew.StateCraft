namespace ZCrew.StateCraft.IntegrationTests.Actions;

public class AsyncActionCtsRaceTests
{
    [Fact(Timeout = 5000)]
    public async Task Transition_WhenConcurrentWithAsyncActionStart_ShouldCancelAction()
    {
        // Arrange — action signals when it starts, then waits for cancellation.
        // A concurrent transition fires as soon as the action is running, exercising
        // the window between lock release and CTS field assignment (BL-F06).
        var actionStarted = new SemaphoreSlim(0, 1);
        var actionCancelled = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithAction(a => a.Invoke(Action))
                        .WithTransition("To B", t => t.To("B"))
            )
            .WithState("B", state => state)
            .Build();

        // Start a concurrent task that will transition as soon as the action starts
        var transitionTask = Task.Run(async () =>
        {
            await actionStarted.WaitAsync(TestContext.Current.CancellationToken);
            await stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        });

        // Act — activate starts the action and releases the lock
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await transitionTask;

        // Assert — action must have been cancelled, not orphaned
        await actionCancelled.Task;

        return;

        async Task Action(CancellationToken token)
        {
            actionStarted.Release();

            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                actionCancelled.TrySetResult();
            }
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WhenConcurrentWithAsyncActionStartAfterTransition_ShouldCancelAction()
    {
        // Arrange — same race but triggered by a transition (not activation),
        // exercising the EnterState path after a state-to-state transition.
        var actionStarted = new SemaphoreSlim(0, 1);
        var actionCancelled = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState(
                "B",
                state =>
                    state
                        .WithAction(a => a.Invoke(Action))
                        .WithTransition("To A", t => t.To("A"))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Start a concurrent task that will transition back as soon as B's action starts
        var transitionTask = Task.Run(async () =>
        {
            await actionStarted.WaitAsync(TestContext.Current.CancellationToken);
            await stateMachine.Transition("To A", TestContext.Current.CancellationToken);
        });

        // Act — transition to B starts the action and releases the lock
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        await transitionTask;

        // Assert — B's action must have been cancelled, not orphaned
        await actionCancelled.Task;

        return;

        async Task Action(CancellationToken token)
        {
            actionStarted.Release();

            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                actionCancelled.TrySetResult();
            }
        }
    }
}
