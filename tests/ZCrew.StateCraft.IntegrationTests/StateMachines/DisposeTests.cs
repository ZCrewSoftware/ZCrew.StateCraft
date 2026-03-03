namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class DisposeTests
{
    [Fact(Timeout = 5000)]
    public async Task Dispose_WhenCalledDuringAsyncAction_ShouldNotThrow()
    {
        // Arrange
        var actionStarted = new SemaphoreSlim(0, 1);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await actionStarted.WaitAsync(TestContext.Current.CancellationToken);

        // Act
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
        // Arrange
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

        // Act
        var disposeTask = Task.Run(
            () => stateMachine.Dispose(),
            TestContext.Current.CancellationToken
        );
        var transitionTask = Task.Run(
            async () =>
            {
                try
                {
                    await stateMachine.Transition(
                        "To B",
                        TestContext.Current.CancellationToken
                    );
                }
                catch (Exception ex)
                    when (ex is InvalidOperationException or ObjectDisposedException)
                {
                    // Expected if Dispose() wins the race
                }
            },
            TestContext.Current.CancellationToken
        );

        var exception = await Record.ExceptionAsync(
            () => Task.WhenAll(disposeTask, transitionTask)
        );

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
        // Arrange
        var actionStarted = new SemaphoreSlim(0, 1);
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await actionStarted.WaitAsync(TestContext.Current.CancellationToken);

        // Act
        var tasks = Enumerable
            .Range(0, 10)
            .Select(_ =>
                Task.Run(
                    async () =>
                    {
                        await gate.Task;
                        stateMachine.Dispose();
                    },
                    TestContext.Current.CancellationToken
                )
            );
        gate.SetResult();
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
