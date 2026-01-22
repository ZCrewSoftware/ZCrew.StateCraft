using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class ActivateTests
{
    [Fact]
    public async Task Activate_WhenCalled_ShouldSetStateToInitialState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Activate_WhenCalled_ShouldCallOnEntryForInitialState()
    {
        // Arrange
        var onEntry = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke();
    }

    [Fact]
    public async Task Activate_WhenCalled_ShouldCallActionForInitialState()
    {
        // Arrange
        var call = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(action => action.Invoke(call)))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        call.Received(1).Invoke();
    }

    [Fact]
    public async Task Activate_WhenCalledTwice_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var activate = () => stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(activate);
    }

    [Fact]
    public async Task Activate_WhenCalled_ShouldCallOnActivateForInitialState()
    {
        // Arrange
        var onActivate = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onActivate.Received(1).Invoke("A");
    }

    [Fact]
    public async Task Activate_WhenCalled_ShouldCallOnActivateBeforeOnEntry()
    {
        // Arrange
        var onActivate = Substitute.For<Action<string>>();
        var onEntry = Substitute.For<Action>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnActivate(onActivate).OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onActivate.Received(1).Invoke("A");
            onEntry.Received(1).Invoke();
        });
    }

    [Fact]
    public async Task Activate_WhenTransitioning_ShouldNotCallOnActivate()
    {
        // Arrange
        var onActivateA = Substitute.For<Action<string>>();
        var onActivateB = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnActivate(onActivateA).WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.OnActivate(onActivateB))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onActivateA.Received(1).Invoke("A");
        onActivateB.Received(0).Invoke(Arg.Any<string>());
    }

    [Fact]
    public async Task Activate_WithAsyncHandler_ShouldCallHandler()
    {
        // Arrange
        var onActivate = Substitute.For<Func<string, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onActivate.Received(1).Invoke("A", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_WithMultipleHandlers_ShouldCallAllHandlersInOrder()
    {
        // Arrange
        var handler1 = Substitute.For<Action<string>>();
        var handler2 = Substitute.For<Action<string>>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnActivate(handler1).OnActivate(handler2))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            handler1.Received(1).Invoke("A");
            handler2.Received(1).Invoke("A");
        });
    }

    [Fact]
    public async Task Activate_WhenParameterlessState_ShouldSucceed()
    {
        // Arrange
        var onActivate = Substitute.For<Action<string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onActivate.Received(1).Invoke("A");
    }

    [Fact]
    public async Task Activate_WhenParameterizedStateWithoutParameter_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithParameter<int>())
            .Build();

        // Act
        var activate = () => stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(activate);
    }

    [Fact]
    public async Task Activate_WhenParameterlessStateWithParameter_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithNoParameters())
            .Build();

        // Act
        var activate = () => stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(activate);
    }
}
