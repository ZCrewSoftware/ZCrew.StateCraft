namespace ZCrew.StateCraft.IntegrationTests.Info;

public class StateMachineInfoTests
{
    [Fact]
    public void InitialState_WhenConfigured_ShouldNotBeNull()
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
    }

    [Fact]
    public void InitialState_When_ShouldNotBeNull()
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
    }

    [Fact]
    public void InitialState_WhenNotConfigured_ShouldBeNull()
    {
        // Arrange
        var configuration = StateMachine.Configure<string, string>().WithState("A", state => state);

        // Act
        var info = configuration.GetInfo();

        // Assert
        Assert.Null(info.InitialState);
    }

    [Fact]
    public void States_WhenMultipleConfigured_ShouldReturnAllInDeclarationOrder()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("C", state => state);

        // Act
        var info = configuration.GetInfo();

        // Assert
        Assert.Equal(3, info.States.Count);
        Assert.Equal("A", info.States[0].StateValue);
        Assert.Equal("B", info.States[1].StateValue);
        Assert.Equal("C", info.States[2].StateValue);
    }

    [Fact]
    public void States_WhenMultiple_ShouldReturnAllInDeclarationOrder()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("C", state => state)
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        Assert.Equal(3, info.States.Count);
        Assert.Equal("A", info.States[0].StateValue);
        Assert.Equal("B", info.States[1].StateValue);
        Assert.Equal("C", info.States[2].StateValue);
    }

    [Fact]
    public void Transitions_WhenMultipleConfigured_ShouldReturnAllInDeclarationOrder()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B").WithTransition("To C", "C"))
            .WithState("B", state => state)
            .WithState("C", state => state);

        // Act
        var info = configuration.GetInfo();

        // Assert
        Assert.Equal(2, info.Transitions.Count);
        Assert.Equal("To B", info.Transitions[0].TransitionValue);
        Assert.Equal("To C", info.Transitions[1].TransitionValue);
    }

    [Fact]
    public void Transitions_WhenMultiple_ShouldReturnAllInDeclarationOrder()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B").WithTransition("To C", "C"))
            .WithState("B", state => state)
            .WithState("C", state => state)
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        Assert.Equal(2, info.Transitions.Count);
        Assert.Equal("To B", info.Transitions[0].TransitionValue);
        Assert.Equal("To C", info.Transitions[1].TransitionValue);
    }
}
