using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class TryTransitionTests_T1_T2_T3
{
    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenTransitionExists_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenTransitionExists_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenTransitionDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.TryTransition("To C", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenTransitionDoesNotExist_ShouldNotChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.TryTransition("To C", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenNotActivated_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        // Act
        var result = await stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenConditionIsTrue_ShouldReturnTrue()
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
                        t => t.WithParameters<int, string, bool>().If((_, _, _) => true).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenConditionIsFalse_ShouldReturnFalse()
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
                        t => t.WithParameters<int, string, bool>().If((_, _, _) => false).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenConditionIsFalse_ShouldNotChangeState()
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
                        t => t.WithParameters<int, string, bool>().If((_, _, _) => false).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenCalled_ShouldPassParametersToOnEntry()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello", true);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenCalled_ShouldPassParametersToAction()
    {
        // Arrange
        var invoke = Substitute.For<Action<int, string, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState(
                "B",
                state => state.WithParameters<int, string, bool>().WithAction(action => action.Invoke(invoke))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42, "hello", true);
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenTransitionDoesNotExist_ShouldNotCallOnExit()
    {
        // Arrange
        var onExit = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(onExit).WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.TryTransition("To C", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        onExit.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task TryTransition_T1_T2_T3_WhenFromParameterizedState_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState("A", state => state.WithParameter<int>().WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.TryTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }
}
