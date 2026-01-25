using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class LockingTests
{
    [Fact(Timeout = 5000)]
    public async Task Transition_WhenConcurrentTransitionsCalled_ShouldSerializeOperations()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource();
        var firstTransitionStarted = new TaskCompletionSource();
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
        await firstTransitionStarted.Task;

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
            firstTransitionStarted.SetResult();
            await blockingTcs.Task;
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Activate_WhenConcurrentActivateCalled_ShouldOnlyAllowOne()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource();
        var activationStarted = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnActivate(OnActivate))
            .Build();

        // Act
        var firstActivation = stateMachine.Activate(TestContext.Current.CancellationToken);
        await activationStarted.Task;

        var secondActivation = stateMachine.Activate(TestContext.Current.CancellationToken);

        blockingTcs.SetResult();

        await firstActivation;

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => secondActivation);

        return;

        async Task OnActivate(string _, CancellationToken __)
        {
            activationStarted.SetResult();
            await blockingTcs.Task;
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Deactivate_WhenConcurrentDeactivateCalled_ShouldOnlyAllowOne()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource();
        var deactivationStarted = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnDeactivate(OnDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var firstDeactivation = stateMachine.Deactivate(TestContext.Current.CancellationToken);
        await deactivationStarted.Task;

        var secondDeactivation = stateMachine.Deactivate(TestContext.Current.CancellationToken);

        blockingTcs.SetResult();

        await firstDeactivation;

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => secondDeactivation);

        return;

        async Task OnDeactivate(string _, CancellationToken __)
        {
            deactivationStarted.SetResult();
            await blockingTcs.Task;
        }
    }

    [Fact(Timeout = 5000)]
    public async Task CanTransition_WhenCalledDuringTransition_ShouldWaitForLock()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource();
        var transitionStarted = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(OnExit).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithTransition("To C", t => t.To("C")))
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        await transitionStarted.Task;

        var canTransitionTask = stateMachine.CanTransition("To C", TestContext.Current.CancellationToken);

        blockingTcs.SetResult();

        await transition;
        var canTransition = await canTransitionTask;

        // Assert
        Assert.True(canTransition);
        Assert.Equal("B", stateMachine.CurrentState?.StateValue);

        return;

        async Task OnExit(CancellationToken _)
        {
            transitionStarted.SetResult();
            await blockingTcs.Task;
        }
    }

    [Fact(Timeout = 5000)]
    public async Task TryTransition_WhenCalledDuringTransition_ShouldWaitForLock()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource();
        var transitionStarted = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(OnExit).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithTransition("To C", t => t.To("C")))
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var firstTransition = stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        await transitionStarted.Task;

        var tryTransitionTask = stateMachine.TryTransition("To C", TestContext.Current.CancellationToken);

        blockingTcs.SetResult();

        await firstTransition;
        var tryTransitionResult = await tryTransitionTask;

        // Assert
        Assert.True(tryTransitionResult);
        Assert.Equal("C", stateMachine.CurrentState?.StateValue);

        return;

        async Task OnExit(CancellationToken _)
        {
            transitionStarted.SetResult();
            await blockingTcs.Task;
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WhenCalledDuringActivation_ShouldWaitForLock()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource();
        var activationStarted = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnEntry(OnEntry).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();

        // Act
        var activation = stateMachine.Activate(TestContext.Current.CancellationToken);
        await activationStarted.Task;

        var transitionTask = stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        blockingTcs.SetResult();

        await activation;
        await transitionTask;

        // Assert
        Assert.Equal("B", stateMachine.CurrentState?.StateValue);

        return;

        async Task OnEntry(CancellationToken _)
        {
            activationStarted.SetResult();
            await blockingTcs.Task;
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Deactivate_WhenCalledDuringTransition_ShouldWaitForLock()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource();
        var transitionStarted = new TaskCompletionSource();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(OnExit).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        await transitionStarted.Task;

        var deactivateTask = stateMachine.Deactivate(TestContext.Current.CancellationToken);

        blockingTcs.SetResult();

        await transition;
        await deactivateTask;

        // Assert
        Assert.Null(stateMachine.CurrentState);

        return;

        async Task OnExit(CancellationToken _)
        {
            transitionStarted.SetResult();
            await blockingTcs.Task;
        }
    }

    [Fact(Timeout = 5000)]
    public async Task MultipleOperations_WhenCalledConcurrently_ShouldNotDeadlock()
    {
        // Arrange
        var blockingTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithTransition("To A", t => t.To("A")))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var tasks = Enumerable
            .Range(0, 10)
            .Select(i => i % 2 == 0 ? "To A" : "To B")
            .Select(TryTransitionUntilSuccessful)
            .ToArray();
        blockingTcs.SetResult();

        // Assert
        await Task.WhenAll(tasks);
        Assert.Equal("A", stateMachine.CurrentState?.StateValue);

        return;

        async Task TryTransitionUntilSuccessful(string transition)
        {
            // Wait for test start
            await blockingTcs.Task;

            while (!TestContext.Current.CancellationToken.IsCancellationRequested)
            {
                var result = await stateMachine.TryTransition(transition, TestContext.Current.CancellationToken);
                if (result)
                {
                    return;
                }
            }
        }
    }
}
