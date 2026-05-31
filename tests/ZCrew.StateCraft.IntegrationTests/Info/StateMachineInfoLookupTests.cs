using ZCrew.StateCraft.Info.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.Info;

public class StateMachineInfoLookupTests
{
    [Fact]
    public void GetState_WhenParameterlessStateExists_ShouldReturnState()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var state = info.GetState("B");

        // Assert
        Assert.Equal("B", state.StateValue);
        Assert.Empty(state.StateParameterTypes);
    }

    [Fact]
    public void GetState_T_WhenParameterizedStateExists_ShouldReturnMatchingState()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>())
            .GetInfo();

        // Act
        var state = info.GetState("A", typeof(int));

        // Assert
        Assert.Equal("A", state.StateValue);
        Assert.Equal([typeof(int)], state.StateParameterTypes);
    }

    [Fact]
    public void GetState_WhenParameterTypesDoNotMatch_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>())
            .GetInfo();

        // Act
        var getState = () => info.GetState("A");

        // Assert
        Assert.Throws<InvalidOperationException>(getState);
    }

    [Fact]
    public void GetState_WhenStateDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .GetInfo();

        // Act
        var getState = () => info.GetState("Z");

        // Assert
        Assert.Throws<InvalidOperationException>(getState);
    }

    [Fact]
    public void GetStateOrDefault_WhenStateDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .GetInfo();

        // Act
        var state = info.GetStateOrDefault("Z");

        // Assert
        Assert.Null(state);
    }
}
