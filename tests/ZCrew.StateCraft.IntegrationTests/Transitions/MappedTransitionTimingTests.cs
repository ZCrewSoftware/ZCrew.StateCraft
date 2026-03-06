using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class MappedTransitionTimingTests
{
    [Fact]
    public async Task Transition_WithMappedParameter_ShouldInvokeMappingBeforeOnExit()
    {
        // Arrange
        var onExit = Substitute.For<Action<int>>();
        var mapping = Substitute.For<Func<int, string>>();
        mapping.Invoke(Arg.Any<int>()).Returns("mapped");

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .OnExit(onExit)
                        .WithTransition("To B", t => t.WithMappedParameter<string>(mapping).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert — mapping runs during condition evaluation, before OnExit
        Received.InOrder(() =>
        {
            mapping.Received(1).Invoke(42);
            onExit.Received(1).Invoke(42);
        });
    }

    [Fact]
    public async Task CanTransition_WithMappedParameter_ShouldInvokeMapping()
    {
        // Arrange
        var mapping = Substitute.For<Func<int, string>>();
        mapping.Invoke(Arg.Any<int>()).Returns("mapped");

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(mapping).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        mapping.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_WithMappedParameter_WhenInitialConditionFalse_ShouldNotInvokeMapping()
    {
        // Arrange
        var mapping = Substitute.For<Func<int, string>>();
        mapping.Invoke(Arg.Any<int>()).Returns("mapped");

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.If(_ => false).WithMappedParameter<string>(mapping).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
        mapping.DidNotReceive().Invoke(Arg.Any<int>());
    }
}
