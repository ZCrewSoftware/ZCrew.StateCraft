using NSubstitute;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.IntegrationTests.Triggers;

public class StateRepeatingTriggerTests
{
    [Fact(Timeout = 5000)]
    public async Task Repeat_WhenInState_ShouldExecuteTriggerRepeatedly()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var triggerGate = new SemaphoreSlim(0);
        trigger.When(x => x.Invoke()).Do(_ => triggerGate.Release());

        var semaphore = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTrigger(t => t.Repeat().Await(token => semaphore.WaitAsync(token)).ThenInvoke(trigger))
            )
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);

        // Act
        semaphore.Release();
        await triggerGate.WaitAsync(token);
        semaphore.Release();
        await triggerGate.WaitAsync(token);
        semaphore.Release();
        await triggerGate.WaitAsync(token);

        // Assert
        trigger.Received(3).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Repeat_WhenTransitionedAway_ShouldStopRepeating()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var triggerGate = new SemaphoreSlim(0);
        trigger.When(x => x.Invoke()).Do(_ => triggerGate.Release());

        var semaphore = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTrigger(t => t.Repeat().Await(token => semaphore.WaitAsync(token)).ThenInvoke(trigger))
                        .WithTransition("ToB", "B")
            )
            .WithState("B", state => state)
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);

        // Trigger a few times
        semaphore.Release();
        await triggerGate.WaitAsync(token);
        semaphore.Release();
        await triggerGate.WaitAsync(token);
        var countBeforeTransition = trigger.ReceivedCalls().Count();

        // Act
        await stateMachine.Transition("ToB", token);
        semaphore.Release();

        // Assert
        Assert.Equal(countBeforeTransition, trigger.ReceivedCalls().Count());
    }

    [Fact(Timeout = 5000)]
    public async Task Repeat_WithStateMachineAccess_ShouldTransitionStateMachine()
    {
        // Arrange
        var semaphore = new SemaphoreSlim(0);
        var transitionGate = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithTrigger(t =>
                            t.Repeat().Await(token => semaphore.WaitAsync(token)).ThenInvoke(TriggerAction)
                        )
                        .WithTransition("Next", "B")
            )
            .WithState(
                "B",
                state =>
                    state
                        .WithTrigger(t =>
                            t.Repeat().Await(token => semaphore.WaitAsync(token)).ThenInvoke(TriggerAction)
                        )
                        .WithTransition("Next", "C")
            )
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        Assert.Equal("A", stateMachine.CurrentState?.StateValue);

        // Act
        semaphore.Release();
        await transitionGate.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Equal("B", stateMachine.CurrentState?.StateValue);

        semaphore.Release();
        await transitionGate.WaitAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("C", stateMachine.CurrentState?.StateValue);

        return;

        async Task TriggerAction(IStateMachine<string, string> sm, CancellationToken token)
        {
            await sm.Transition("Next", token);
            transitionGate.Release();
        }
    }
}
