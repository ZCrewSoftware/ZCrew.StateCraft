using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionTests
{
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
    public async Task OnTransition_T_Async_WhenCalled_ShouldReceiveNextParameter()
    {
        // Arrange
        var received = 0;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameter<int>()
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
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, received);
    }

    [Fact]
    public async Task OnTransition_T_ValueTaskAsync_WhenCalled_ShouldReceiveNextParameter()
    {
        // Arrange
        var received = 0;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t =>
                            t.WithParameter<int>()
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
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, received);
    }

    [Fact]
    public async Task OnTransition_T_WhenMultipleRegistered_ShouldInvokeInRegistrationOrder()
    {
        // Arrange
        var first = Substitute.For<Action<int>>();
        var second = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.WithParameter<int>().OnTransition(first).OnTransition(second).To("B")
                    )
            )
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            first.Received(1).Invoke(42);
            second.Received(1).Invoke(42);
        });
    }
}
