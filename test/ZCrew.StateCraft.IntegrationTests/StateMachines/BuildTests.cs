namespace ZCrew.StateCraft.IntegrationTests.StateMachines;

public class BuildTests
{
    [Fact]
    public void Build_WhenNoInitialState_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>();

        // Act
        var build = () => configuration.Build();

        // Assert
        Assert.Throws<InvalidOperationException>(build);
    }

    [Fact]
    public void Build_WhenInitialStateValue_ShouldSucceed()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState("A");

        // Act
        var stateMachine = configuration.Build();

        // Assert
        Assert.NotNull(stateMachine);
    }

    [Fact]
    public void Build_WhenInitialStateFunc_ShouldSucceed()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState(() => "A");

        // Act
        var stateMachine = configuration.Build();

        // Assert
        Assert.NotNull(stateMachine);
    }

    [Fact]
    public void Build_WhenInitialStateFuncTask_ShouldSucceed()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState(_ => Task.FromResult("A"));

        // Act
        var stateMachine = configuration.Build();

        // Assert
        Assert.NotNull(stateMachine);
    }

    [Fact]
    public void Build_WhenInitialStateFuncValueTask_ShouldSucceed()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithInitialState(_ => ValueTask.FromResult("A"));

        // Act
        var stateMachine = configuration.Build();

        // Assert
        Assert.NotNull(stateMachine);
    }
}
