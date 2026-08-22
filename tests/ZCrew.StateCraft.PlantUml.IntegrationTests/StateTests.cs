namespace ZCrew.StateCraft.PlantUml.IntegrationTests;

public class StateTests
{
    [Fact]
    public void ToPlantUmlDiagram_WhenStateIsParameterless_ShouldDeclareStateWithMatchingLabelAndAlias()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("state \"Idle\" as Idle", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenStateHasOneParameter_ShouldIncludeTypeInLabelAndAlias()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Working", 1)
            .WithState("Working", state => state.WithParameter<int>());

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("state \"Working<U+003C>int<U+003E>\" as Working_int", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenStateHasMultipleParameters_ShouldIncludeEveryTypeInLabelAndAlias()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Working", 1, "a")
            .WithState("Working", state => state.WithParameters<int, string>());

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("state \"Working<U+003C>int, string<U+003E>\" as Working_int_string", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenStateValueContainsUnsafeCharacters_ShouldSanitizeAliasButNotLabel()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Order.Placed")
            .WithState("Order.Placed", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("state \"Order.Placed\" as Order_Placed", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenStateValueStartsWithDigit_ShouldPrefixAliasWithUnderscore()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("1Idle")
            .WithState("1Idle", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("state \"1Idle\" as _1Idle", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenSanitizedAliasesCollide_ShouldSuffixTheLaterAlias()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A.B")
            .WithState("A.B", state => state)
            .WithState("A-B", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("state \"A.B\" as A_B", diagram);
        Assert.Contains("state \"A-B\" as A_B_2", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenStatesShareValueButDifferParameters_ShouldDeclareSeparateNodes()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Working", 1)
            .WithState("Working", state => state.WithParameter<int>())
            .WithState("Working", state => state.WithParameter<string>());

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("state \"Working<U+003C>int<U+003E>\" as Working_int", diagram);
        Assert.Contains("state \"Working<U+003C>string<U+003E>\" as Working_string", diagram);
    }
}
