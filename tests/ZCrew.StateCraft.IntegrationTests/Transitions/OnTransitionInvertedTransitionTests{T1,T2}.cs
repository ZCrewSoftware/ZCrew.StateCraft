using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionInvertedTransitionTests
{
    [Fact]
    public async Task OnTransition_T1_T2_WhenCalledOnInvertedTransition_ShouldReceiveNextParameters()
    {
        // Arrange
        var handler = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To D", t => t.From().AllOtherStates().OnTransition(handler))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, "x", TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42, "x");
    }

    [Fact]
    public async Task OnTransition_T1_T2_Async_WhenCalledOnInvertedTransition_ShouldReceiveNextParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To D",
                            t =>
                                t.From()
                                    .AllOtherStates()
                                    .OnTransition(
                                        (number, text, _) =>
                                        {
                                            receivedNumber = number;
                                            receivedText = text;
                                            return Task.CompletedTask;
                                        }
                                    )
                        )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, "x", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("x", receivedText);
    }

    [Fact]
    public async Task OnTransition_T1_T2_ValueTaskAsync_WhenCalledOnInvertedTransition_ShouldReceiveNextParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition(
                            "To D",
                            t =>
                                t.From()
                                    .AllOtherStates()
                                    .OnTransition(
                                        (number, text, _) =>
                                        {
                                            receivedNumber = number;
                                            receivedText = text;
                                            return ValueTask.CompletedTask;
                                        }
                                    )
                        )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, "x", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("x", receivedText);
    }
}
