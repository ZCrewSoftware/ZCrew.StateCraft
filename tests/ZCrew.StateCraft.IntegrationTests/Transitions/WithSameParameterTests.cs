using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class WithSameParameterTests
{
    [Fact]
    public async Task Transition_WithSameParameter_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithSameParameter_ShouldPreserveParameterValue()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_WithSameParameter_ShouldCallOnExitWithParameter()
    {
        // Arrange
        var onExit = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnExit(onExit).WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_WithSameParameter_ShouldCallOnEntryWithSameParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_WithSameParameter_ShouldCallOnStateChangeWithSameParameter()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", 42);
    }

    [Fact]
    public async Task Transition_WithSameParameter_ShouldCallActionWithSameParameter()
    {
        // Arrange
        var invoke = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>().WithAction(action => action.Invoke(invoke)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_WithSameParameter_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onStateChangeState = Substitute.For<Action<string, string, string, int>>();
        var onEntry = Substitute.For<Action<int>>();
        var invoke = Substitute.For<Action<int>>();
        var onExit = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .OnStateChange(onStateChangeMachine)
            .WithState("A", state => state.WithParameter<int>().OnExit(onExit).WithTransition("To B", "B"))
            .WithState(
                "B",
                state =>
                    state
                        .WithParameter<int>()
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
            onStateChangeState.Received(1).Invoke("A", "To B", "B", 42);
            onEntry.Received(1).Invoke(42);
            invoke.Received(1).Invoke(42);
        });
    }

    [Fact]
    public async Task Transition_WithSameParameter_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state.WithParameter<int>().WithTransition("To B", t => t.WithSameParameter().If(x => x > 0).To("B"))
            )
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithSameParameter_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state.WithParameter<int>().WithTransition("To B", t => t.WithSameParameter().If(_ => false).To("B"))
            )
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task CanTransition_WhenThereIsNoCondition_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_WhenConditionIsTrue_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state.WithParameter<int>().WithTransition("To B", t => t.WithSameParameter().If(x => x > 0).To("B"))
            )
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_WhenConditionIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state.WithParameter<int>().WithTransition("To B", t => t.WithSameParameter().If(x => x < 0).To("B"))
            )
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryTransition_WhenThereIsNoCondition_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TryTransition_WhenThereIsNoCondition_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }
}
