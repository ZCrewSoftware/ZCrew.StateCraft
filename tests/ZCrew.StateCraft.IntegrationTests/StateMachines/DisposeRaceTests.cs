namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class DisposeRaceTests
{
    [Fact(Skip = "BL-F04: Dispose() does not acquire stateMachineLock, races with async action CTS write")]
    public async Task Dispose_WhenAsyncActionInFlight_ShouldCancelAction()
    {
        // Arrange
        var tokenWasCanceled = false;
        var actionStarted = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await actionStarted.Task;

        // Act
        stateMachine.Dispose();

        // Assert
        Assert.True(tokenWasCanceled);

        return;

        async Task Action(CancellationToken token)
        {
            actionStarted.SetResult();
            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                tokenWasCanceled = true;
            }
        }
    }

    [Fact]
    public async Task Dispose_WhenNoActiveAction_ShouldNotThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        var dispose = () => stateMachine.Dispose();

        // Assert
        dispose();
    }

    [Fact]
    public async Task Dispose_AfterDeactivate_ShouldNotThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(_ => Task.CompletedTask)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Act
        var dispose = () => stateMachine.Dispose();

        // Assert
        dispose();
    }
}
