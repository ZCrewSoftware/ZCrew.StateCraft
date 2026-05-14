namespace ZCrew.StateCraft.IntegrationTests.Info;

public class ConditionInfoTests
{
    [Fact]
    public void Descriptor_WhenConditionConfiguredWithCallerExpression_ShouldCaptureExpression()
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
        var condition = Assert.Single(direct.NextParameterConditions);
        Assert.Equal("value => value > 0", condition.Descriptor);
    }

    [Fact]
    public void Descriptor_WhenConditionWithCallerExpression_ShouldCaptureExpression()
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
        var condition = Assert.Single(direct.NextParameterConditions);
        Assert.Equal("value => value > 0", condition.Descriptor);
    }

    [Fact]
    public void TypeParameters_WhenSingleParameterConditionConfigured_ShouldContainParameterType()
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
        var condition = Assert.Single(direct.NextParameterConditions);
        Assert.Equal([typeof(int)], condition.TypeParameters);
    }

    [Fact]
    public void TypeParameters_WhenSingleParameterCondition_ShouldContainParameterType()
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
        var condition = Assert.Single(direct.NextParameterConditions);
        Assert.Equal([typeof(int)], condition.TypeParameters);
    }
}
