using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class WithMappedParameterTests_2_3
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
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
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
        var mappingFunction = Substitute.For<Func<int, string, (int, string, bool)>>();
        mappingFunction
            .Invoke(Arg.Any<int>(), Arg.Any<string>())
            .Returns(x => (x.ArgAt<int>(0), x.ArgAt<string>(1), x.ArgAt<int>(0) > 0));

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithMappedParameters(mappingFunction).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
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
        var onEntry = Substitute.For<Action<int, string, bool>>();
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
                            t => t.WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello", true);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldTransformParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string, bool>>();
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
                            t => t.WithMappedParameters<int, string, bool>((a, b) => (a * 2, b + "!", a < 0)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(84, "hello!", false);
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
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
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
        var onEntry = Substitute.For<Action<int, string, bool>>();
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
                            t => t.WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello", true);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallOnStateChangeForNextStateWithMappedParameter()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, int, string, bool>>();
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
                            t => t.WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", 42, "hello", true);
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
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
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
        var invoke = Substitute.For<Action<int, string, bool>>();
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
                            t => t.WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0)).To("B")
                        )
            )
            .WithState(
                "B",
                state => state.WithParameters<int, string, bool>().WithAction(action => action.Invoke(invoke))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42, "hello", true);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onStateChangeState = Substitute.For<Action<string, string, string, int, string, bool>>();
        var onEntry = Substitute.For<Action<int, string, bool>>();
        var invoke = Substitute.For<Action<int, string, bool>>();
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
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0)).To("B")
                        )
            )
            .WithState(
                "B",
                state =>
                    state
                        .WithParameters<int, string, bool>()
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
            onStateChangeState.Received(1).Invoke("A", "To B", "B", 42, "hello", true);
            onEntry.Received(1).Invoke(42, "hello", true);
            invoke.Received(1).Invoke(42, "hello", true);
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
                            t =>
                                t.WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0))
                                    .If((_, _, _) => true)
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
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
                            t =>
                                t.WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0))
                                    .If((_, _, _) => false)
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
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
                            t =>
                                t.If((_, _) => true)
                                    .WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
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
                            t =>
                                t.If((_, _) => false)
                                    .WithMappedParameters<int, string, bool>((a, b) => (a, b, a > 0))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }
}
