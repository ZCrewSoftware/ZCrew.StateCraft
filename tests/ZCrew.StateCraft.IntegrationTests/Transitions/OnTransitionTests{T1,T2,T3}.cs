using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionTests
{
    [Fact]
    public async Task OnTransition_T1_T2_T3_WhenCalled_ShouldReceiveNextParameters()
    {
        // Arrange
        var handler = Substitute.For<Action<int, string, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.WithParameters<int, string, bool>().OnTransition(handler).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "x", true, TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42, "x", true);
    }

    [Fact]
    public async Task OnTransition_T1_T2_T3_Async_WhenCalled_ShouldReceiveNextParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var receivedFlag = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameters<int, string, bool>()
                                .OnTransition(
                                    (number, text, flag, _) =>
                                    {
                                        receivedNumber = number;
                                        receivedText = text;
                                        receivedFlag = flag;
                                        return Task.CompletedTask;
                                    }
                                )
                                .To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "x", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("x", receivedText);
        Assert.True(receivedFlag);
    }

    [Fact]
    public async Task OnTransition_T1_T2_T3_ValueTaskAsync_WhenCalled_ShouldReceiveNextParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var receivedFlag = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameters<int, string, bool>()
                                .OnTransition(
                                    (number, text, flag, _) =>
                                    {
                                        receivedNumber = number;
                                        receivedText = text;
                                        receivedFlag = flag;
                                        return ValueTask.CompletedTask;
                                    }
                                )
                                .To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, "x", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("x", receivedText);
        Assert.True(receivedFlag);
    }
}
