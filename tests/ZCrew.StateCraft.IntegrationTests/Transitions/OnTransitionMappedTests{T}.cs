using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionMappedTests
{
    [Fact]
    public async Task OnTransition_Mapped_T_WhenCalled_ShouldReceiveMappedParameter()
    {
        // Arrange
        var handler = Substitute.For<Action<string>>();
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
                            t => t.WithMappedParameter<string>(x => x.ToString()).OnTransition(handler).To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke("42");
    }

    [Fact]
    public async Task OnTransition_Mapped_T_Async_WhenCalled_ShouldReceiveMappedParameter()
    {
        // Arrange
        var received = string.Empty;
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
                                t.WithMappedParameter<string>(x => x.ToString())
                                    .OnTransition(
                                        (value, _) =>
                                        {
                                            received = value;
                                            return Task.CompletedTask;
                                        }
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("42", received);
    }

    [Fact]
    public async Task OnTransition_Mapped_T_ValueTaskAsync_WhenCalled_ShouldReceiveMappedParameter()
    {
        // Arrange
        var received = string.Empty;
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
                                t.WithMappedParameter<string>(x => x.ToString())
                                    .OnTransition(
                                        (value, _) =>
                                        {
                                            received = value;
                                            return ValueTask.CompletedTask;
                                        }
                                    )
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Go", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("42", received);
    }
}
