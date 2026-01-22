using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Triggers;

public class RunOnceTriggerTests
{
    [Fact]
    public async Task Once_WhenSignalCompletes_ShouldExecuteTrigger()
    {
        // Arrange
        var signal = Substitute.For<Action>();
        var trigger = Substitute.For<Action>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Once().Await(signal).ThenInvoke(trigger))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert
        signal.Received(1).Invoke();
        trigger.Received(1).Invoke();
    }

    [Fact]
    public async Task Once_WhenSignalCompletesMultipleTimes_ShouldExecuteTriggerOnlyOnce()
    {
        // Arrange
        var trigger = Substitute.For<Action>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(trigger))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert - Once() trigger only fires once even after signal completes
        trigger.Received(1).Invoke();
    }

    [Fact]
    public async Task Once_WhenDeactivatedBeforeSignal_ShouldNotExecuteTrigger()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var signalGate = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Once().Await(token => signalGate.Task.WaitAsync(token)).ThenInvoke(trigger))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act - Deactivate before signal completes
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);
        signalGate.SetResult();
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert
        trigger.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Once_WhenReactivated_ShouldResetAndExecuteAgain()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var signalGate = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Once().Await(token => signalGate.WaitAsync(token)).ThenInvoke(trigger))
            .Build();

        // First activation cycle
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        signalGate.Release();
        await Task.Delay(50, TestContext.Current.CancellationToken);
        trigger.Received(1).Invoke();

        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        signalGate.Release();
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert
        trigger.Received(2).Invoke();
    }

    [Fact]
    public async Task Once_WithStateMachineAccess_ShouldTransitionStateMachine()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("ToB", "B"))
            .WithState("B", state => state)
            .WithTrigger(t => t.Once().Await(() => { }).ThenInvoke((sm, token) => sm.Transition("ToB", token)))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await Task.Delay(50, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("B", stateMachine.CurrentState?.StateValue);
    }
}
