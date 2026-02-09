namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class CurrentStateTests
{
    [Fact]
    public void CurrentState_WhenBeforeActivation_ShouldBeNull()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        // Act
        var state = stateMachine.CurrentState;

        // Assert
        Assert.Null(state);
    }
}
