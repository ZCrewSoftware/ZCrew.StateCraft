using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.IntegrationTests.Triggers;

public class DeactivationTriggerTests
{
    [Fact(Timeout = 5000)]
    public async Task Deactivate_WhenOnExitThrows_ShouldLeaveTriggerActive()
    {
        // Arrange
        var callCount = 0;
        var signalGate = new TaskCompletionSource();
        var triggerCompleted = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .OnExit(OnExit)
                        .WithTransition("To B", t => t.To("B"))
            )
            .WithState("B", state => state)
            .WithTrigger(t =>
                t.Once()
                    .Await(token => signalGate.Task.WaitAsync(token))
                    .ThenInvoke(TriggerAction)
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => stateMachine.Deactivate(TestContext.Current.CancellationToken)
        );

        signalGate.SetResult();
        await triggerCompleted.Task;

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);

        return;

        Task OnExit(CancellationToken _)
        {
            if (Interlocked.Increment(ref callCount) == 1)
                throw new InvalidOperationException("Deactivation failure");
            return Task.CompletedTask;
        }

        async Task TriggerAction(IStateMachine<string, string> sm, CancellationToken token)
        {
            await sm.Transition("To B", token);
            triggerCompleted.TrySetResult();
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Deactivate_WhenSuccessfulAfterFailure_ShouldDeactivateTriggers()
    {
        // Arrange
        var callCount = 0;
        var signalGate = new SemaphoreSlim(0);
        var triggerFired = false;

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(OnExit))
            .WithTrigger(t =>
                t.Once()
                    .Await(token => signalGate.WaitAsync(token))
                    .ThenInvoke(TriggerAction)
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => stateMachine.Deactivate(TestContext.Current.CancellationToken)
        );

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);
        signalGate.Release();

        // Allow any pending tasks to complete
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(triggerFired);

        return;

        Task OnExit(CancellationToken _)
        {
            if (Interlocked.Increment(ref callCount) == 1)
                throw new InvalidOperationException("Deactivation failure");
            return Task.CompletedTask;
        }

        Task TriggerAction(IStateMachine<string, string> _, CancellationToken __)
        {
            triggerFired = true;
            return Task.CompletedTask;
        }
    }
}
