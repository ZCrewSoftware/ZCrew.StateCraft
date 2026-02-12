using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class DeactivateTests_T1_T2
{
    [Fact]
    public async Task Deactivate_T1_T2_WhenCalled_ShouldCallOnDeactivateWithParameters()
    {
        // Arrange
        var onDeactivate = Substitute.For<Action<string, int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onDeactivate.Received(1).Invoke("A", 42, "hello");
    }

    [Fact]
    public async Task Deactivate_T1_T2_WhenCalled_ShouldCallOnExitWithParameters()
    {
        // Arrange
        var onExit = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnExit(onExit))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Deactivate_T1_T2_WhenCalled_ShouldCallOnExitBeforeOnDeactivate()
    {
        // Arrange
        var onExit = Substitute.For<Action<int, string>>();
        var onDeactivate = Substitute.For<Action<string, int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnExit(onExit).OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onExit.Received(1).Invoke(42, "hello");
            onDeactivate.Received(1).Invoke("A", 42, "hello");
        });
    }

    [Fact]
    public async Task Deactivate_T1_T2_WithAsyncOnDeactivateHandler_ShouldCallHandler()
    {
        // Arrange
        var onDeactivate = Substitute.For<Func<string, int, string, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onDeactivate.Received(1).Invoke("A", 42, "hello", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_T1_T2_WithValueTaskOnDeactivateHandler_ShouldCallHandler()
    {
        // Arrange
        var onDeactivate = Substitute.For<Func<string, int, string, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onDeactivate.Received(1).Invoke("A", 42, "hello", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_T1_T2_WithAsyncOnExitHandler_ShouldCallHandler()
    {
        // Arrange
        var onExit = Substitute.For<Func<int, string, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnExit(onExit))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onExit.Received(1).Invoke(42, "hello", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_T1_T2_WithValueTaskOnExitHandler_ShouldCallHandler()
    {
        // Arrange
        var onExit = Substitute.For<Func<int, string, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().OnExit(onExit))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onExit.Received(1).Invoke(42, "hello", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_T1_T2_AfterTransition_ShouldCallOnDeactivateForCurrentState()
    {
        // Arrange
        var onDeactivate = Substitute.For<Action<string, int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", 42, "hello", TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onDeactivate.Received(1).Invoke("B", 42, "hello");
    }
}
