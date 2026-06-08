using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionInvertedTransitionTests
{
    [Fact]
    public async Task OnTransition_WhenCalledOnInvertedTransition_ShouldInvokeHandler()
    {
        // Arrange
        var handler = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates().OnTransition(handler)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke();
    }

    [Fact]
    public async Task OnTransition_Async_WhenCalledOnInvertedTransition_ShouldInvokeHandler()
    {
        // Arrange
        var invoked = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state.WithTransition(
                        "To D",
                        t =>
                            t.From()
                                .AllOtherStates()
                                .OnTransition(_ =>
                                {
                                    invoked = true;
                                    return Task.CompletedTask;
                                })
                    )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(invoked);
    }

    [Fact]
    public async Task OnTransition_ValueTaskAsync_WhenCalledOnInvertedTransition_ShouldInvokeHandler()
    {
        // Arrange
        var invoked = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state.WithTransition(
                        "To D",
                        t =>
                            t.From()
                                .AllOtherStates()
                                .OnTransition(_ =>
                                {
                                    invoked = true;
                                    return ValueTask.CompletedTask;
                                })
                    )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(invoked);
    }

    [Fact]
    public async Task OnTransition_WithAllStates_WhenCalledOnInvertedTransition_ShouldInvokeHandler()
    {
        // Arrange
        var handler = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllStates().OnTransition(handler)))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke();
    }

    [Fact]
    public async Task OnTransition_WhenMultipleRegisteredOnInvertedTransition_ShouldInvokeInRegistrationOrder()
    {
        // Arrange
        var first = Substitute.For<Action>();
        var second = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state.WithTransition(
                        "To D",
                        t => t.From().AllOtherStates().OnTransition(first).OnTransition(second)
                    )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            first.Received(1).Invoke();
            second.Received(1).Invoke();
        });
    }
}
