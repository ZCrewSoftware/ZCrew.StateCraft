using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Actions;

public class ParameterizedActionTests
{
    [Fact]
    public async Task Activate_WhenInitialStateHasParameterizedAction_ShouldPassParameter()
    {
        // Arrange
        var invoke = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithAction(a => a.Invoke(invoke)))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Activate_WhenInitialStateHasMultiParameterAction_ShouldPassAllParameters()
    {
        // Arrange
        var invoke = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>().WithAction(a => a.Invoke(invoke)))
            .Build();

        // Act
        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Assert
        invoke.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task WithAction_WhenMultipleActionsConfigured_ShouldInvokeAllWithParameters()
    {
        // Arrange
        var invoke1 = Substitute.For<Action<int>>();
        var invoke2 = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState(
                "B",
                state =>
                    state
                        .WithParameter<int>()
                        .WithAction(a => a.Invoke(invoke1))
                        .WithAction(a => a.Invoke(invoke2))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", 99, TestContext.Current.CancellationToken);

        // Assert
        invoke1.Received(1).Invoke(99);
        invoke2.Received(1).Invoke(99);
    }
}
