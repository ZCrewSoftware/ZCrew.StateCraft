using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionInvertedTransitionTests
{
    [Fact]
    public async Task OnTransition_T1_T2_T3_T4_WhenCalledOnInvertedTransition_ShouldReceiveNextParameters()
    {
        // Arrange
        var handler = Substitute.For<Action<int, string, bool, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition("To D", t => t.From().AllOtherStates().OnTransition(handler))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, "x", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42, "x", true, 3.14);
    }

    [Fact]
    public async Task OnTransition_T1_T2_T3_T4_Async_WhenCalledOnInvertedTransition_ShouldReceiveNextParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var receivedFlag = false;
        var receivedValue = 0d;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To D",
                            t =>
                                t.From()
                                    .AllOtherStates()
                                    .OnTransition(
                                        (number, text, flag, value, _) =>
                                        {
                                            receivedNumber = number;
                                            receivedText = text;
                                            receivedFlag = flag;
                                            receivedValue = value;
                                            return Task.CompletedTask;
                                        }
                                    )
                        )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, "x", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("x", receivedText);
        Assert.True(receivedFlag);
        Assert.Equal(3.14, receivedValue);
    }

    [Fact]
    public async Task OnTransition_T1_T2_T3_T4_ValueTaskAsync_WhenCalledOnInvertedTransition_ShouldReceiveNextParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var receivedFlag = false;
        var receivedValue = 0d;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameters<int, string, bool, double>()
                        .WithTransition(
                            "To D",
                            t =>
                                t.From()
                                    .AllOtherStates()
                                    .OnTransition(
                                        (number, text, flag, value, _) =>
                                        {
                                            receivedNumber = number;
                                            receivedText = text;
                                            receivedFlag = flag;
                                            receivedValue = value;
                                            return ValueTask.CompletedTask;
                                        }
                                    )
                        )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, "x", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("x", receivedText);
        Assert.True(receivedFlag);
        Assert.Equal(3.14, receivedValue);
    }
}
