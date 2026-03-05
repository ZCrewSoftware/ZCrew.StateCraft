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

    [Fact(Timeout = 5000)]
    public async Task Deactivate_WithAsyncActionDeactivatingSelf_ShouldNotDeadlock()
    {
        // Arrange
        var tokenCanceled = new TaskCompletionSource();
        IStateMachine<string, string> stateMachine = null!;
        stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithAction(action =>
                        action.Invoke(
                            async Task (token) =>
                            {
                                // ReSharper disable once AccessToModifiedClosure
                                await stateMachine.Deactivate(token);

                                token.Register(() => tokenCanceled.SetResult());
                            }
                        )
                    )
            )
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await tokenCanceled.Task;

        // Assert
        Assert.Null(stateMachine.CurrentState);
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithAsyncActionThrowingAfterSelfTransition_ShouldRemainInTransitionedState()
    {
        // Arrange
        var onBEntry = Substitute.For<Action>();
        var transitionCompleted = new TaskCompletionSource();
        var actionCompleted = new TaskCompletionSource();
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
                                    actionCompleted.SetResult();
                                    throw new InvalidOperationException("Post-transition error");
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state.OnEntry(onBEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await transitionCompleted.Task;

        // Assert
        onBEntry.Received(1).Invoke();
        await actionCompleted.Task;
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithAsyncActionMultipleSequentialSelfTransitions_ShouldReachFinalState()
    {
        // Arrange
        var onCEntry = Substitute.For<Action>();
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
                                    // The action's token is canceled after the first self-transition
                                    // (deferred CTS disposal), so use the test token for subsequent calls
                                    // ReSharper disable once AccessToModifiedClosure
                                    await stateMachine.Transition("To B", token);
                                    // ReSharper disable once AccessToModifiedClosure
                                    await stateMachine.Transition("To C", TestContext.Current.CancellationToken);
                                    chainCompleted.SetResult();
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state.WithTransition("To C", "C"))
            .WithState("C", state => state.OnEntry(onCEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await chainCompleted.Task;

        // Assert
        onCEntry.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task TryTransition_WithAsyncActionForInvalidTransition_ShouldReturnFalse()
    {
        // Arrange
        var tryTransitionResult = new TaskCompletionSource<bool>();
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
                                    var result = await stateMachine.TryTransition("Nonexistent", token);
                                    tryTransitionResult.SetResult(result);
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state)
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        var result = await tryTransitionResult.Task;

        // Assert
        Assert.False(result);
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithAsyncActionReentrantSelfTransition_ShouldReenterState()
    {
        // Arrange
        var onAEntry = Substitute.For<Action>();
        var transitionCompleted = new TaskCompletionSource();
        var tokenCanceled = new TaskCompletionSource();
        IStateMachine<string, string> stateMachine = null!;
        var isFirstEntry = true;
        stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .OnEntry(onAEntry)
                        .WithAction(action =>
                            action.Invoke(
                                async Task (token) =>
                                {
                                    if (!isFirstEntry)
                                        return;

                                    isFirstEntry = false;

                                    // ReSharper disable once AccessToModifiedClosure
                                    await stateMachine.Transition("Loop", token);
                                    transitionCompleted.SetResult();

                                    token.Register(() => tokenCanceled.SetResult());
                                }
                            )
                        )
                        .WithTransition("Loop", t => t.WithNoParameters().ToSameState())
            )
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await transitionCompleted.Task;

        // Assert
        onAEntry.Received(2).Invoke();
        await tokenCanceled.Task;
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithAsyncActionParameterizedSelfTransition_ShouldTransitionWithParameter()
    {
        // Arrange
        int? receivedParameter = null;
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
                                    await stateMachine.Transition("To B", 42, token);
                                    transitionCompleted.SetResult();

                                    token.Register(() => tokenCanceled.SetResult());
                                }
                            )
                        )
                        .WithTransition("To B", t => t.WithParameter<int>().To("B"))
            )
            .WithState("B", state => state.WithParameter<int>().OnEntry((param) => receivedParameter = param))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await transitionCompleted.Task;

        // Assert
        Assert.Equal(42, receivedParameter);
        await tokenCanceled.Task;
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithAsyncActionCanTransitionThenTransition_ShouldNotDeadlock()
    {
        // Arrange
        var onBEntry = Substitute.For<Action>();
        var transitionCompleted = new TaskCompletionSource();
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
                                    var canTransition = await stateMachine.CanTransition("To B", token);
                                    if (canTransition)
                                    {
                                        // ReSharper disable once AccessToModifiedClosure
                                        await stateMachine.Transition("To B", token);
                                    }

                                    transitionCompleted.SetResult();
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state.OnEntry(onBEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await transitionCompleted.Task;

        // Assert
        onBEntry.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task CanTransition_WithAsyncActionForUnavailableTransition_ShouldReturnFalse()
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
                                    var result = await stateMachine.CanTransition("Nonexistent", token);
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
        Assert.False(canTransition);
    }

    [Fact(Timeout = 5000)]
    public async Task Deactivate_WithAsyncActionSelfTransitionThenDeactivate_ShouldNotDeadlock()
    {
        // Arrange
        var onBEntry = Substitute.For<Action>();
        var deactivateCompleted = new TaskCompletionSource();
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
                                    // The action's token is canceled after the self-transition
                                    // (deferred CTS disposal), so use the test token
                                    // ReSharper disable once AccessToModifiedClosure
                                    await stateMachine.Deactivate(TestContext.Current.CancellationToken);
                                    deactivateCompleted.SetResult();
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state.OnEntry(onBEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await deactivateCompleted.Task;

        // Assert
        onBEntry.Received(1).Invoke();
        Assert.Null(stateMachine.CurrentState);
    }

    [Fact(Timeout = 5000)]
    public async Task Transition_WithChainedAsyncActionsAcrossFourStates_ShouldReachFinalState()
    {
        // Arrange
        var onDEntry = Substitute.For<Action>();
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
                                }
                            )
                        )
                        .WithTransition("To C", "C")
            )
            .WithState(
                "C",
                state =>
                    state
                        .WithAction(action =>
                            action.Invoke(
                                async Task (token) =>
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    await stateMachine.Transition("To D", token);
                                    chainCompleted.SetResult();
                                }
                            )
                        )
                        .WithTransition("To D", "D")
            )
            .WithState("D", state => state.OnEntry(onDEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await chainCompleted.Task;

        // Assert
        onDEntry.Received(1).Invoke();
    }

    [Fact(Timeout = 5000)]
    public async Task Activate_WithCrossStateMachineAsyncActions_ShouldNotDeadlock()
    {
        // Arrange
        var sm1OnEntry = Substitute.For<Action>();
        var sm2OnEntry = Substitute.For<Action>();
        var sm1TokenCanceled = new TaskCompletionSource();
        var chainCompleted = new TaskCompletionSource();

        IStateMachine<string, string> sm2 = null!;

        var sm1 = StateMachine
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
                                    await sm2.Transition("To B", token);
                                    token.Register(() => sm1TokenCanceled.TrySetResult());
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state.OnEntry(sm1OnEntry))
            .Build();

        sm2 = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState(
                "B",
                state =>
                    state
                        .OnEntry(sm2OnEntry)
                        .WithAction(action =>
                            action.Invoke(
                                async Task (token) =>
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    await sm1.Transition("To B", token);
                                    chainCompleted.SetResult();
                                }
                            )
                        )
            )
            .Build();

        // Act
        await sm2.Activate(TestContext.Current.CancellationToken);
        await sm1.Activate(TestContext.Current.CancellationToken);
        await chainCompleted.Task;

        // Assert
        sm1OnEntry.Received(1).Invoke();
        sm2OnEntry.Received(1).Invoke();
        await sm1TokenCanceled.Task;
    }

    [Fact(Timeout = 5000)]
    public async Task Activate_WithCrossMachineAsyncActionCallbackAfterSelfTransition_ShouldNotDeadlock()
    {
        // Arrange
        var sm1ActionCanceled = new TaskCompletionSource();
        var sm1OnBEntry = Substitute.For<Action>();
        var chainCompleted = new TaskCompletionSource();

        IStateMachine<string, string> sm2 = null!;

        // sm1's action transitions sm2 (A → B), then waits
        var sm1 = StateMachine
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
                                    await sm2.Transition("To B", token);

                                    try
                                    {
                                        await Task.Delay(Timeout.Infinite, token);
                                    }
                                    catch (OperationCanceledException) when (token.IsCancellationRequested)
                                    {
                                        sm1ActionCanceled.TrySetResult();
                                    }
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state.OnEntry(sm1OnBEntry))
            .Build();

        // sm2's action self-transitions (B → C), then calls back to sm1
        sm2 = StateMachine
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
                                    // Self-transition within sm2
                                    // ReSharper disable once AccessToModifiedClosure
                                    await sm2.Transition("To C", token);

                                    // The action's token is canceled after the self-transition
                                    // (deferred CTS disposal), so use the test token.
                                    // AsyncLocal inherited sm1's ID from the call chain,
                                    // which must not cause a false-positive self-call
                                    // detection in sm1
                                    // ReSharper disable once AccessToModifiedClosure
                                    await sm1.TryTransition("To B", TestContext.Current.CancellationToken);
                                    chainCompleted.SetResult();
                                }
                            )
                        )
                        .WithTransition("To C", "C")
            )
            .WithState("C", state => state)
            .Build();

        // Act
        await sm2.Activate(TestContext.Current.CancellationToken);
        await sm1.Activate(TestContext.Current.CancellationToken);
        await chainCompleted.Task;

        // Assert
        sm1OnBEntry.Received(1).Invoke();
        await sm1ActionCanceled.Task;
    }

    [Fact(Timeout = 5000)]
    public async Task Activate_WithAsyncActionThrowingAfterSelfTransition_ShouldCallOnExceptionHandler()
    {
        // Arrange
        var exception = new InvalidOperationException("Post-transition error");
        var exceptionHandled = new TaskCompletionSource<Exception>();
        var onBEntry = Substitute.For<Action>();
        IStateMachine<string, string> stateMachine = null!;
        stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .OnException(ex =>
            {
                exceptionHandled.TrySetResult(ex);
                return ExceptionResult.Rethrow();
            })
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
                                    throw exception;
                                }
                            )
                        )
                        .WithTransition("To B", "B")
            )
            .WithState("B", state => state.OnEntry(onBEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);
        var handledException = await exceptionHandled.Task;

        // Assert
        onBEntry.Received(1).Invoke();
        Assert.Same(exception, handledException);
    }
}
