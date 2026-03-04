using NSubstitute;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.IntegrationTests.Actions;

public class WithAsynchronousActionsTests
{
    [Fact(Timeout = 5000)]
    public async Task Activate_WithAsyncAction_ShouldNotAwaitAction()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .OnEntry(onEntry)
                        .WithAction(a =>
                            a.Invoke(_ => Task.Delay(Timeout.Infinite, TestContext.Current.CancellationToken))
                        )
            )
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Activate_WithSyncAction_ShouldNotAwaitAction()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.OnEntry(onEntry).WithAction(a => a.Invoke(() => Thread.Sleep(TimeSpan.FromSeconds(5))))
            )
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Activate_WithCompletedAction_ShouldNotAwaitAction()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.OnEntry(onEntry).WithAction(a => a.Invoke(_ => Task.CompletedTask)))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Activate_WithAsyncAction_ShouldBeCanceledDuringNextTransition()
    {
        // Arrange
        var tokenWasCanceled = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(tokenWasCanceled);

        return;

        async Task Action(CancellationToken token)
        {
            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                tokenWasCanceled = true;
            }
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Activate_WithActionThrowingException_ShouldTransitionSuccessfully()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var activateCompleted = new TaskCompletionSource();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        activateCompleted.SetResult();

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke();

        return;

        async Task Action(CancellationToken _)
        {
            await activateCompleted.Task;
            throw new Exception("Test Exception");
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Activate_WithAsyncAction_ShouldBeCanceledDuringDeactivation()
    {
        // Arrange
        var tokenWasCanceled = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(tokenWasCanceled);

        return;

        async Task Action(CancellationToken token)
        {
            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                tokenWasCanceled = true;
            }
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithAsyncAction_ShouldNotAwaitAction()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState(
                "B",
                state =>
                    state
                        .OnEntry(onEntry)
                        .WithAction(a =>
                            a.Invoke(_ => Task.Delay(Timeout.Infinite, TestContext.Current.CancellationToken))
                        )
            )
            .Build();
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithSyncAction_ShouldNotAwaitAction()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState(
                "B",
                state =>
                    state
                        .OnEntry(onEntry)
                        .WithAction(a =>
                            a.Invoke(() =>
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(5));
                                Console.WriteLine("Done sleeping");
                            })
                        )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithCompletedAction_ShouldNotAwaitAction()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.OnEntry(onEntry).WithTransition("To B", t => t.To("B")))
            .WithState(
                "B",
                state => state.WithAction(a => a.Invoke(_ => Task.CompletedTask)).WithTransition("To A", t => t.To("A"))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To A", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(2).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithAsyncAction_ShouldBeCanceledDuringNextTransition()
    {
        // Arrange
        var tokenWasCanceled = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithAction(a => a.Invoke(Action)).WithTransition("To A", t => t.To("A")))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To A", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(tokenWasCanceled);

        return;

        async Task Action(CancellationToken token)
        {
            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                tokenWasCanceled = true;
            }
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithActionThrowingException_ShouldTransitionSuccessfully()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var transitionCompleted = new TaskCompletionSource();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.OnEntry(onEntry).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithAction(a => a.Invoke(Action)).WithTransition("To A", t => t.To("A")))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        var transition = stateMachine.Transition("To A", TestContext.Current.CancellationToken);
        transitionCompleted.SetResult();
        await transition;

        // Assert
        onEntry.Received(2).Invoke();

        return;

        async Task Action(CancellationToken _)
        {
            await transitionCompleted.Task;
            throw new Exception("Test Exception");
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithAsyncAction_ShouldBeCanceledDuringDeactivation()
    {
        // Arrange
        var tokenWasCanceled = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(tokenWasCanceled);

        return;

        async Task Action(CancellationToken token)
        {
            try
            {
                await Task.Delay(Timeout.Infinite, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                tokenWasCanceled = true;
            }
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithAsyncAction_ShouldAllowTransition()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var transitionCompleted = new TaskCompletionSource();
        var tokenCanceled = new TaskCompletionSource();
        IStateMachine<string, string> stateMachine = null!;
        stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithAction(action =>
                            action.Invoke(
                                async Task (token) =>
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    await stateMachine.Transition("To B", token);
                                    transitionCompleted.SetResult();

                                    token.Register(() => tokenCanceled.SetResult());
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state.OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await transitionCompleted.Task;

        // Assert
        onEntry.Received(1).Invoke();
        await tokenCanceled.Task;
    }

    [Fact(Timeout = 5000)]
    public async Task TryTransition_WithAsyncAction_ShouldAllowTransition()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var transitionCompleted = new TaskCompletionSource();
        var tokenCanceled = new TaskCompletionSource();
        IStateMachine<string, string> stateMachine = null!;
        stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithAction(action =>
                            action.Invoke(
                                async Task (token) =>
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    await stateMachine.TryTransition("To B", token);
                                    transitionCompleted.SetResult();

                                    token.Register(() => tokenCanceled.SetResult());
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state.OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await transitionCompleted.Task;

        // Assert
        onEntry.Received(1).Invoke();
        await tokenCanceled.Task;
    }

    [Fact(Timeout = 5000)]
    public async Task CanTransition_WithAsyncAction_ShouldNotDeadlock()
    {
        // Arrange
        var canTransitionResult = new TaskCompletionSource<bool>();
        IStateMachine<string, string> stateMachine = null!;
        stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithAction(action =>
                            action.Invoke(
                                async Task (token) =>
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    var result = await stateMachine.CanTransition("To B", token);
                                    canTransitionResult.SetResult(result);
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state)
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        var canTransition = await canTransitionResult.Task;

        // Assert
        Assert.True(canTransition);
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithAsyncActionAfterTransition_ShouldAllowTransition()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var transitionCompleted = new TaskCompletionSource();
        var tokenCanceled = new TaskCompletionSource();
        IStateMachine<string, string> stateMachine = null!;
        stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState(
                "B",
                state =>
                    state
                        .WithAction(action =>
                            action.Invoke(
                                async Task (token) =>
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    await stateMachine.Transition("To C", token);
                                    transitionCompleted.SetResult();

                                    token.Register(() => tokenCanceled.SetResult());
                                }
                            )
                        )
                        .WithTransition("To C", "C")
            )
            .WithState("C", state => state.OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);
        await transitionCompleted.Task;

        // Assert
        onEntry.Received(1).Invoke();
        await tokenCanceled.Task;
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithChainedAsyncActions_ShouldAllowTransitions()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var chainCompleted = new TaskCompletionSource();
        IStateMachine<string, string> stateMachine = null!;
        stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .WithAction(action =>
                            action.Invoke(
                                async Task (token) =>
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    await stateMachine.Transition("To B", token);
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState(
                "B",
                state =>
                    state
                        .WithAction(action =>
                            action.Invoke(
                                async Task (token) =>
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    await stateMachine.Transition("To C", token);
                                    chainCompleted.SetResult();
                                }
                            )
                        )
                        .WithTransition("To C", "C")
            )
            .WithState("C", state => state.OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await chainCompleted.Task;

        // Assert
        onEntry.Received(1).Invoke();
    }
}
