namespace ZCrew.StateCraft.IntegrationTests.Info;

public class StateInfoTests
{
    [Fact]
    public void StateInfo_WhenParameterlessStateConfigured_ShouldHaveEmptyParameterTypes()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state);

        // Act
        var info = configuration.GetInfo();

        // Assert
        var stateInfo = Assert.Single(info.States);
        Assert.Equal("A", stateInfo.StateValue);
        Assert.Empty(stateInfo.StateParameterTypes);
    }

    [Fact]
    public void StateInfo_WhenParameterlessState_ShouldHaveEmptyParameterTypes()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var stateInfo = Assert.Single(info.States);
        Assert.Equal("A", stateInfo.StateValue);
        Assert.Empty(stateInfo.StateParameterTypes);
    }

    [Fact]
    public void StateParameterTypes_WhenFourParameterStateConfigured_ShouldReflectAllTypesInOrder()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState<int, string, bool, double>("A", 1, "two", true, 4.0)
            .WithState("A", state => state.WithParameters<int, string, bool, double>());

        // Act
        var info = configuration.GetInfo();

        // Assert
        var stateInfo = Assert.Single(info.States);
        Assert.Equal("A", stateInfo.StateValue);
        Assert.Equal([typeof(int), typeof(string), typeof(bool), typeof(double)], stateInfo.StateParameterTypes);
    }

    [Fact]
    public void StateParameterTypes_WhenFourParameterState_ShouldReflectAllTypesInOrder()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState<int, string, bool, double>("A", 1, "two", true, 4.0)
            .WithState("A", state => state.WithParameters<int, string, bool, double>())
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var stateInfo = Assert.Single(info.States);
        Assert.Equal("A", stateInfo.StateValue);
        Assert.Equal([typeof(int), typeof(string), typeof(bool), typeof(double)], stateInfo.StateParameterTypes);
    }
}
