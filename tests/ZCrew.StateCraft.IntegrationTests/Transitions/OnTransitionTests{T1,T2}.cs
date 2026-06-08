using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionTests
{
    [Fact]
    public async Task OnTransition_T1_T2_WhenCalled_ShouldReceiveNextParameters()
    {
        // Arrange
        var handler = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition("To B", t => t.WithParameters<int, string>().OnTransition(handler).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "x", TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42, "x");
    }

    [Fact]
    public async Task OnTransition_T1_T2_Async_WhenCalled_ShouldReceiveNextParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameters<int, string>()
                                .OnTransition(
                                    (number, text, _) =>
                                    {
                                        receivedNumber = number;
                                        receivedText = text;
                                        return Task.CompletedTask;
                                    }
                                )
                                .To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "x", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("x", receivedText);
    }

    [Fact]
    public async Task OnTransition_T1_T2_ValueTaskAsync_WhenCalled_ShouldReceiveNextParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameters<int, string>()
                                .OnTransition(
                                    (number, text, _) =>
                                    {
                                        receivedNumber = number;
                                        receivedText = text;
                                        return ValueTask.CompletedTask;
                                    }
                                )
                                .To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "x", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("x", receivedText);
    }
}
