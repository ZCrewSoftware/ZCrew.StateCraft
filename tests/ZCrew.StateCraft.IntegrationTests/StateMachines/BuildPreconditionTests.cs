namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class BuildPreconditionTests
{
    [Fact]
    public void Build_WhenInvalidOptions_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState("A");

        // Act
        var build = () => configuration.Build((StateMachineBuildOptions)999);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(build);
    }

    [Fact]
    public void Build_WithValidate_WhenConfigurationValid_ShouldSucceed()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state);

        // Act
        var stateMachine = configuration.Build(StateMachineBuildOptions.Validate);

        // Assert
        Assert.NotNull(stateMachine);
    }

    [Fact]
    public void Build_WithValidate_WhenNoStates_ShouldSucceed()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState("A");

        // Act
        var stateMachine = configuration.Build(StateMachineBuildOptions.Validate);

        // Assert
        Assert.NotNull(stateMachine);
    }
}
