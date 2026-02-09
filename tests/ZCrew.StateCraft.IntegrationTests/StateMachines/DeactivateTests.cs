using NSubstitute;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class DeactivateTests
{
    [Fact]
    public async Task Deactivate_WhenCalled_ShouldSetCurrentStateToNull()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(stateMachine.CurrentState);
    }

    [Fact]
    public async Task Deactivate_WhenCalled_ShouldClearProperties()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Assert.False(stateMachine.Parameters.IsPreviousSet);
        Assert.False(stateMachine.Parameters.IsCurrentSet);
        Assert.False(stateMachine.Parameters.IsNextSet);
        Assert.Null(stateMachine.CurrentState);
    }

    [Fact]
    public async Task Deactivate_WhenNotActivated_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        // Act
        var deactivate = () => stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(deactivate);
    }

    [Fact]
    public async Task Deactivate_WhenCalledTwice_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Act
        var deactivate = () => stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(deactivate);
    }

    [Fact]
    public async Task Deactivate_WhenCalled_ShouldCallOnExitForCurrentState()
    {
        // Arrange
        var onExit = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(onExit))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke();
    }

    [Fact]
    public async Task Deactivate_WhenCalled_ShouldCallOnDeactivateForCurrentState()
    {
        // Arrange
        var onDeactivate = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onDeactivate.Received(1).Invoke("A");
    }

    [Fact]
    public async Task Deactivate_WhenCalled_ShouldCallOnExitBeforeOnDeactivate()
    {
        // Arrange
        var onExit = Substitute.For<Action>();
        var onDeactivate = Substitute.For<Action<string>>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnExit(onExit).OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onExit.Received(1).Invoke();
            onDeactivate.Received(1).Invoke("A");
        });
    }

    [Fact]
    public async Task Deactivate_WhenTransitioning_ShouldNotCallOnDeactivate()
    {
        // Arrange
        var onDeactivateA = Substitute.For<Action<string>>();
        var onDeactivateB = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnDeactivate(onDeactivateA).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.OnDeactivate(onDeactivateB))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onDeactivateA.Received(0).Invoke(Arg.Any<string>());
        onDeactivateB.Received(0).Invoke(Arg.Any<string>());
    }

    [Fact]
    public async Task Deactivate_WithAsyncHandler_ShouldCallHandler()
    {
        // Arrange
        var onDeactivate = Substitute.For<Func<string, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onDeactivate.Received(1).Invoke("A", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_WithMultipleHandlers_ShouldCallAllHandlersInOrder()
    {
        // Arrange
        var handler1 = Substitute.For<Action<string>>();
        var handler2 = Substitute.For<Action<string>>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnDeactivate(handler1).OnDeactivate(handler2))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            handler1.Received(1).Invoke("A");
            handler2.Received(1).Invoke("A");
        });
    }

    [Fact]
    public async Task Deactivate_WithParameterizedState_ShouldCallOnDeactivateWithParameter()
    {
        // Arrange
        var onDeactivate = Substitute.For<Action<string, int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onDeactivate.Received(1).Invoke("A", 42);
    }

    [Fact]
    public async Task Deactivate_WithParameterizedState_ShouldCallOnExitWithParameter()
    {
        // Arrange
        var onExit = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnExit(onExit))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Deactivate_AfterTransition_ShouldCallOnDeactivateForCurrentState()
    {
        // Arrange
        var onDeactivateA = Substitute.For<Action<string>>();
        var onDeactivateB = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnDeactivate(onDeactivateA).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.OnDeactivate(onDeactivateB))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onDeactivateA.Received(0).Invoke(Arg.Any<string>());
        onDeactivateB.Received(1).Invoke("B");
    }

    [Fact]
    public async Task Deactivate_AfterTransitionToParameterizedState_ShouldCallOnDeactivateWithParameter()
    {
        // Arrange
        var onDeactivate = Substitute.For<Action<string, int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", 99, TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onDeactivate.Received(1).Invoke("B", 99);
    }

    [Fact]
    public async Task Deactivate_WhenCalled_ShouldAllowReactivation()
    {
        // Arrange
        var onActivate = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnActivate(onActivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        onActivate.Received(2).Invoke("A");
    }

    [Fact]
    public async Task Deactivate_WithValueTaskHandler_ShouldCallHandler()
    {
        // Arrange
        var onDeactivate = Substitute.For<Func<string, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onDeactivate.Received(1).Invoke("A", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_WithParameterizedTaskHandler_ShouldCallHandler()
    {
        // Arrange
        var onDeactivate = Substitute.For<Func<string, int, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onDeactivate.Received(1).Invoke("A", 42, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_WithParameterizedValueTaskHandler_ShouldCallHandler()
    {
        // Arrange
        var onDeactivate = Substitute.For<Func<string, int, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onDeactivate.Received(1).Invoke("A", 42, Arg.Any<CancellationToken>());
    }
}
