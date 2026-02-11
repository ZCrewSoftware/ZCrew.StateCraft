using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class WithMappedParameterTests_1_4
{
    [Fact]
    public async Task Transition_WithMappedParameters_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_ShouldCallMappingFunction()
    {
        // Arrange
        var mappingFunction = Substitute.For<Func<int, (int, string, bool, double)>>();
        mappingFunction
            .Invoke(Arg.Any<int>())
            .Returns(x => (x.Arg<int>(), x.Arg<int>().ToString(), x.Arg<int>() > 0, (double)x.Arg<int>()));

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameters(mappingFunction).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        mappingFunction.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_ShouldTransformParameterType()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x, $"value: {x}", x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "value: 42", true, 42.0);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_ShouldTransformParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x * 2, x.ToString(), x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(84, "42", true, 42.0);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_ShouldCallOnExitWithPreviousParameter()
    {
        // Arrange
        var onExit = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .OnExit(onExit)
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_ShouldCallOnEntryWithMappedParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "42", true, 42.0);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_ShouldCallOnStateChangeForNextStateWithMappedParameter()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", 42, "42", true, 42.0);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_ShouldCallOnStateChangeHandlersOnMachine()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .OnStateChange(onStateChange)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B");
    }

    [Fact]
    public async Task Transition_WithMappedParameters_ShouldCallActionWithMappedParameter()
    {
        // Arrange
        var invoke = Substitute.For<Action<int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
            .WithState(
                "B",
                state => state.WithParameters<int, string, bool, double>().WithAction(action => action.Invoke(invoke))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42, "42", true, 42.0);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onStateChangeState = Substitute.For<Action<string, string, string, int, string, bool, double>>();
        var onEntry = Substitute.For<Action<int, string, bool, double>>();
        var invoke = Substitute.For<Action<int, string, bool, double>>();
        var onExit = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .OnStateChange(onStateChangeMachine)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .OnExit(onExit)
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
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
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onExit.Received(1).Invoke(42);
            onStateChangeMachine.Received(1).Invoke("A", "To B", "B");
            onStateChangeState.Received(1).Invoke("A", "To B", "B", 42, "42", true, 42.0);
            onEntry.Received(1).Invoke(42, "42", true, 42.0);
            invoke.Received(1).Invoke(42, "42", true, 42.0);
        });
    }

    [Fact]
    public async Task Transition_WithMappedParameters_WhenConditionOnMappedIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .If((_, _, _, _) => true)
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_WhenConditionOnMappedIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .If((_, _, _, _) => false)
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_WhenConditionOnPreviousIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If(_ => true)
                                    .WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMappedParameters_WhenConditionOnPreviousIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.If(_ => false)
                                    .WithMappedParameters<int, string, bool, double>(x =>
                                        (x, x.ToString(), x > 0, (double)x)
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }
}
