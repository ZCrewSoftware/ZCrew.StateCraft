using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class BuildInstanceIndependenceTests
{
    [Fact]
    public async Task Build_WhenHandlerAddedAfterBuild_ShouldNotAffectPreviouslyBuiltInstance()
    {
        // Arrange
        var handler = Substitute.For<Action<string, string, string>>();

        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state);

        var machine1 = configuration.Build();

        // Add handler AFTER machine1 was built — should not affect machine1
        configuration.OnStateChange(handler);

        // Act
        await machine1.Activate(TestContext.Current.CancellationToken);
        await machine1.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        handler
            .DidNotReceive()
            .Invoke(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Build_WhenCalledTwice_ShouldProduceIndependentOnStateChangeHandlers()
    {
        // Arrange
        var handler1Count = 0;
        var handler2Count = 0;

        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .OnStateChange((_, _, _) => handler1Count++);

        var machine1 = configuration.Build();

        configuration.OnStateChange((_, _, _) => handler2Count++);

        var machine2 = configuration.Build();

        // Act — only transition machine1
        await machine1.Activate(TestContext.Current.CancellationToken);
        await machine1.Transition("To B", TestContext.Current.CancellationToken);

        // Assert — machine1 should only have handler1 (not handler2)
        Assert.True(handler1Count > 0, "Handler1 should have been invoked on machine1");
        Assert.Equal(0, handler2Count);
    }

    [Fact]
    public async Task Build_WhenCalledTwice_ShouldNotShareMutableState()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state.WithTransition("To A", t => t.To("A")));

        var machine1 = configuration.Build();
        var machine2 = configuration.Build();

        // Act
        await machine1.Activate(TestContext.Current.CancellationToken);
        await machine1.Transition("To B", TestContext.Current.CancellationToken);

        await machine2.Activate(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("B", machine1.CurrentState?.StateValue);
        Assert.Equal("A", machine2.CurrentState?.StateValue);
    }
}
