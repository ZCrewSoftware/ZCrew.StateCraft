using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class WithMappedParameterTests_4_2
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
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
        var mappingFunction = Substitute.For<Func<int, string, bool, double, (int, string)>>();
        mappingFunction
            .Invoke(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<double>())
            .Returns(x => (x.ArgAt<int>(0), x.ArgAt<string>(1)));

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition("To B", t => t.WithMappedParameters(mappingFunction).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>())
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
        var onEntry = Substitute.For<Action<int, string>>();
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldTransformParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string>>();
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a * 2, b + "!")).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(84, "hello!");
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
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
        var onEntry = Substitute.For<Action<int, string>>();
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallOnStateChangeForNextStateWithMappedParameter()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, int, string>>();
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", 42, "hello");
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
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
        var invoke = Substitute.For<Action<int, string>>();
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>().WithAction(action => action.Invoke(invoke)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Transition_WithMappedParameter_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onStateChangeState = Substitute.For<Action<string, string, string, int, string>>();
        var onEntry = Substitute.For<Action<int, string>>();
        var invoke = Substitute.For<Action<int, string>>();
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).To("B")
                        )
            )
            .WithState(
                "B",
                state =>
                    state
                        .WithParameters<int, string>()
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
            onStateChangeState.Received(1).Invoke("A", "To B", "B", 42, "hello");
            onEntry.Received(1).Invoke(42, "hello");
            invoke.Received(1).Invoke(42, "hello");
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).If((_, _) => true).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).If((_, _) => false).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
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
                                    .WithMappedParameters<int, string>((a, b, _, _) => (a, b))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
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
                                    .WithMappedParameters<int, string>((a, b, _, _) => (a, b))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_WithTaskMapping_ShouldTransformParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string>>();
        Func<int, string, bool, double, CancellationToken, Task<(int, string)>> mapping = async (a, b, _, _, _) =>
            await Task.FromResult((a * 2, b + "!"));
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition("To B", t => t.WithMappedParameters<int, string>(mapping).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(84, "hello!");
    }

    [Fact]
    public async Task Transition_WithMappedParameters_WithValueTaskMapping_ShouldTransformParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string>>();
        Func<int, string, bool, double, CancellationToken, ValueTask<(int, string)>> mapping = (a, b, _, _, _) =>
            new ValueTask<(int, string)>((a * 2, b + "!"));
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition("To B", t => t.WithMappedParameters<int, string>(mapping).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(84, "hello!");
    }

    [Fact]
    public async Task Transition_WithMappedParameters_ToSameState_ShouldTransitionToSameState()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string>>();
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
                            t => t.WithMappedParameters<int, string>((a, b, _, _) => (a, b)).ToSameState()
                        )
            )
            .WithState("A", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Loop", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        onEntry.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Transition_WithMappedParameters_WhenTaskConditionOnMappedIsTrue_ShouldTransition()
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
                                t.WithMappedParameters<int, string>((a, b, _, _) => (a, b))
                                    .If((_, _, _) => Task.FromResult(true))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_WhenTaskConditionOnMappedIsFalse_ShouldThrow()
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
                                t.WithMappedParameters<int, string>((a, b, _, _) => (a, b))
                                    .If((_, _, _) => Task.FromResult(false))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_WhenValueTaskConditionOnMappedIsTrue_ShouldTransition()
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
                                t.WithMappedParameters<int, string>((a, b, _, _) => (a, b))
                                    .If((_, _, _) => ValueTask.FromResult(true))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_WhenValueTaskConditionOnMappedIsFalse_ShouldThrow()
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
                                t.WithMappedParameters<int, string>((a, b, _, _) => (a, b))
                                    .If((_, _, _) => ValueTask.FromResult(false))
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }
}
