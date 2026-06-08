using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public partial class OnTransitionInvertedTransitionTests
{
    [Fact]
    public async Task OnTransition_T_WhenCalledOnInvertedTransition_ShouldReceiveNextParameter()
    {
        // Arrange
        var handler = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To D", t => t.From().AllOtherStates().OnTransition(handler))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42);
    }

    [Fact]
    public async Task OnTransition_T_Async_WhenCalledOnInvertedTransition_ShouldReceiveNextParameter()
    {
        // Arrange
        var received = 0;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To D",
                            t =>
                                t.From()
                                    .AllOtherStates()
                                    .OnTransition(
                                        (value, _) =>
                                        {
                                            received = value;
                                            return Task.CompletedTask;
                                        }
                                    )
                        )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, received);
    }

    [Fact]
    public async Task OnTransition_T_ValueTaskAsync_WhenCalledOnInvertedTransition_ShouldReceiveNextParameter()
    {
        // Arrange
        var received = 0;
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To D",
                            t =>
                                t.From()
                                    .AllOtherStates()
                                    .OnTransition(
                                        (value, _) =>
                                        {
                                            received = value;
                                            return ValueTask.CompletedTask;
                                        }
                                    )
                        )
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, received);
    }

    [Fact]
    public async Task OnTransition_T_WhenMultipleRegisteredOnInvertedTransition_ShouldInvokeInRegistrationOrder()
    {
        // Arrange
        var first = Substitute.For<Action<int>>();
        var second = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To D", t => t.From().AllOtherStates().OnTransition(first).OnTransition(second))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            first.Received(1).Invoke(42);
            second.Received(1).Invoke(42);
        });
    }

    [Fact]
    public async Task OnTransition_T_WithExcludedState_WhenCalledOnInvertedTransition_ShouldReceiveNextParameter()
    {
        // Arrange
        var handler = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState(
                "D",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To D", t => t.From().AllOtherStates().Except("B").OnTransition(handler))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, TestContext.Current.CancellationToken);

        // Assert
        handler.Received(1).Invoke(42);
    }
}
