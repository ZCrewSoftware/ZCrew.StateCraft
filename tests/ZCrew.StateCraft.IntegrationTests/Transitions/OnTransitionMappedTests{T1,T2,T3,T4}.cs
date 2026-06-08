using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionMappedTests
{
    [Fact]
    public async Task OnTransition_Mapped_T1_T2_T3_T4_WhenCalled_ShouldReceiveMappedParameters()
    {
        // Arrange
        var handler = Substitute.For<Action<int, string, bool, double>>();
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
                                t.WithMappedParameters<int, string, bool, double>(x => (x, x.ToString(), true, 3.14))
                                    .OnTransition(handler)
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42, "42", true, 3.14);
    }

    [Fact]
    public async Task OnTransition_Mapped_T1_T2_T3_T4_Async_WhenCalled_ShouldReceiveMappedParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var receivedFlag = false;
        var receivedValue = 0d;
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
                                t.WithMappedParameters<int, string, bool, double>(x => (x, x.ToString(), true, 3.14))
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
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("42", receivedText);
        Assert.True(receivedFlag);
        Assert.Equal(3.14, receivedValue);
    }

    [Fact]
    public async Task OnTransition_Mapped_T1_T2_T3_T4_ValueTaskAsync_WhenCalled_ShouldReceiveMappedParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
        var receivedFlag = false;
        var receivedValue = 0d;
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
                                t.WithMappedParameters<int, string, bool, double>(x => (x, x.ToString(), true, 3.14))
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
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("42", receivedText);
        Assert.True(receivedFlag);
        Assert.Equal(3.14, receivedValue);
    }
}
