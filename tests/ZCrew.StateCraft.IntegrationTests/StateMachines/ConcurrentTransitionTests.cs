using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class ConcurrentTransitionTests
{
    [Fact(Timeout = 5000)]
    public async Task Transition_WhenConcurrentTransitionsOnSameMachine_ShouldSerialize()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource();
        var exitAStarted = new TaskCompletionSource();
        var onExitA = Substitute.For<Action>();
        var onExitB = Substitute.For<Action>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(OnExitA).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.OnExit(onExitB).WithTransition("To C", t => t.To("C")))
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var firstTransition = stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        await exitAStarted.Task;

        var secondTransition = stateMachine.Transition("To C", TestContext.Current.CancellationToken);

        blockingTcs.SetResult();

        await Task.WhenAll(firstTransition, secondTransition);

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
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WhenHighContention_ShouldCompleteAllTransitions()
    {
        // Arrange
        var startGate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithTransition("To A", t => t.To("A")))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        var token = TestContext.Current.CancellationToken;

        // Act
        var tasks = Enumerable
            .Range(0, 10)
            .Select(i => i % 2 == 0 ? "To A" : "To B")
            .Select(TryTransitionUntilSuccessful)
            .ToArray();
        startGate.SetResult();

        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal("A", stateMachine.CurrentState?.StateValue);

        return;

        async Task TryTransitionUntilSuccessful(string transition)
        {
            await startGate.Task;

            while (!token.IsCancellationRequested)
            {
                var result = await stateMachine.TryTransition(transition, token);
                if (result)
                {
                    return;
                }
            }
        }
    }
}
