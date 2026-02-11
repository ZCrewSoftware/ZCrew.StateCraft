using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public partial class ActivateTests
{
    [Fact]
    public async Task Activate_T_WhenParameterizedStateWithoutParameter_ShouldThrow()
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
    public async Task Activate_T_WhenParameterlessStateWithParameter_ShouldThrow()
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

    [Fact]
    public async Task Activate_T_WhenCalled_ShouldCallOnActivateWithParameter()
    {
        // Arrange
        var onActivate = Substitute.For<Action<string, int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onActivate.Received(1).Invoke("A", 42);
    }

    [Fact]
    public async Task Activate_T_WhenCalled_ShouldCallOnEntryWithParameter()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Activate_T_WhenCalled_ShouldCallActionWithParameter()
    {
        // Arrange
        var invoke = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithAction(action => action.Invoke(invoke)))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Activate_T_WhenCalled_ShouldCallOnActivateBeforeOnEntry()
    {
        // Arrange
        var onActivate = Substitute.For<Action<string, int>>();
        var onEntry = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnActivate(onActivate).OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onActivate.Received(1).Invoke("A", 42);
            onEntry.Received(1).Invoke(42);
        });
    }

    [Fact]
    public async Task Activate_T_WithAsyncOnActivateHandler_ShouldCallHandler()
    {
        // Arrange
        var onActivate = Substitute.For<Func<string, int, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onActivate.Received(1).Invoke("A", 42, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T_WithValueTaskOnActivateHandler_ShouldCallHandler()
    {
        // Arrange
        var onActivate = Substitute.For<Func<string, int, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onActivate.Received(1).Invoke("A", 42, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T_WithAsyncOnEntryHandler_ShouldCallHandler()
    {
        // Arrange
        var onEntry = Substitute.For<Func<int, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onEntry.Received(1).Invoke(42, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T_WithValueTaskOnEntryHandler_ShouldCallHandler()
    {
        // Arrange
        var onEntry = Substitute.For<Func<int, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onEntry.Received(1).Invoke(42, Arg.Any<CancellationToken>());
    }
}
