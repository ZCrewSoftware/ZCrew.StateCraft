namespace ZCrew.StateCraft.IntegrationTests.Info;

public class MappingFunctionInfoTests
{
    [Fact]
    public void Descriptor_WhenMappingConfiguredWithCallerExpression_ShouldCaptureExpression()
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
        Assert.Equal("value => value.ToString()", mapped.MappingFunction.Descriptor);
    }

    [Fact]
    public void Descriptor_WhenMappingWithCallerExpression_ShouldCaptureExpression()
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
        Assert.Equal("value => value.ToString()", mapped.MappingFunction.Descriptor);
    }

    [Fact]
    public void TypeParametersAndResultTypes_WhenSingleInputSingleOutputMappingConfigured_ShouldReflectInputAndOutputTypes()
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
        Assert.Equal([typeof(int)], mapped.MappingFunction.TypeParameters);
        Assert.Equal([typeof(string)], mapped.MappingFunction.ResultTypes);
    }

    [Fact]
    public void TypeParametersAndResultTypes_WhenSingleInputSingleOutputMapping_ShouldReflectInputAndOutputTypes()
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
        Assert.Equal([typeof(int)], mapped.MappingFunction.TypeParameters);
        Assert.Equal([typeof(string)], mapped.MappingFunction.ResultTypes);
    }

    [Fact]
    public void ResultTypes_WhenMappingToTwoParametersConfigured_ShouldContainBothOutputTypes()
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
                            t => t.WithMappedParameters<string, bool>(value => (value.ToString(), value > 0)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<string, bool>());

        // Act
        var info = configuration.GetInfo();

        // Assert
        var mapped = Assert.IsAssignableFrom<IMappedTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.Equal([typeof(int)], mapped.MappingFunction.TypeParameters);
        Assert.Equal([typeof(string), typeof(bool)], mapped.MappingFunction.ResultTypes);
    }

    [Fact]
    public void ResultTypes_WhenMappingToTwoParameters_ShouldContainBothOutputTypes()
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
                            t => t.WithMappedParameters<string, bool>(value => (value.ToString(), value > 0)).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<string, bool>())
            .Build();

        // Act
        var info = stateMachine.GetInfo();

        // Assert
        var mapped = Assert.IsAssignableFrom<IMappedTransitionInfo<string, string>>(info.Transitions[0]);
        Assert.Equal([typeof(int)], mapped.MappingFunction.TypeParameters);
        Assert.Equal([typeof(string), typeof(bool)], mapped.MappingFunction.ResultTypes);
    }
}
