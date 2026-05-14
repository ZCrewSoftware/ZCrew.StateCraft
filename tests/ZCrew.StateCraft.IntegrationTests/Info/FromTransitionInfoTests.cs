namespace ZCrew.StateCraft.IntegrationTests.Info;

public class FromTransitionInfoTests
{
    [Fact]
    public void Transition_WhenFromAllOtherStatesConfigured_ShouldExcludeOnlyDestination()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()));

        // Act
        var info = configuration.GetInfo();

        // Assert
        var transition = Assert.Single(info.Transitions);
        var from = Assert.IsAssignableFrom<IFromTransitionInfo<string, string>>(transition);
        Assert.Equal("To D", from.TransitionValue);
        Assert.Equal("D", from.NextState.StateValue);
        var excluded = Assert.Single(from.ExcludedStates);
        Assert.Equal("D", excluded.StateValue);
    }

    [Fact]
    public void Transition_WhenFromAllOtherStates_ShouldExcludeOnlyDestination()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var transition = Assert.Single(info.Transitions);
        var from = Assert.IsAssignableFrom<IFromTransitionInfo<string, string>>(transition);
        Assert.Equal("To D", from.TransitionValue);
        Assert.Equal("D", from.NextState.StateValue);
        var excluded = Assert.Single(from.ExcludedStates);
        Assert.Equal("D", excluded.StateValue);
    }

    [Fact]
    public void Transition_WhenFromAllStatesConfigured_ShouldHaveNoExclusions()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllStates()));

        // Act
        var info = configuration.GetInfo();

        // Assert
        var transition = Assert.Single(info.Transitions);
        var from = Assert.IsAssignableFrom<IFromTransitionInfo<string, string>>(transition);
        Assert.Equal("D", from.NextState.StateValue);
        Assert.Empty(from.ExcludedStates);
    }

    [Fact]
    public void Transition_WhenFromAllStates_ShouldHaveNoExclusions()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllStates()))
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var transition = Assert.Single(info.Transitions);
        var from = Assert.IsAssignableFrom<IFromTransitionInfo<string, string>>(transition);
        Assert.Equal("D", from.NextState.StateValue);
        Assert.Empty(from.ExcludedStates);
    }

    [Fact]
    public void Transition_WhenExceptStateConfigured_ShouldAppearInExcludedStates()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("C", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates().Except("B")));

        // Act
        var info = configuration.GetInfo();

        // Assert
        var transition = Assert.Single(info.Transitions);
        var from = Assert.IsAssignableFrom<IFromTransitionInfo<string, string>>(transition);
        Assert.Equal(2, from.ExcludedStates.Count);
        Assert.Contains(from.ExcludedStates, s => Equals(s.StateValue, "D"));
        Assert.Contains(from.ExcludedStates, s => Equals(s.StateValue, "B"));
    }

    [Fact]
    public void Transition_WhenExceptState_ShouldAppearInExcludedStates()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("C", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates().Except("B")))
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var transition = Assert.Single(info.Transitions);
        var from = Assert.IsAssignableFrom<IFromTransitionInfo<string, string>>(transition);
        Assert.Equal(2, from.ExcludedStates.Count);
        Assert.Contains(from.ExcludedStates, s => Equals(s.StateValue, "D"));
        Assert.Contains(from.ExcludedStates, s => Equals(s.StateValue, "B"));
    }

    [Fact]
    public void IsConditional_WhenNoConditionsConfigured_ShouldBeFalseWithEmptyConditionList()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()));

        // Act
        var info = configuration.GetInfo();

        // Assert
        var from = Assert.IsAssignableFrom<IFromTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.False(from.IsConditional);
        Assert.Empty(from.NextParameterConditions);
    }

    [Fact]
    public void IsConditional_WhenNoConditions_ShouldBeFalseWithEmptyConditionList()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var from = Assert.IsAssignableFrom<IFromTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.False(from.IsConditional);
        Assert.Empty(from.NextParameterConditions);
    }
}
