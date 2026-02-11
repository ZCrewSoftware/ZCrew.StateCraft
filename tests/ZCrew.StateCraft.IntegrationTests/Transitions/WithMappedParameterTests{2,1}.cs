using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class WithMappedParameterTests_2_1
{
    [Fact]
    public async Task Transition_WithMappedParameter_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").To("B"))
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
        var mappingFunction = Substitute.For<Func<int, string, string>>();
        mappingFunction
            .Invoke(Arg.Any<int>(), Arg.Any<string>())
            .Returns(x => $"{x.ArgAt<int>(0)}-{x.ArgAt<string>(1)}");

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithMappedParameter(mappingFunction).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        mappingFunction.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldTransformParameterType()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").To("B"))
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
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, b) => $"{a * 2}-{b}!").To("B"))
            )
            .WithState("B", state => state.WithParameter<string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke("84-hello!");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallOnExitWithPreviousParameter()
    {
        // Arrange
        var onExit = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .OnExit(onExit)
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallOnEntryWithMappedParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").To("B"))
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
    public async Task Transition_WithMappedParameter_ShouldCallOnStateChangeForNextStateWithMappedParameter()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").To("B"))
            )
            .WithState("B", state => state.WithParameter<string>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", "42-hello");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallOnStateChangeHandlersOnMachine()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .OnStateChange(onStateChange)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").To("B"))
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
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").To("B"))
            )
            .WithState("B", state => state.WithParameter<string>().WithAction(action => action.Invoke(invoke)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke("42-hello");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onStateChangeState = Substitute.For<Action<string, string, string, string>>();
        var onEntry = Substitute.For<Action<string>>();
        var invoke = Substitute.For<Action<string>>();
        var onExit = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .OnStateChange(onStateChangeMachine)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .OnExit(onExit)
                        .WithTransition("To B", t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").To("B"))
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
            onExit.Received(1).Invoke(42, "hello");
            onStateChangeMachine.Received(1).Invoke("A", "To B", "B");
            onStateChangeState.Received(1).Invoke("A", "To B", "B", "42-hello");
            onEntry.Received(1).Invoke("42-hello");
            invoke.Received(1).Invoke("42-hello");
        });
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenConditionOnMappedIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").If(_ => true).To("B")
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
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").If(_ => false).To("B")
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
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t => t.If((_, _) => true).WithMappedParameter<string>((a, b) => $"{a}-{b}").To("B")
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
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t => t.If((_, _) => false).WithMappedParameter<string>((a, b) => $"{a}-{b}").To("B")
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
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>(
                                        (Func<int, string, CancellationToken, Task<string>>)(
                                            async (a, b, _) => await Task.FromResult($"{a}-{b}")
                                        )
                                    )
                                    .To("B")
                        )
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
    public async Task Transition_WithMappedParameter_WithValueTaskMapping_ShouldTransformParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>(
                                        (Func<int, string, CancellationToken, ValueTask<string>>)(
                                            (a, b, _) => new ValueTask<string>($"{a}-{b}")
                                        )
                                    )
                                    .To("B")
                        )
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
    public async Task Transition_WithMappedParameter_ToSameState_ShouldTransitionToSameState()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("Loop", t => t.WithMappedParameter<string>((a, b) => $"{a}-{b}").ToSameState())
            )
            .WithState("A", state => state.WithParameter<string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Loop", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        onEntry.Received(1).Invoke("42-hello");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenTaskConditionOnMappedIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>((a, b) => $"{a}:{b}")
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
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>((a, b) => $"{a}:{b}")
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
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>((a, b) => $"{a}:{b}")
                                    .If((_, _) => new ValueTask<bool>(true))
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
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>((a, b) => $"{a}:{b}")
                                    .If((_, _) => new ValueTask<bool>(false))
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
