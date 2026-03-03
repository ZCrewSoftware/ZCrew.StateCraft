namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class DisposeRaceTests
{
    [Fact(Timeout = 5000)]
    public async Task Dispose_WhenCalledDuringAsyncAction_ShouldNotThrow()
    {
        // Arrange — action signals when it starts, then waits for cancellation.
        // Dispose() is called concurrently, racing with the action's CTS field (BL-F04).
        var actionStarted = new SemaphoreSlim(0, 1);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await actionStarted.WaitAsync(TestContext.Current.CancellationToken);

        // Act — dispose while the async action is running; must not throw
        var exception = Record.Exception(() => stateMachine.Dispose());

        // Assert
        Assert.Null(exception);

        return;

        async Task Action(CancellationToken token)
        {
            actionStarted.Release();

            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Dispose_WhenCalledConcurrentlyWithTransition_ShouldNotThrow()
    {
        // Arrange — race Dispose() against a transition that calls ExitState,
        // both competing to cancel the same actionCancellationTokenSource (BL-F04).
        var actionStarted = new SemaphoreSlim(0, 1);

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

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await actionStarted.WaitAsync(TestContext.Current.CancellationToken);

        // Act — fire Dispose() and Transition concurrently; neither should throw
        // due to double-cancel or use-after-dispose on the CTS.
        var disposeTask = Task.Run(() => stateMachine.Dispose());
        var transitionTask = Task.Run(async () =>
        {
            try
            {
                await stateMachine.Transition("To B", TestContext.Current.CancellationToken);
            }
            catch (Exception ex) when (ex is InvalidOperationException or ObjectDisposedException)
            {
                // Expected if Dispose() wins the race
            }
        });

        var exception = await Record.ExceptionAsync(() => Task.WhenAll(disposeTask, transitionTask));

        // Assert
        Assert.Null(exception);

        return;

        async Task Action(CancellationToken token)
        {
            actionStarted.Release();

            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Dispose_WhenCalledMultipleTimesConcurrently_ShouldNotThrow()
    {
        // Arrange — multiple concurrent Dispose() calls must not double-dispose the CTS.
        var actionStarted = new SemaphoreSlim(0, 1);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await actionStarted.WaitAsync(TestContext.Current.CancellationToken);

        // Act — race multiple Dispose() calls
        var tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(() => stateMachine.Dispose()));
        var exception = await Record.ExceptionAsync(() => Task.WhenAll(tasks));

        // Assert
        Assert.Null(exception);

        return;

        async Task Action(CancellationToken token)
        {
            actionStarted.Release();

            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }
    }
}
