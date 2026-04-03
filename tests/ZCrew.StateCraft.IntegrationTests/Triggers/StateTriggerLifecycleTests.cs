using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Triggers;

public class StateTriggerLifecycleTests
{
    [Fact(Timeout = 5000)]
    public async Task Enter_ShouldActivateTriggerAfterEntryHandlers()
    {
        // Arrange
        var order = new List<string>();
        var triggerCalled = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .OnEntry(() => order.Add("entry"))
                        .WithTrigger(t =>
                            t.Once()
                                .Await(() => { })
                                .ThenInvoke(() =>
                                {
                                    order.Add("trigger");
                                    triggerCalled.TrySetResult();
                                })
                        )
            )
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await triggerCalled.Task;

        // Assert
        Assert.Equal(["entry", "trigger"], order);
    }

    [Fact(Timeout = 5000)]
    public async Task Exit_ShouldDeactivateTriggerBeforeExitHandlers()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var signalGate = new TaskCompletionSource();
        var exitHandlerCalled = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTrigger(t => t.Once().Await(token => signalGate.Task.WaitAsync(token)).ThenInvoke(trigger))
                        .OnExit(() => exitHandlerCalled.TrySetResult())
                        .WithTransition("ToB", "B")
            )
            .WithState("B", state => state)
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);

        // Act
        await stateMachine.Transition("ToB", token);
        await exitHandlerCalled.Task;

        // Complete the signal after deactivation
        signalGate.SetResult();

        // Assert
        trigger.DidNotReceive().Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task StateScopedAndMachineScopedTriggers_ShouldBothOperate()
    {
        // Arrange
        var stateScopedTrigger = Substitute.For<Action>();
        var machineScopedTrigger = Substitute.For<Action>();
        var stateScopedCalled = new TaskCompletionSource();
        var machineScopedCalled = new TaskCompletionSource();
        stateScopedTrigger.When(x => x.Invoke()).Do(_ => stateScopedCalled.TrySetResult());
        machineScopedTrigger.When(x => x.Invoke()).Do(_ => machineScopedCalled.TrySetResult());

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(stateScopedTrigger)))
            .WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(machineScopedTrigger))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateScopedCalled.Task;
        await machineScopedCalled.Task;

        // Assert
        stateScopedTrigger.Received(1).Invoke();
        machineScopedTrigger.Received(1).Invoke();
    }
}
