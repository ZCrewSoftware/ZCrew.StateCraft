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
}
