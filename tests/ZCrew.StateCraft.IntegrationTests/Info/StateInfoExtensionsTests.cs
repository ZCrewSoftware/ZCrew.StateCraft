using ZCrew.StateCraft.Info.Extensions;

namespace ZCrew.StateCraft.IntegrationTests.Info;

public class StateInfoExtensionsTests
{
    [Fact]
    public void GetTransitions_WhenStateHasOutgoingTransition_ShouldReturnIt()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var transitions = info.GetState("A").GetTransitions().ToList();

        // Assert
        var transition = Assert.Single(transitions);
        Assert.Equal("To B", transition.TransitionValue);
    }

    [Fact]
    public void GetTransitions_WhenStateHasNoOutgoingTransitions_ShouldReturnEmpty()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var transitions = info.GetState("B").GetTransitions().ToList();

        // Assert
        Assert.Empty(transitions);
    }

    [Fact]
    public void GetTransitions_WhenFromTransitionAndStateIsIncludedSource_ShouldReturnIt()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .GetInfo();

        // Act
        var transitions = info.GetState("A").GetTransitions().ToList();

        // Assert
        var transition = Assert.Single(transitions);
        Assert.Equal("To D", transition.TransitionValue);
    }

    [Fact]
    public void GetTransitions_WhenFromTransitionAndStateIsExcludedDestination_ShouldReturnEmpty()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .GetInfo();

        // Act
        var transitions = info.GetState("D").GetTransitions().ToList();

        // Assert
        Assert.Empty(transitions);
    }

    [Fact]
    public void GetNextStates_WhenLinearChain_ShouldReturnImmediateSuccessorOnly()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state.WithTransition("To C", "C"))
            .WithState("C", state => state)
            .GetInfo();

        // Act
        var nextStates = info.GetState("A").GetNextStates().Select(state => state.StateValue).ToList();

        // Assert
        Assert.Equal(["B"], nextStates);
    }

    [Fact]
    public void GetNextStates_WhenMultipleTransitionsToDifferentStates_ShouldReturnAll()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B").WithTransition("To C", "C"))
            .WithState("B", state => state)
            .WithState("C", state => state)
            .GetInfo();

        // Act
        var nextStates = info.GetState("A").GetNextStates().Select(state => state.StateValue).ToList();

        // Assert
        Assert.Equal(2, nextStates.Count);
        Assert.Contains("B", nextStates);
        Assert.Contains("C", nextStates);
    }

    [Fact]
    public void GetNextStates_WhenMultipleTransitionsToSameState_ShouldReturnDuplicates()
    {
        // Arrange
        var info = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("First To B", "B").WithTransition("Second To B", "B"))
            .WithState("B", state => state)
            .GetInfo();

        // Act
        var nextStates = info.GetState("A").GetNextStates().Select(state => state.StateValue).ToList();

        // Assert
        Assert.Equal(["B", "B"], nextStates);
    }
}
