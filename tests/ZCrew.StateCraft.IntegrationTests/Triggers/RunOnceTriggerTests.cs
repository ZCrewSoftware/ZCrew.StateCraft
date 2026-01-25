using NSubstitute;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.IntegrationTests.Triggers;

public class RunOnceTriggerTests
{
    [Fact(Timeout = 5000)]
    public async Task Once_WhenSignalCompletes_ShouldExecuteTrigger()
    {
        // Arrange
        var signal = Substitute.For<Action>();
        var trigger = Substitute.For<Action>();
        var triggerCalled = new TaskCompletionSource();
        trigger.When(x => x.Invoke()).Do(_ => triggerCalled.TrySetResult());

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Once().Await(signal).ThenInvoke(trigger))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await triggerCalled.Task;

        // Assert
        signal.Received(1).Invoke();
        trigger.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Once_WhenSignalCompletesMultipleTimes_ShouldExecuteTriggerOnlyOnce()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var triggerCalled = new TaskCompletionSource();
        trigger.When(x => x.Invoke()).Do(_ => triggerCalled.TrySetResult());

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(trigger))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await triggerCalled.Task;

        // Assert
        trigger.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
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

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);
        signalGate.SetResult();

        // Assert
        trigger.DidNotReceive().Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Once_WhenReactivated_ShouldResetAndExecuteAgain()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var triggerGate = new SemaphoreSlim(0);
        trigger.When(x => x.Invoke()).Do(_ => triggerGate.Release());

        var signalGate = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Once().Await(token => signalGate.WaitAsync(token)).ThenInvoke(trigger))
            .Build();

        var token = TestContext.Current.CancellationToken;

        // First activation cycle
        await stateMachine.Activate(token);
        signalGate.Release();
        await triggerGate.WaitAsync(token);
        trigger.Received(1).Invoke();

        await stateMachine.Deactivate(token);

        // Act
        await stateMachine.Activate(token);
        signalGate.Release();
        await triggerGate.WaitAsync(token);

        // Assert
        trigger.Received(2).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Once_WithStateMachineAccess_ShouldTransitionStateMachine()
    {
        // Arrange
        var transitionCompleted = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("ToB", "B"))
            .WithState("B", state => state)
            .WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(TriggerAction))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await transitionCompleted.Task;

        // Assert
        Assert.Equal("B", stateMachine.CurrentState?.StateValue);

        return;

        async Task TriggerAction(IStateMachine<string, string> sm, CancellationToken token)
        {
            await sm.Transition("ToB", token);
            transitionCompleted.TrySetResult();
        }
    }
}
