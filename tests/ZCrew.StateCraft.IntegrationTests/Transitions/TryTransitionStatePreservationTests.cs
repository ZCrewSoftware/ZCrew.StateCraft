namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class TryTransitionStatePreservationTests
{
    [Fact]
    public async Task TryTransition_WhenConditionFails_ShouldPreserveCurrentStateAndParameters()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.If(() => false).To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task TryTransition_T_WhenConditionFails_ShouldPreserveOriginalParameter()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state.WithParameter<int>().WithTransition("To B", t => t.If(_ => false).To("B"))
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
        Assert.Equal(42, stateMachine.Parameters.GetCurrentParameter<int>());
    }
}
