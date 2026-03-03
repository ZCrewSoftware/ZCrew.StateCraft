using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class DisposeLifecycleTests
{
    [Fact]
    public async Task DisposeAsync_WhenActivated_ShouldCallDeactivateHandlers()
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
        await stateMachine.DisposeAsync();

        // Assert
        onDeactivate.Received(1).Invoke("A");
    }

    [Fact]
    public async Task DisposeAsync_WhenNotActivated_ShouldNotThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        // Act
        var disposeAsync = async () => await stateMachine.DisposeAsync();

        // Assert
        await disposeAsync();
    }

    [Fact]
    public async Task DisposeAsync_WhenDeactivateThrows_ShouldNotPropagate()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.OnExit(() => throw new InvalidOperationException("Exit failure"))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var disposeAsync = async () => await stateMachine.DisposeAsync();

        // Assert
        await disposeAsync();
    }
}
