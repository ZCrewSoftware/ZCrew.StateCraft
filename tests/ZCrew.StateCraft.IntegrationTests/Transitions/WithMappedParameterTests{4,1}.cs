using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class WithMappedParameterTests_4_1
{
    [Fact]
    public async Task Transition_WithMappedParameter_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}").To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallMappingFunction()
    {
        // Arrange
        var mappingFunction = Substitute.For<Func<int, string, bool, double, string>>();
        mappingFunction
            .Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<double>())
            .Returns(x => $"{x.ArgAt<int>(0)}-{x.ArgAt<string>(1)}-{x.ArgAt<bool>(2)}-{x.ArgAt<double>(3)}");

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition("To B", t => t.WithMappedParameter(mappingFunction).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        mappingFunction.Received(1).Invoke(42, "hello", true, 3.14);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldTransformParameterType()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, b, _, _) => $"{a}-{b}").To("B"))
            )
            .WithState("B", state => state.WithParameter<string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke("42-hello");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldTransformParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, _, _, _) => $"{a * 2}").To("B"))
            )
            .WithState("B", state => state.WithParameter<string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke("84");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallOnExitWithPreviousParameter()
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
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}").To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42, "hello", true, 3.14);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallOnEntryWithMappedParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}").To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke("42-hello-True-3.14");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallOnStateChangeForNextStateWithMappedParameter()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}").To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", "42-hello-True-3.14");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallOnStateChangeHandlersOnMachine()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .OnStateChange(onStateChange)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}").To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallActionWithMappedParameter()
    {
        // Arrange
        var invoke = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}").To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>().WithAction(action => action.Invoke(invoke)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke("42-hello-True-3.14");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onStateChangeState = Substitute.For<Action<string, string, string, string>>();
        var onEntry = Substitute.For<Action<string>>();
        var invoke = Substitute.For<Action<string>>();
        var onExit = Substitute.For<Action<int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .OnStateChange(onStateChangeMachine)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .OnExit(onExit)
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}").To("B")
                        )
            )
            .WithState(
                "B",
                state =>
                    state
                        .WithParameter<string>()
                        .OnStateChange(onStateChangeState)
                        .OnEntry(onEntry)
                        .WithAction(action => action.Invoke(invoke))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onExit.Received(1).Invoke(42, "hello", true, 3.14);
            onStateChangeMachine.Received(1).Invoke("A", "To B", "B");
            onStateChangeState.Received(1).Invoke("A", "To B", "B", "42-hello-True-3.14");
            onEntry.Received(1).Invoke("42-hello-True-3.14");
            invoke.Received(1).Invoke("42-hello-True-3.14");
        });
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenConditionOnMappedIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}").If(_ => true).To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenConditionOnMappedIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}").If(_ => false).To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenConditionOnPreviousIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _, _) => true)
                                    .WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}")
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenConditionOnPreviousIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If((_, _, _, _) => false)
                                    .WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}")
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WithTaskMapping_ShouldTransformParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        Func<int, string, bool, double, CancellationToken, Task<string>> mapping = async (a, b, c, d, _) =>
            await Task.FromResult($"{a}-{b}-{c}-{d}");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(mapping).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke("42-hello-True-3.14");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WithValueTaskMapping_ShouldTransformParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        Func<int, string, bool, double, CancellationToken, ValueTask<string>> mapping = (a, b, c, d, _) =>
            new ValueTask<string>($"{a}-{b}-{c}-{d}");
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(mapping).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke("42-hello-True-3.14");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ToSameState_ShouldTransitionToSameState()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "Loop",
                            t => t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}").ToSameState()
                        )
            )
            .WithState("A", state => state.WithParameter<string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Loop", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        onEntry.Received(1).Invoke("42-hello-True-3.14");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenTaskConditionOnMappedIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}")
                                    .If((_, _) => Task.FromResult(true))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenTaskConditionOnMappedIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}")
                                    .If((_, _) => Task.FromResult(false))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenValueTaskConditionOnMappedIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}")
                                    .If((_, _) => ValueTask.FromResult(true))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenValueTaskConditionOnMappedIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>((a, b, c, d) => $"{a}-{b}-{c}-{d}")
                                    .If((_, _) => ValueTask.FromResult(false))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }
}
