namespace ZCrew.StateCraft.IntegrationTests.Info;

public class MappedTransitionInfoTests
{
    [Fact]
    public void Transition_WhenMappedTransitionConfigured_ShouldExposePreviousNextStatesAndMappingFunction()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(value => value.ToString()).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>());

        // Act
        var info = configuration.GetInfo();

        // Assert
        var transition = Assert.Single(info.Transitions);
        var mapped = Assert.IsAssignableFrom<IMappedTransitionInfo<string, string>>(transition);
        Assert.Equal("To B", mapped.TransitionValue);
        Assert.Equal("A", mapped.PreviousState.StateValue);
        Assert.Equal("B", mapped.NextState.StateValue);
        Assert.NotNull(mapped.MappingFunction);
        Assert.Empty(mapped.TransitionParameterTypes);
    }

    [Fact]
    public void Transition_WhenMappedTransition_ShouldExposePreviousNextStatesAndMappingFunction()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(value => value.ToString()).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var transition = Assert.Single(info.Transitions);
        var mapped = Assert.IsAssignableFrom<IMappedTransitionInfo<string, string>>(transition);
        Assert.Equal("To B", mapped.TransitionValue);
        Assert.Equal("A", mapped.PreviousState.StateValue);
        Assert.Equal("B", mapped.NextState.StateValue);
        Assert.NotNull(mapped.MappingFunction);
        Assert.Empty(mapped.TransitionParameterTypes);
    }

    [Fact]
    public void IsConditional_WhenNoConditionsConfigured_ShouldBeFalseWithEmptyConditionLists()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(value => value.ToString()).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>());

        // Act
        var info = configuration.GetInfo();

        // Assert
        var mapped = Assert.IsAssignableFrom<IMappedTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.False(mapped.IsConditional);
        Assert.Empty(mapped.PreviousParameterConditions);
        Assert.Empty(mapped.NextParameterConditions);
    }

    [Fact]
    public void IsConditional_WhenNoConditions_ShouldBeFalseWithEmptyConditionLists()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(value => value.ToString()).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var mapped = Assert.IsAssignableFrom<IMappedTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.False(mapped.IsConditional);
        Assert.Empty(mapped.PreviousParameterConditions);
        Assert.Empty(mapped.NextParameterConditions);
    }

    [Fact]
    public void IsConditional_WhenMappedConditionConfigured_ShouldBeTrueWithNextCondition()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>(value => value.ToString())
                                    .If(mapped => mapped.Length > 0)
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>());

        // Act
        var info = configuration.GetInfo();

        // Assert
        var mapped = Assert.IsAssignableFrom<IMappedTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.True(mapped.IsConditional);
        Assert.Empty(mapped.PreviousParameterConditions);
        Assert.Single(mapped.NextParameterConditions);
    }

    [Fact]
    public void IsConditional_WhenMappedCondition_ShouldBeTrueWithNextCondition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 1)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t =>
                                t.WithMappedParameter<string>(value => value.ToString())
                                    .If(mapped => mapped.Length > 0)
                                    .To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var mapped = Assert.IsAssignableFrom<IMappedTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.True(mapped.IsConditional);
        Assert.Empty(mapped.PreviousParameterConditions);
        Assert.Single(mapped.NextParameterConditions);
    }
}
