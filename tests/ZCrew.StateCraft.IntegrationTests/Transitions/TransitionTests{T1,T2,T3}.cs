using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class TransitionTests_T1_T2_T3
{
    [Fact]
    public async Task Transition_T1_T2_T3_WhenCalled_ShouldChangeState()
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
        await stateMachine.Transition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenCalled_ShouldPassParametersToOnEntry()
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
        await stateMachine.Transition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello", true);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenCalled_ShouldPassParametersToAction()
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
        await stateMachine.Transition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42, "hello", true);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenCalled_ShouldInvokeLifecycleHandlersInOrder()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onEntry = Substitute.For<Action<int, string, bool>>();
        var invoke = Substitute.For<Action<int, string, bool>>();
        var onExit = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange(onStateChangeMachine)
            .WithState("A", state => state.OnExit(onExit).WithTransition<int, string, bool>("To B", "B"))
            .WithState(
                "B",
                state =>
                    state
                        .WithParameters<int, string, bool>()
                        .OnEntry(onEntry)
                        .WithAction(action => action.Invoke(invoke))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onExit.Received(1).Invoke();
            onStateChangeMachine.Received(1).Invoke("A", "To B", "B");
            onEntry.Received(1).Invoke(42, "hello", true);
            invoke.Received(1).Invoke(42, "hello", true);
        });
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenNotActivated_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WithCondition_WhenConditionIsTrue_ShouldTransition()
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
        await stateMachine.Transition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WithCondition_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition("To B", t => t.WithParameters<int, string, bool>().If((_, _, b) => b).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("To B", 42, "hello", false, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WhenFromParameterizedState_ShouldChangeState()
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
        await stateMachine.Transition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T1_T2_T3_WithShortcut_WhenCalled_ShouldChangeState()
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
        await stateMachine.Transition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }
}
