using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionMappedTests
{
    [Fact]
    public async Task OnTransition_Mapped_T1_T2_WhenCalled_ShouldReceiveMappedParameters()
    {
        // Arrange
        var handler = Substitute.For<Action<int, string>>();
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
                                t.WithMappedParameters<int, string>(x => (x, x.ToString()))
                                    .OnTransition(handler)
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42, "42");
    }

    [Fact]
    public async Task OnTransition_Mapped_T1_T2_Async_WhenCalled_ShouldReceiveMappedParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
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
                                t.WithMappedParameters<int, string>(x => (x, x.ToString()))
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
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("42", receivedText);
    }

    [Fact]
    public async Task OnTransition_Mapped_T1_T2_ValueTaskAsync_WhenCalled_ShouldReceiveMappedParameters()
    {
        // Arrange
        var receivedNumber = 0;
        var receivedText = string.Empty;
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
                                t.WithMappedParameters<int, string>(x => (x, x.ToString()))
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
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, receivedNumber);
        Assert.Equal("42", receivedText);
    }
}
