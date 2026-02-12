using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class DeactivateTests_T1_T2_T3_T4
{
    [Fact]
    public async Task Deactivate_T1_T2_T3_T4_WhenCalled_ShouldCallOnDeactivateWithParameters()
    {
        // Arrange
        var onDeactivate = Substitute.For<Action<string, int, string, double, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onDeactivate.Received(1).Invoke("A", 42, "hello", 3.14, true);
    }

    [Fact]
    public async Task Deactivate_T1_T2_T3_T4_WhenCalled_ShouldCallOnExitWithParameters()
    {
        // Arrange
        var onExit = Substitute.For<Action<int, string, double, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnExit(onExit))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42, "hello", 3.14, true);
    }

    [Fact]
    public async Task Deactivate_T1_T2_T3_T4_WhenCalled_ShouldCallOnExitBeforeOnDeactivate()
    {
        // Arrange
        var onExit = Substitute.For<Action<int, string, double, bool>>();
        var onDeactivate = Substitute.For<Action<string, int, string, double, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState(
                "A",
                state => state.WithParameters<int, string, double, bool>().OnExit(onExit).OnDeactivate(onDeactivate)
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onExit.Received(1).Invoke(42, "hello", 3.14, true);
            onDeactivate.Received(1).Invoke("A", 42, "hello", 3.14, true);
        });
    }

    [Fact]
    public async Task Deactivate_T1_T2_T3_T4_WithAsyncOnDeactivateHandler_ShouldCallHandler()
    {
        // Arrange
        var onDeactivate = Substitute.For<Func<string, int, string, double, bool, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onDeactivate.Received(1).Invoke("A", 42, "hello", 3.14, true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_T1_T2_T3_T4_WithValueTaskOnDeactivateHandler_ShouldCallHandler()
    {
        // Arrange
        var onDeactivate = Substitute.For<Func<string, int, string, double, bool, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onDeactivate.Received(1).Invoke("A", 42, "hello", 3.14, true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_T1_T2_T3_T4_WithAsyncOnExitHandler_ShouldCallHandler()
    {
        // Arrange
        var onExit = Substitute.For<Func<int, string, double, bool, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnExit(onExit))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onExit.Received(1).Invoke(42, "hello", 3.14, true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_T1_T2_T3_T4_WithValueTaskOnExitHandler_ShouldCallHandler()
    {
        // Arrange
        var onExit = Substitute.For<Func<int, string, double, bool, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnExit(onExit))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        await onExit.Received(1).Invoke(42, "hello", 3.14, true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deactivate_T1_T2_T3_T4_AfterTransition_ShouldCallOnDeactivateForCurrentState()
    {
        // Arrange
        var onDeactivate = Substitute.For<Action<string, int, string, double, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, double, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, double, bool>().OnDeactivate(onDeactivate))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", 42, "hello", 3.14, true, TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Deactivate(TestContext.Current.CancellationToken);

        // Assert
        onDeactivate.Received(1).Invoke("B", 42, "hello", 3.14, true);
    }
}
