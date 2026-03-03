namespace ZCrew.StateCraft.IntegrationTests.Actions;

public class AsyncActionRaceConditionTests
{
    [Fact(
        Skip = "BL-F06: CTS assignment race window in async action mode can orphan fire-and-forget task",
        Timeout = 5000
    )]
    public async Task WithAsynchronousActions_WhenConcurrentTransition_ShouldNotOrphanTask()
    {
        // Arrange — async action mode with rapid transitions
        var actionStarted = new SemaphoreSlim(0);
        var actionTokenCancelled = false;

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.WithAction(a => a.Invoke(Action)).WithTransition("To B", t => t.To("B"))
            )
            .WithState(
                "B",
                state => state.WithAction(a => a.Invoke(Action)).WithTransition("To A", t => t.To("A"))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await actionStarted.WaitAsync(TestContext.Current.CancellationToken);

        // Act — rapid transition should cancel previous action's token
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        await actionStarted.WaitAsync(TestContext.Current.CancellationToken);

        // Assert — previous action's token should have been cancelled, not orphaned
        Assert.True(actionTokenCancelled);

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
                actionTokenCancelled = true;
            }
        }
    }
}
