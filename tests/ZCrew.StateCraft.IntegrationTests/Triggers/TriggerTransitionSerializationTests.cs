using NSubstitute;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.IntegrationTests.Triggers;

public class TriggerTransitionSerializationTests
{
    [Fact(Timeout = 5000)]
    public async Task Transition_WhenTriggerFiresDuringUserTransition_ShouldSerialize()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource();
        var exitAStarted = new TaskCompletionSource();
        var triggerCompleted = new TaskCompletionSource();
        var signalGate = new TaskCompletionSource();
        var onExitA = Substitute.For<Action>();
        var onExitB = Substitute.For<Action>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.OnExit(OnExitA).WithTransition("To B", t => t.To("B"))
            )
            .WithState(
                "B",
                state => state.OnExit(onExitB).WithTransition("To C", t => t.To("C"))
            )
            .WithState("C", state => state)
            .WithTrigger(t =>
                t.Once().Await(token => signalGate.Task.WaitAsync(token)).ThenInvoke(TriggerAction)
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var userTransition = stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        await exitAStarted.Task;

        signalGate.SetResult();

        blockingTcs.SetResult();

        await userTransition;
        await triggerCompleted.Task;

        // Assert
        Assert.Equal("C", stateMachine.CurrentState?.StateValue);
        Received.InOrder(() =>
        {
            onExitA.Received(1).Invoke();
            onExitB.Received(1).Invoke();
        });

        return;

        async Task OnExitA(CancellationToken _)
        {
            onExitA();
            exitAStarted.SetResult();
            await blockingTcs.Task;
        }

        async Task TriggerAction(IStateMachine<string, string> sm, CancellationToken token)
        {
            await sm.Transition("To C", token);
            triggerCompleted.TrySetResult();
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WhenUserTransitionFiresDuringTrigger_ShouldSerialize()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource();
        var triggerExitStarted = new TaskCompletionSource();
        var onExitA = Substitute.For<Action>();
        var onExitB = Substitute.For<Action>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.OnExit(OnExitA).WithTransition("To B", t => t.To("B"))
            )
            .WithState(
                "B",
                state => state.OnExit(onExitB).WithTransition("To C", t => t.To("C"))
            )
            .WithState("C", state => state)
            .WithTrigger(t => t.Once().Await(() => { }).ThenInvoke(TriggerAction))
            .Build();

        // Act
        var activation = stateMachine.Activate(TestContext.Current.CancellationToken);
        await triggerExitStarted.Task;

        var userTransition = stateMachine.Transition("To C", TestContext.Current.CancellationToken);

        blockingTcs.SetResult();

        await activation;
        await userTransition;

        // Assert
        Assert.Equal("C", stateMachine.CurrentState?.StateValue);
        Received.InOrder(() =>
        {
            onExitA.Received(1).Invoke();
            onExitB.Received(1).Invoke();
        });

        return;

        async Task OnExitA(CancellationToken _)
        {
            onExitA();
            triggerExitStarted.SetResult();
            await blockingTcs.Task;
        }

        async Task TriggerAction(IStateMachine<string, string> sm, CancellationToken token)
        {
            await sm.Transition("To B", token);
        }
    }
}
