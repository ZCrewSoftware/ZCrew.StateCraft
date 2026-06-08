using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionInvertedTransitionTests
{
    [Fact]
    public async Task OnTransition_T1_T2_T3_WhenCalledOnInvertedTransition_ShouldReceiveNextParameters()
    {
        // Arrange
        var handler = Substitute.For<Action<int, string, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameters<int, string, bool>()
                        .WithTransition("To D", t => t.From().AllOtherStates().OnTransition(handler))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, "x", true, TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42, "x", true);
    }

    [Fact]
    public async Task OnTransition_T1_T2_T3_Async_WhenCalledOnInvertedTransition_ShouldReceiveNextParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var receivedFlag = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameters<int, string, bool>()
                        .WithTransition(
                            "To D",
                            t =>
                                t.From()
                                    .AllOtherStates()
                                    .OnTransition(
                                        (number, text, flag, _) =>
                                        {
                                            receivedNumber = number;
                                            receivedText = text;
                                            receivedFlag = flag;
                                            return Task.CompletedTask;
                                        }
                                    )
                        )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, "x", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("x", receivedText);
        Assert.True(receivedFlag);
    }

    [Fact]
    public async Task OnTransition_T1_T2_T3_ValueTaskAsync_WhenCalledOnInvertedTransition_ShouldReceiveNextParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var receivedFlag = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameters<int, string, bool>()
                        .WithTransition(
                            "To D",
                            t =>
                                t.From()
                                    .AllOtherStates()
                                    .OnTransition(
                                        (number, text, flag, _) =>
                                        {
                                            receivedNumber = number;
                                            receivedText = text;
                                            receivedFlag = flag;
                                            return ValueTask.CompletedTask;
                                        }
                                    )
                        )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, "x", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("x", receivedText);
        Assert.True(receivedFlag);
    }
}
