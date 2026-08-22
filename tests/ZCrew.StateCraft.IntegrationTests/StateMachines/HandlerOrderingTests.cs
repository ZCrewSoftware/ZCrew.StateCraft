using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class HandlerOrderingTests
{
    [Fact]
    public async Task OnEntry_WhenMultipleHandlersRegistered_ShouldExecuteInRegistrationOrder()
    {
        // Arrange
        var handler1 = Substitute.For<Action>();
        var handler2 = Substitute.For<Action>();
        var handler3 = Substitute.For<Action>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState(
                "B",
                state => state.OnEntry(handler1).OnEntry(handler2).OnEntry(handler3)
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            handler1.Received(1).Invoke();
            handler2.Received(1).Invoke();
            handler3.Received(1).Invoke();
        });
    }

    [Fact]
    public async Task OnExit_WhenMultipleHandlersRegistered_ShouldExecuteInRegistrationOrder()
    {
        // Arrange
        var handler1 = Substitute.For<Action>();
        var handler2 = Substitute.For<Action>();
        var handler3 = Substitute.For<Action>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .OnExit(handler1)
                        .OnExit(handler2)
                        .OnExit(handler3)
                        .WithTransition("To B", t => t.To("B"))
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            handler1.Received(1).Invoke();
            handler2.Received(1).Invoke();
            handler3.Received(1).Invoke();
        });
    }

    [Fact]
    public async Task OnStateChange_WhenMultipleHandlersOnMachine_ShouldExecuteInRegistrationOrder()
    {
        // Arrange
        var handler1 = Substitute.For<Action<string, string, string>>();
        var handler2 = Substitute.For<Action<string, string, string>>();

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .OnStateChange(handler1)
            .OnStateChange(handler2)
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            handler1.Received(1).Invoke("A", "To B", "B");
            handler2.Received(1).Invoke("A", "To B", "B");
        });
    }
}
