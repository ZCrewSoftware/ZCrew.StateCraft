using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class TransitionTests_T1_T2_T3_T4
{
    [Fact]
    public async Task Transition_T1_T2_T3_T4_WhenCalled_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WhenCalled_ShouldPassParametersToOnEntry()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello", true, 3.14);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WhenCalled_ShouldPassParametersToAction()
    {
        // Arrange
        var invoke = Substitute.For<Action<int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState(
                "B",
                state => state.WithParameters<int, string, bool, double>().WithAction(action => action.Invoke(invoke))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42, "hello", true, 3.14);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WhenCalled_ShouldInvokeLifecycleHandlersInOrder()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onEntry = Substitute.For<Action<int, string, bool, double>>();
        var invoke = Substitute.For<Action<int, string, bool, double>>();
        var onExit = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange(onStateChangeMachine)
            .WithState("A", state => state.OnExit(onExit).WithTransition<int, string, bool, double>("To B", "B"))
            .WithState(
                "B",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .OnEntry(onEntry)
                        .WithAction(action => action.Invoke(invoke))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onExit.Received(1).Invoke();
            onStateChangeMachine.Received(1).Invoke("A", "To B", "B");
            onEntry.Received(1).Invoke(42, "hello", true, 3.14);
            invoke.Received(1).Invoke(42, "hello", true, 3.14);
        });
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WhenNotActivated_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithCondition_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.WithParameters<int, string, bool, double>().If((_, _, _, _) => true).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithCondition_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.WithParameters<int, string, bool, double>().If((_, _, _, _) => false).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithCondition_Async_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameters<int, string, bool, double>()
                                .If((_, _, _, _, _) => Task.FromResult(true))
                                .To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithCondition_Async_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameters<int, string, bool, double>()
                                .If((_, _, _, _, _) => Task.FromResult(false))
                                .To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithCondition_ValueTaskAsync_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameters<int, string, bool, double>()
                                .If((_, _, _, _, _) => ValueTask.FromResult(true))
                                .To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithCondition_ValueTaskAsync_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameters<int, string, bool, double>()
                                .If((_, _, _, _, _) => ValueTask.FromResult(false))
                                .To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WhenFromParameterizedState_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState("A", state => state.WithParameter<int>().WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithShortcut_WhenCalled_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WhenCalled_ShouldCallOnStateChangeForNextState()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", 42, "hello", true, 3.14);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WhenCalled_ShouldCallMethodsInCorrectOrderIncludingStateChange()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onStateChangeState = Substitute.For<Action<string, string, string, int, string, bool, double>>();
        var onEntry = Substitute.For<Action<int, string, bool, double>>();
        var invoke = Substitute.For<Action<int, string, bool, double>>();
        var onExit = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange(onStateChangeMachine)
            .WithState("A", state => state.OnExit(onExit).WithTransition<int, string, bool, double>("To B", "B"))
            .WithState(
                "B",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .OnStateChange(onStateChangeState)
                        .OnEntry(onEntry)
                        .WithAction(action => action.Invoke(invoke))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onExit.Received(1).Invoke();
            onStateChangeMachine.Received(1).Invoke("A", "To B", "B");
            onStateChangeState.Received(1).Invoke("A", "To B", "B", 42, "hello", true, 3.14);
            onEntry.Received(1).Invoke(42, "hello", true, 3.14);
            invoke.Received(1).Invoke(42, "hello", true, 3.14);
        });
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WhenFromMultiParamState_ShouldCallOnExitWithParameters()
    {
        // Arrange
        var onExit = Substitute.For<Action<int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .OnExit(onExit)
                        .WithTransition("To B", t => t.To("B"))
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42, "hello", true, 3.14);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithAsyncOnEntryHandler_ShouldCallHandler()
    {
        // Arrange
        var onEntry = Substitute.For<Func<int, string, bool, double, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await onEntry.Received(1).Invoke(42, "hello", true, 3.14, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithValueTaskOnEntryHandler_ShouldCallHandler()
    {
        // Arrange
        var onEntry = Substitute.For<Func<int, string, bool, double, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await onEntry.Received(1).Invoke(42, "hello", true, 3.14, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithAsyncOnExitHandler_ShouldCallHandler()
    {
        // Arrange
        var onExit = Substitute.For<Func<int, string, bool, double, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .OnExit(onExit)
                        .WithTransition("To B", t => t.To("B"))
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await onExit.Received(1).Invoke(42, "hello", true, 3.14, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithValueTaskOnExitHandler_ShouldCallHandler()
    {
        // Arrange
        var onExit = Substitute.For<Func<int, string, bool, double, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .OnExit(onExit)
                        .WithTransition("To B", t => t.To("B"))
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await onExit.Received(1).Invoke(42, "hello", true, 3.14, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithAsyncOnStateChangeHandler_ShouldCallHandler()
    {
        // Arrange
        var onStateChange = Substitute.For<
            Func<string, string, string, int, string, bool, double, CancellationToken, Task>
        >();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await onStateChange.Received(1).Invoke("A", "To B", "B", 42, "hello", true, 3.14, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.If(() => true).WithParameters<int, string, bool, double>().To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.If(() => false).WithParameters<int, string, bool, double>().To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_Async_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.If(_ => Task.FromResult(true)).WithParameters<int, string, bool, double>().To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_Async_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.If(_ => Task.FromResult(false)).WithParameters<int, string, bool, double>().To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_ValueTaskAsync_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.If(_ => ValueTask.FromResult(true)).WithParameters<int, string, bool, double>().To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_ValueTaskAsync_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.If(_ => ValueTask.FromResult(false)).WithParameters<int, string, bool, double>().To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t => t.If(_ => true).WithParameters<int, string, bool, double>().To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t => t.If(_ => false).WithParameters<int, string, bool, double>().To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT_Async_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _) => Task.FromResult(true))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT_Async_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _) => Task.FromResult(false))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT_ValueTaskAsync_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _) => ValueTask.FromResult(true))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT_ValueTaskAsync_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _) => ValueTask.FromResult(false))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t => t.If((_, _) => true).WithParameters<int, string, bool, double>().To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t => t.If((_, _) => false).WithParameters<int, string, bool, double>().To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_Async_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _) => Task.FromResult(true))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_Async_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _) => Task.FromResult(false))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_ValueTaskAsync_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _) => ValueTask.FromResult(true))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_ValueTaskAsync_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _) => ValueTask.FromResult(false))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool>()
                        .WithTransition(
                            "To B",
                            t => t.If((_, _, _) => true).WithParameters<int, string, bool, double>().To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool>()
                        .WithTransition(
                            "To B",
                            t => t.If((_, _, _) => false).WithParameters<int, string, bool, double>().To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_Async_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _, _) => Task.FromResult(true))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_Async_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _, _) => Task.FromResult(false))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_ValueTaskAsync_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _, _) => ValueTask.FromResult(true))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_ValueTaskAsync_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _, _) => ValueTask.FromResult(false))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_T4_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t => t.If((_, _, _, _) => true).WithParameters<int, string, bool, double>().To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_T4_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t => t.If((_, _, _, _) => false).WithParameters<int, string, bool, double>().To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_T4_Async_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _, _, _) => Task.FromResult(true))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_T4_Async_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _, _, _) => Task.FromResult(false))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_T4_ValueTaskAsync_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _, _, _) => ValueTask.FromResult(true))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_T4_WithInitialCondition_FromT1_T2_T3_T4_ValueTaskAsync_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10, "x", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _, _, _) => ValueTask.FromResult(false))
                                    .WithParameters<int, string, bool, double>()
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }
}
