using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public partial class ActivateTests
{
    [Fact]
    public async Task Activate_T1_T2_WhenParameterizedStateWithoutParameter_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithParameters<int, string>())
            .Build();

        // Act
        var activate = () => stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(activate);
    }

    [Fact]
    public async Task Activate_T1_T2_WhenParameterlessStateWithParameter_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithNoParameters())
            .Build();

        // Act
        var activate = () => stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(activate);
    }

    [Fact]
    public async Task Activate_T1_T2_WhenCalled_ShouldCallOnActivateWithParameters()
    {
        // Arrange
        var onActivate = Substitute.For<Action<string, int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onActivate.Received(1).Invoke("A", 42, "hello");
    }

    [Fact]
    public async Task Activate_T1_T2_WhenCalled_ShouldCallOnEntryWithParameters()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Activate_T1_T2_WhenCalled_ShouldCallActionWithParameters()
    {
        // Arrange
        var invoke = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().WithAction(action => action.Invoke(invoke)))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Activate_T1_T2_WhenCalled_ShouldCallOnActivateBeforeOnEntry()
    {
        // Arrange
        var onActivate = Substitute.For<Action<string, int, string>>();
        var onEntry = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnActivate(onActivate).OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onActivate.Received(1).Invoke("A", 42, "hello");
            onEntry.Received(1).Invoke(42, "hello");
        });
    }

    [Fact]
    public async Task Activate_T1_T2_WithAsyncOnActivateHandler_ShouldCallHandler()
    {
        // Arrange
        var onActivate = Substitute.For<Func<string, int, string, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onActivate.Received(1).Invoke("A", 42, "hello", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T1_T2_WithValueTaskOnActivateHandler_ShouldCallHandler()
    {
        // Arrange
        var onActivate = Substitute.For<Func<string, int, string, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onActivate.Received(1).Invoke("A", 42, "hello", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T1_T2_WithAsyncOnEntryHandler_ShouldCallHandler()
    {
        // Arrange
        var onEntry = Substitute.For<Func<int, string, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onEntry.Received(1).Invoke(42, "hello", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T1_T2_WithValueTaskOnEntryHandler_ShouldCallHandler()
    {
        // Arrange
        var onEntry = Substitute.For<Func<int, string, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onEntry.Received(1).Invoke(42, "hello", Arg.Any<CancellationToken>());
    }
}
