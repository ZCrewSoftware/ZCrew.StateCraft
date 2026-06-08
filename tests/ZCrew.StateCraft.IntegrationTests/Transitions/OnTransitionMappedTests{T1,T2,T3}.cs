using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionMappedTests
{
    [Fact]
    public async Task OnTransition_Mapped_T1_T2_T3_WhenCalled_ShouldReceiveMappedParameters()
    {
        // Arrange
        var handler = Substitute.For<Action<int, string, bool>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "Go",
                            t =>
                                t.WithMappedParameters<int, string, bool>(x => (x, x.ToString(), true))
                                    .OnTransition(handler)
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42, "42", true);
    }

    [Fact]
    public async Task OnTransition_Mapped_T1_T2_T3_Async_WhenCalled_ShouldReceiveMappedParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var receivedFlag = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "Go",
                            t =>
                                t.WithMappedParameters<int, string, bool>(x => (x, x.ToString(), true))
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
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("42", receivedText);
        Assert.True(receivedFlag);
    }

    [Fact]
    public async Task OnTransition_Mapped_T1_T2_T3_ValueTaskAsync_WhenCalled_ShouldReceiveMappedParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var receivedFlag = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "Go",
                            t =>
                                t.WithMappedParameters<int, string, bool>(x => (x, x.ToString(), true))
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
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("42", receivedText);
        Assert.True(receivedFlag);
    }
}
