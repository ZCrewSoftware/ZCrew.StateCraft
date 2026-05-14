namespace ZCrew.StateCraft.IntegrationTests.Info;

public class StaticInitialStateInfoTests
{
    [Fact]
    public void InitialState_WhenParameterlessConfigured_ShouldHaveValueAndEmptyParameters()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state);

        // Act
        var info = configuration.GetInfo();

        // Assert
        Assert.NotNull(info.InitialState);
        var initialState = Assert.IsAssignableFrom<IStaticInitialStateInfo<string>>(info.InitialState);
        Assert.Equal("A", initialState.InitialStateValue);
        Assert.Empty(initialState.InitialParameters);
        Assert.Empty(initialState.InitialParameterTypes);
    }

    [Fact]
    public void InitialState_WhenParameterless_ShouldHaveValueAndEmptyParameters()
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
        Assert.NotNull(info.InitialState);
        var initialState = Assert.IsAssignableFrom<IStaticInitialStateInfo<string>>(info.InitialState);
        Assert.Equal("A", initialState.InitialStateValue);
        Assert.Empty(initialState.InitialParameters);
        Assert.Empty(initialState.InitialParameterTypes);
    }

    [Fact]
    public void InitialState_WhenFourParametersConfigured_ShouldHaveAllValuesAndTypes()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState<int, string, bool, double>("A", 1, "two", true, 4.0)
            .WithState("A", state => state.WithParameters<int, string, bool, double>());

        // Act
        var info = configuration.GetInfo();

        // Assert
        Assert.NotNull(info.InitialState);
        var initialState = Assert.IsAssignableFrom<IStaticInitialStateInfo<string>>(info.InitialState);
        Assert.Equal("A", initialState.InitialStateValue);
        Assert.Equal([1, "two", true, 4.0], initialState.InitialParameters);
        Assert.Equal([typeof(int), typeof(string), typeof(bool), typeof(double)], initialState.InitialParameterTypes);
    }

    [Fact]
    public void InitialState_WhenFourParameters_ShouldHaveAllValuesAndTypes()
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
        Assert.NotNull(info.InitialState);
        var initialState = Assert.IsAssignableFrom<IStaticInitialStateInfo<string>>(info.InitialState);
        Assert.Equal("A", initialState.InitialStateValue);
        Assert.Equal([1, "two", true, 4.0], initialState.InitialParameters);
        Assert.Equal([typeof(int), typeof(string), typeof(bool), typeof(double)], initialState.InitialParameterTypes);
    }
}
