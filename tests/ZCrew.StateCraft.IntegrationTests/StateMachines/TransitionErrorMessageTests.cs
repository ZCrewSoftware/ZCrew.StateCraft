namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class TransitionErrorMessageTests
{
    [Fact(Skip = "BL-F08: Transition error message lacks current state")]
    public async Task Transition_WhenInvalidTransition_ShouldIncludeCurrentStateInErrorMessage()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("To Active", t => t.To("Active")))
            .WithState("Active", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("NonExistent", TestContext.Current.CancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(transition);
        Assert.Contains("Idle", exception.Message);
    }

    [Fact(Skip = "BL-F08: Transition error message lacks available transitions")]
    public async Task Transition_WhenInvalidTransition_ShouldIncludeAvailableTransitionsInErrorMessage()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState(
                "Idle",
                state =>
                    state
                        .WithTransition("To Active", t => t.To("Active"))
                        .WithTransition("To Disabled", t => t.To("Disabled"))
            )
            .WithState("Active", state => state)
            .WithState("Disabled", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () =>
            stateMachine.Transition("NonExistent", TestContext.Current.CancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(transition);
        Assert.Contains("To Active", exception.Message);
        Assert.Contains("To Disabled", exception.Message);
    }
}
