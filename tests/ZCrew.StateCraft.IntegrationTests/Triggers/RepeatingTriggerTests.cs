using NSubstitute;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.IntegrationTests.Triggers;

public class RepeatingTriggerTests
{
    [Fact(Timeout = 5000)]
    public async Task Repeat_WhenSignalCompletesMultipleTimes_ShouldExecuteTriggerEachTime()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var triggerGate = new SemaphoreSlim(0);
        trigger.When(x => x.Invoke()).Do(_ => triggerGate.Release());

        var semaphore = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Repeat().Await(token => semaphore.WaitAsync(token)).ThenInvoke(trigger))
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
    public async Task Repeat_WhenDeactivated_ShouldStopRepeating()
    {
        // Arrange
        var trigger = Substitute.For<Action>();
        var triggerGate = new SemaphoreSlim(0);
        trigger.When(x => x.Invoke()).Do(_ => triggerGate.Release());

        var semaphore = new SemaphoreSlim(0);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithTrigger(t => t.Repeat().Await(token => semaphore.WaitAsync(token)).ThenInvoke(trigger))
            .Build();

        var token = TestContext.Current.CancellationToken;
        await stateMachine.Activate(token);

        // Trigger a few times
        semaphore.Release();
        await triggerGate.WaitAsync(token);
        semaphore.Release();
        await triggerGate.WaitAsync(token);
        var countBeforeDeactivate = trigger.ReceivedCalls().Count();

        // Act
        await stateMachine.Deactivate(token);
        semaphore.Release();

        // Assert
        Assert.Equal(countBeforeDeactivate, trigger.ReceivedCalls().Count());
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
            .WithState("A", state => state.WithTransition("Next", "B"))
            .WithState("B", state => state.WithTransition("Next", "C"))
            .WithState("C", state => state)
            .WithTrigger(t => t.Repeat().Await(token => semaphore.WaitAsync(token)).ThenInvoke(TriggerAction))
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
