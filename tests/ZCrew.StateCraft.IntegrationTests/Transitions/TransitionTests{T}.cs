using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class TransitionTests_T
{
    [Fact]
    public async Task Transition_T_WhenCalled_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T_WhenCalled_ShouldPassParameterToOnStateChange()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", 42);
    }

    [Fact]
    public async Task Transition_T_WhenCalled_ShouldPassParameterToOnEntry()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_T_WhenCalled_ShouldPassParameterToAction()
    {
        // Arrange
        var invoke = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>().WithAction(action => action.Invoke(invoke)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_T_WhenNotActivated_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        // Act
        var transition = () => stateMachine.Transition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_WhenFromParameterizedToParameterless_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WhenFromParameterizedToParameterless_ShouldCallOnExitWithParameter()
    {
        // Arrange
        var onExit = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnExit(onExit).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_WhenFromParameterizedToParameterless_ShouldCallOnStateChangeForNextState()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B");
    }

    [Fact]
    public async Task Transition_WhenFromParameterizedToParameterless_ShouldCallOnEntryForNextState()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke();
    }

    [Fact]
    public async Task Transition_T_WhenFromParameterizedToParameterized_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T_WhenFromParameterizedToParameterized_ShouldCallOnExitWithPreviousParameter()
    {
        // Arrange
        var onExit = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnExit(onExit).WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_T_WhenFromParameterizedToParameterized_ShouldCallOnStateChangeWithNextParameter()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", "hello");
    }

    [Fact]
    public async Task Transition_T_WhenFromParameterizedToParameterized_ShouldCallOnEntryWithNextParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke("hello");
    }

    [Fact]
    public async Task Transition_T_WhenFromParameterizedToParameterized_ShouldCallActionWithNextParameter()
    {
        // Arrange
        var invoke = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>().WithAction(action => action.Invoke(invoke)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", "hello", TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke("hello");
    }

    [Fact]
    public async Task Transition_T_WhenFromParameterizedToParameterizedSameType_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 100, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T_WhenFromParameterizedToParameterizedSameType_ShouldPassCorrectParameters()
    {
        // Arrange
        var onExitA = Substitute.For<Action<int>>();
        var onEntryB = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnExit(onExitA).WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>().OnEntry(onEntryB))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 100, TestContext.Current.CancellationToken);

        // Assert
        onExitA.Received(1).Invoke(42);
        onEntryB.Received(1).Invoke(100);
    }

    [Fact]
    public async Task Transition_T_WhenParameterIsAssignable_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<object>("To B", "B"))
            .WithState("B", state => state.WithParameter<object>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", "a string value", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_T_WhenParameterIsAssignable_ShouldPassParameterToOnEntry()
    {
        // Arrange
        var onEntry = Substitute.For<Action<object>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<object>("To B", "B"))
            .WithState("B", state => state.WithParameter<object>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", "a string value", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke("a string value");
    }

    [Fact]
    public async Task Transition_T_WhenParameterIsAssignable_ShouldPassParameterToOnStateChange()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, object>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<object>("To B", "B"))
            .WithState("B", state => state.WithParameter<object>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", "a string value", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", "a string value");
    }

    [Fact]
    public async Task Transition_T_WhenParameterIsAssignable_ShouldPassParameterToAction()
    {
        // Arrange
        var invoke = Substitute.For<Action<object>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<object>("To B", "B"))
            .WithState("B", state => state.WithParameter<object>().WithAction(action => action.Invoke(invoke)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", "a string value", TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke("a string value");
    }

    [Fact]
    public async Task Transition_WhenFromParameterizedWithAssignableType_ShouldCallOnExitWithParameter()
    {
        // Arrange
        var onExit = Substitute.For<Action<object>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<object>("To B", "B"))
            .WithState(
                "B",
                state => state.WithParameter<object>().OnExit(onExit).WithTransition("To C", t => t.To("C"))
            )
            .WithState("C", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", "stored string", TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To C", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke("stored string");
    }

    [Fact]
    public async Task Transition_T_WhenFromParameterizedToParameterizedWithAssignableTypes_ShouldPassCorrectParameters()
    {
        // Arrange
        var onExitB = Substitute.For<Action<object>>();
        var onEntryC = Substitute.For<Action<object>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<object>("To B", "B"))
            .WithState("B", state => state.WithParameter<object>().OnExit(onExitB).WithTransition<object>("To C", "C"))
            .WithState("C", state => state.WithParameter<object>().OnEntry(onEntryC))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", "first string", TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To C", "second string", TestContext.Current.CancellationToken);

        // Assert
        onExitB.Received(1).Invoke("first string");
        onEntryC.Received(1).Invoke("second string");
    }
}
