using System.Diagnostics;
using NSubstitute;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.IntegrationTests.Triggers;

public class StateRunOnceTriggerTests
{
    [Fact(Timeout = 5000)]
    public async Task Once_WhenStateIsEntered_ShouldExecuteTrigger()
    {
        // Arrange
        var signal = Substitute.For<Action>();
        var trigger = Substitute.For<Action>();
        var triggerCalled = new TaskCompletionSource();
        trigger.When(x => x.Invoke()).Do(_ => triggerCalled.TrySetResult());

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTrigger(t => t.Once().Await(signal).ThenInvoke(trigger)))
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
            .WithState("A", state => state.WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(trigger)))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await triggerCalled.Task;

        // Assert
        trigger.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Once_WhenTransitionedAway_ShouldNotExecuteTrigger()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var signalGate = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTrigger(t => t.Once().Await(token => signalGate.Task.WaitAsync(token)).ThenInvoke(trigger))
                        .WithTransition("ToB", "B")
            )
            .WithState("B", state => state)
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);

        // Act
        await stateMachine.Transition("ToB", token);
        signalGate.SetResult();

        // Assert
        trigger.DidNotReceive().Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Once_WhenReenteringState_ShouldReactivateTrigger()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var triggerGate = new SemaphoreSlim(0);
        trigger.When(x => x.Invoke()).Do(_ => triggerGate.Release());

        var signalGate = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTrigger(t => t.Once().Await(token => signalGate.WaitAsync(token)).ThenInvoke(trigger))
                        .WithTransition("ToB", "B")
            )
            .WithState("B", state => state.WithTransition("ToA", "A"))
            .Build();

        var token = TestContext.Current.CancellationToken;

        // First entry cycle
        await stateMachine.Activate(token);
        signalGate.Release();
        await triggerGate.WaitAsync(token);
        trigger.Received(1).Invoke();

        // Transition away and back
        await stateMachine.Transition("ToB", token);

        // Act
        await stateMachine.Transition("ToA", token);
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
            .WithState(
                "A",
                state =>
                    state
                        .WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(TriggerAction))
                        .WithTransition("ToB", "B")
            )
            .WithState("B", state => state)
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

    [Fact(Timeout = 5000)]
    public async Task Once_WithStateMachineAccess_ShouldDeactivateStateMachine()
    {
        // Arrange
        var deactivateCompleted = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(TriggerAction)))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await deactivateCompleted.Task;

        // Assert
        Assert.Null(stateMachine.CurrentState);

        return;

        async Task TriggerAction(IStateMachine<string, string> sm, CancellationToken token)
        {
            await sm.Deactivate(token);
            deactivateCompleted.TrySetResult();
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Once_WhenMultipleTriggersOnSameState_FirstTransitionShouldDeactivateSecond()
    {
        // Arrange
        var secondTrigger = Substitute.For<Action>();
        var transitionCompleted = new TaskCompletionSource();
        var firstSignalGate = new SemaphoreSlim(0);
        var secondSignalGate = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTrigger(t =>
                            t.Once().Await(token => firstSignalGate.WaitAsync(token)).ThenInvoke(TriggerAction)
                        )
                        .WithTrigger(t =>
                            t.Once().Await(token => secondSignalGate.Task.WaitAsync(token)).ThenInvoke(secondTrigger)
                        )
                        .WithTransition("ToB", "B")
            )
            .WithState("B", state => state)
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);

        // Act
        firstSignalGate.Release();
        await transitionCompleted.Task;
        secondSignalGate.SetResult();

        // Assert
        Assert.Equal("B", stateMachine.CurrentState?.StateValue);
        secondTrigger.DidNotReceive().Invoke();

        return;

        async Task TriggerAction(IStateMachine<string, string> sm, CancellationToken ct)
        {
            await sm.Transition("ToB", ct);
            transitionCompleted.TrySetResult();
        }
    }
}
