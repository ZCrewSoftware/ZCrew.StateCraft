using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionTests
{
    [Fact]
    public async Task OnTransition_WhenCalled_ShouldInvokeHandler()
    {
        // Arrange
        var handler = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.WithTransition("To B", t => t.WithNoParameters().OnTransition(handler).To("B"))
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke();
    }

    [Fact]
    public async Task OnTransition_Async_WhenCalled_ShouldInvokeHandler()
    {
        // Arrange
        var invoked = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithNoParameters()
                                .OnTransition(_ =>
                                {
                                    invoked = true;
                                    return Task.CompletedTask;
                                })
                                .To("B")
                    )
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(invoked);
    }

    [Fact]
    public async Task OnTransition_ValueTaskAsync_WhenCalled_ShouldInvokeHandler()
    {
        // Arrange
        var invoked = false;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithNoParameters()
                                .OnTransition(_ =>
                                {
                                    invoked = true;
                                    return ValueTask.CompletedTask;
                                })
                                .To("B")
                    )
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(invoked);
    }

    [Fact]
    public async Task OnTransition_WhenMultipleRegistered_ShouldInvokeInRegistrationOrder()
    {
        // Arrange
        var first = Substitute.For<Action>();
        var second = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.WithNoParameters().OnTransition(first).OnTransition(second).To("B")
                    )
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            first.Received(1).Invoke();
            second.Received(1).Invoke();
        });
    }
}
