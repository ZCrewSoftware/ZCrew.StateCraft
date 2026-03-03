namespace ZCrew.StateCraft.UnitTests.StateMachines;

public class DoubleDisposeTests
{
    [Fact]
    public async Task Dispose_WhenCalledTwice_ShouldNotThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        stateMachine.Dispose();
        var disposeAgain = () => stateMachine.Dispose();

        // Assert
        disposeAgain();
    }

    [Fact(Timeout = 5000)]
    public async Task Dispose_WhenCalledTwiceWithActiveAction_ShouldNotThrow()
    {
        // Arrange
        var actionStarted = new TaskCompletionSource();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithAsynchronousActions()
            .WithInitialState("A")
            .WithState("A", state => state.WithAction(a => a.Invoke(Action)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await actionStarted.Task;

        // Act
        stateMachine.Dispose();
        var disposeAgain = () => stateMachine.Dispose();

        // Assert
        disposeAgain();

        return;

        async Task Action(CancellationToken token)
        {
            actionStarted.SetResult();
            await Task.Delay(Timeout.Infinite, token);
        }
    }

    [Fact]
    public void Dispose_WhenCalledWithoutActivation_ShouldNotThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        // Act
        var dispose = () => stateMachine.Dispose();

        // Assert
        dispose();
    }
}
