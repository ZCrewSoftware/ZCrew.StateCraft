namespace ZCrew.StateCraft.IntegrationTests.Info;

public class DirectTransitionInfoTests
{
    [Fact]
    public void Transition_WhenDirectTransitionConfigured_ShouldExposePreviousAndNextStates()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state);

        // Act
        var info = configuration.GetInfo();

        // Assert
        var transition = Assert.Single(info.Transitions);
        var direct = Assert.IsAssignableFrom<IDirectTransitionInfo<string, string>>(transition);
        Assert.Equal("To B", direct.TransitionValue);
        Assert.Equal("A", direct.PreviousState.StateValue);
        Assert.Equal("B", direct.NextState.StateValue);
    }

    [Fact]
    public void Transition_WhenDirectTransition_ShouldExposePreviousAndNextStates()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var transition = Assert.Single(info.Transitions);
        var direct = Assert.IsAssignableFrom<IDirectTransitionInfo<string, string>>(transition);
        Assert.Equal("To B", direct.TransitionValue);
        Assert.Equal("A", direct.PreviousState.StateValue);
        Assert.Equal("B", direct.NextState.StateValue);
    }

    [Fact]
    public void IsConditional_WhenNoConditionsConfigured_ShouldBeFalseWithEmptyConditionLists()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state);

        // Act
        var info = configuration.GetInfo();

        // Assert
        var direct = Assert.IsAssignableFrom<IDirectTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.False(direct.IsConditional);
        Assert.Empty(direct.PreviousParameterConditions);
        Assert.Empty(direct.NextParameterConditions);
    }

    [Fact]
    public void IsConditional_WhenNoConditions_ShouldBeFalseWithEmptyConditionLists()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var direct = Assert.IsAssignableFrom<IDirectTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.False(direct.IsConditional);
        Assert.Empty(direct.PreviousParameterConditions);
        Assert.Empty(direct.NextParameterConditions);
    }

    [Fact]
    public void IsConditional_WhenDestinationConditionConfigured_ShouldBeTrueWithNextCondition()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.WithTransition("To B", t => t.WithParameter<int>().If(value => value > 0).To("B"))
            )
            .WithState("B", state => state.WithParameter<int>());

        // Act
        var info = configuration.GetInfo();

        // Assert
        var direct = Assert.IsAssignableFrom<IDirectTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.True(direct.IsConditional);
        Assert.Empty(direct.PreviousParameterConditions);
        Assert.Single(direct.NextParameterConditions);
    }

    [Fact]
    public void IsConditional_WhenDestinationCondition_ShouldBeTrueWithNextCondition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.WithTransition("To B", t => t.WithParameter<int>().If(value => value > 0).To("B"))
            )
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var direct = Assert.IsAssignableFrom<IDirectTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.True(direct.IsConditional);
        Assert.Empty(direct.PreviousParameterConditions);
        Assert.Single(direct.NextParameterConditions);
    }
}
