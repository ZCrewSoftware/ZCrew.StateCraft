using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class DisposeLifecycleTests
{
    [Fact(Skip = "BL-F05: Dispose() does not call Deactivate() — OnExit handlers are skipped")]
    public async Task Dispose_WhenActivated_ShouldInvokeOnExitHandlers()
    {
        // Arrange
        var exitCalled = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(OnExit))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        stateMachine.Dispose();

        // Assert
        Assert.True(exitCalled);

        return;

        Task OnExit(CancellationToken _)
        {
            exitCalled = true;
            return Task.CompletedTask;
        }
    }

    [Fact(Skip = "BL-F05: Dispose() does not call Deactivate() — triggers are not deactivated")]
    public async Task Dispose_WhenActivated_ShouldDeactivateTriggers()
    {
        // Arrange
        var signalGate = new TaskCompletionSource();
        var triggerFired = false;

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .WithTrigger(t =>
                t.Once()
                    .Await(token => signalGate.Task.WaitAsync(token))
                    .ThenInvoke(TriggerAction)
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        stateMachine.Dispose();
        signalGate.SetResult();

        // Allow any pending tasks to complete
        await Task.Delay(100, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(triggerFired);

        return;

        Task TriggerAction(IStateMachine<string, string> _, CancellationToken __)
        {
            triggerFired = true;
            return Task.CompletedTask;
        }
    }
}
