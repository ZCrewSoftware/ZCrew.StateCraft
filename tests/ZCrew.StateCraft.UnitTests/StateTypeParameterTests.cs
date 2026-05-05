namespace ZCrew.StateCraft.UnitTests;

public class StateTypeParameterTests
{
    [Fact]
    public async Task TypeParameters_WhenParameterlessState_ShouldReturnEmptyCollection()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var typeParameters = stateMachine.CurrentState!.TypeParameters;

        // Assert
        Assert.Empty(typeParameters);
    }

    [Fact]
    public async Task TypeParameters_WhenSingleParameterState_ShouldReturnSingleType()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var typeParameters = stateMachine.CurrentState!.TypeParameters;

        // Assert
        Assert.Equal([typeof(int)], typeParameters);
    }

    [Fact]
    public async Task TypeParameters_WhenTwoParameterState_ShouldReturnBothTypes()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState("A", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var typeParameters = stateMachine.CurrentState!.TypeParameters;

        // Assert
        Assert.Equal([typeof(int), typeof(string)], typeParameters);
    }

    [Fact]
    public async Task TypeParameters_WhenFourParameterState_ShouldReturnAllFourTypes()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", true, 3.14)
            .WithState("A", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var typeParameters = stateMachine.CurrentState!.TypeParameters;

        // Assert
        Assert.Equal([typeof(int), typeof(string), typeof(bool), typeof(double)], typeParameters);
    }
}
