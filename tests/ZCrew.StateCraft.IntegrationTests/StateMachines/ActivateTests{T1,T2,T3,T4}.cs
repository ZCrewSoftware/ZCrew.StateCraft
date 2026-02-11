using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public partial class ActivateTests
{
    [Fact]
    public async Task Activate_T1_T2_T3_T4_WhenParameterizedStateWithoutParameter_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithParameters<int, string, double, bool>())
            .Build();

        // Act
        var activate = () => stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(activate);
    }

    [Fact]
    public async Task Activate_T1_T2_T3_T4_WhenParameterlessStateWithParameter_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithNoParameters())
            .Build();

        // Act
        var activate = () => stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(activate);
    }

    [Fact]
    public async Task Activate_T1_T2_T3_T4_WhenCalled_ShouldCallOnActivateWithParameters()
    {
        // Arrange
        var onActivate = Substitute.For<Action<string, int, string, double, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onActivate.Received(1).Invoke("A", 42, "hello", 3.14, true);
    }

    [Fact]
    public async Task Activate_T1_T2_T3_T4_WhenCalled_ShouldCallOnEntryWithParameters()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string, double, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello", 3.14, true);
    }

    [Fact]
    public async Task Activate_T1_T2_T3_T4_WhenCalled_ShouldCallActionWithParameters()
    {
        // Arrange
        var invoke = Substitute.For<Action<int, string, double, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState(
                "A",
                state => state.WithParameters<int, string, double, bool>().WithAction(action => action.Invoke(invoke))
            )
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42, "hello", 3.14, true);
    }

    [Fact]
    public async Task Activate_T1_T2_T3_T4_WhenCalled_ShouldCallOnActivateBeforeOnEntry()
    {
        // Arrange
        var onActivate = Substitute.For<Action<string, int, string, double, bool>>();
        var onEntry = Substitute.For<Action<int, string, double, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState(
                "A",
                state => state.WithParameters<int, string, double, bool>().OnActivate(onActivate).OnEntry(onEntry)
            )
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onActivate.Received(1).Invoke("A", 42, "hello", 3.14, true);
            onEntry.Received(1).Invoke(42, "hello", 3.14, true);
        });
    }

    [Fact]
    public async Task Activate_T1_T2_T3_T4_WithAsyncOnActivateHandler_ShouldCallHandler()
    {
        // Arrange
        var onActivate = Substitute.For<Func<string, int, string, double, bool, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onActivate.Received(1).Invoke("A", 42, "hello", 3.14, true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T1_T2_T3_T4_WithValueTaskOnActivateHandler_ShouldCallHandler()
    {
        // Arrange
        var onActivate = Substitute.For<Func<string, int, string, double, bool, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnActivate(onActivate))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onActivate.Received(1).Invoke("A", 42, "hello", 3.14, true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T1_T2_T3_T4_WithAsyncOnEntryHandler_ShouldCallHandler()
    {
        // Arrange
        var onEntry = Substitute.For<Func<int, string, double, bool, CancellationToken, Task>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onEntry.Received(1).Invoke(42, "hello", 3.14, true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Activate_T1_T2_T3_T4_WithValueTaskOnEntryHandler_ShouldCallHandler()
    {
        // Arrange
        var onEntry = Substitute.For<Func<int, string, double, bool, CancellationToken, ValueTask>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14, true)
            .WithState("A", state => state.WithParameters<int, string, double, bool>().OnEntry(onEntry))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        await onEntry.Received(1).Invoke(42, "hello", 3.14, true, Arg.Any<CancellationToken>());
    }
}
