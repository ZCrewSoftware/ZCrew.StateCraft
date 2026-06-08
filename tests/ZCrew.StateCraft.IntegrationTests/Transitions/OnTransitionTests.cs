using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class OnTransitionTests
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

    [Fact]
    public async Task OnTransition_T_WhenCalled_ShouldReceiveNextParameter()
    {
        // Arrange
        var handler = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.WithTransition("To B", t => t.WithParameter<int>().OnTransition(handler).To("B"))
            )
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42);
    }

    [Fact]
    public async Task OnTransition_Mapped_WhenCalled_ShouldReceiveMappedParameter()
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
}
