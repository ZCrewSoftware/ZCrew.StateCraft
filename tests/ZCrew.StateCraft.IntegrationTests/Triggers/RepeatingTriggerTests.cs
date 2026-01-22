using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Triggers;

public class RepeatingTriggerTests
{
    [Fact]
    public async Task Repeat_WhenSignalCompletesMultipleTimes_ShouldExecuteTriggerEachTime()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var semaphore = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Repeat().Await(token => semaphore.WaitAsync(token)).ThenInvoke(trigger))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act - Release semaphore 3 times
        semaphore.Release();
        await Task.Delay(50, TestContext.Current.CancellationToken);
        semaphore.Release();
        await Task.Delay(50, TestContext.Current.CancellationToken);
        semaphore.Release();
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert
        trigger.Received(3).Invoke();
    }

    [Fact]
    public async Task Repeat_WhenDeactivated_ShouldStopRepeating()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var semaphore = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Repeat().Await(token => semaphore.WaitAsync(token)).ThenInvoke(trigger))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Trigger a few times
        semaphore.Release();
        await Task.Delay(50, TestContext.Current.CancellationToken);
        semaphore.Release();
        await Task.Delay(50, TestContext.Current.CancellationToken);
        var countBeforeDeactivate = trigger.ReceivedCalls().Count();

        // Act - Deactivate
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Try to signal more
        semaphore.Release();
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert - Count should not increase after deactivation
        Assert.Equal(countBeforeDeactivate, trigger.ReceivedCalls().Count());
    }

    [Fact]
    public async Task Repeat_WithStateMachineAccess_ShouldTransitionStateMachine()
    {
        // Arrange
        var semaphore = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("Next", "B"))
            .WithState("B", state => state.WithTransition("Next", "C"))
            .WithState("C", state => state)
            .WithTrigger(t =>
                t.Repeat()
                    .Await(token => semaphore.WaitAsync(token))
                    .ThenInvoke((sm, token) => sm.Transition("Next", token))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        Assert.Equal("A", stateMachine.CurrentState?.StateValue);

        // Act - First transition
        semaphore.Release();
        await Task.Delay(50, TestContext.Current.CancellationToken);
        Assert.Equal("B", stateMachine.CurrentState?.StateValue);

        // Second transition
        semaphore.Release();
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("C", stateMachine.CurrentState?.StateValue);
    }
}
