using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Triggers;

public class TriggerLifecycleTests
{
    [Fact(Timeout = 5000)]
    public async Task RepeatingTrigger_WhenDeactivatedAndReactivated_ShouldFireAgainFromZero()
    {
        // Arrange
        var triggerCount = 0;
        var triggerGate = new SemaphoreSlim(0);
        var signal = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t =>
                t.Repeat()
                    .Await(signal.WaitAsync)
                    .ThenInvoke(() =>
                    {
                        Interlocked.Increment(ref triggerCount);
                        triggerGate.Release();
                    })
            )
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);

        // Fire trigger twice
        signal.Release();
        await triggerGate.WaitAsync(token);
        signal.Release();
        await triggerGate.WaitAsync(token);
        Assert.Equal(2, triggerCount);

        // Deactivate resets trigger count
        await stateMachine.Deactivate(token);

        // Act — reactivate and fire again
        triggerCount = 0;
        await stateMachine.Activate(token);
        signal.Release();
        await triggerGate.WaitAsync(token);

        // Assert — fires again from zero
        Assert.Equal(1, triggerCount);
    }

    [Fact(Timeout = 5000)]
    public async Task Activate_WhenCalledTwice_ShouldThrowAndNotDuplicateTriggers()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var triggerGate = new SemaphoreSlim(0);
        trigger.When(x => x.Invoke()).Do(_ => triggerGate.Release());

        var signal = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Once().Await(signal.WaitAsync).ThenInvoke(trigger))
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);

        // Act — second activate throws, ensuring no duplicate triggers
        var activate = () => stateMachine.Activate(token);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(activate);

        // Trigger still works correctly — fires exactly once
        signal.Release();
        await triggerGate.WaitAsync(token);
        trigger.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Trigger_WhenThrowsWithContinue_ShouldAllowSubsequentTransitions()
    {
        // Arrange
        var exception = new InvalidOperationException("Trigger error");
        var handlerCalled = new TaskCompletionSource();
        var exceptionHandler = Substitute.For<Func<Exception, ExceptionResult>>();
        exceptionHandler
            .Invoke(Arg.Any<Exception>())
            .Returns(_ =>
            {
                handlerCalled.TrySetResult();
                return ExceptionResult.Continue();
            });

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnException(exceptionHandler)
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(() => throw exception))
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);
        await handlerCalled.Task;

        // Act — machine should still accept transitions after trigger exception
        await stateMachine.Transition("To B", token);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }
}
