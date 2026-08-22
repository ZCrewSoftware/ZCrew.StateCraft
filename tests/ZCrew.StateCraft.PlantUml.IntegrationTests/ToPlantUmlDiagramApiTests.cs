namespace ZCrew.StateCraft.PlantUml.IntegrationTests;

public class ToPlantUmlDiagramApiTests
{
    [Fact]
    public void ToPlantUmlDiagram_WhenOptionsInstanceIsProvided_ShouldHonorSuppliedDirection()
    {
        // Arrange
        var configuration = NewConfiguration();
        var options = new PlantUmlOptions { Direction = PlantUmlDirection.LeftToRight };

        // Act
        var diagram = configuration.ToPlantUmlDiagram(options);

        // Assert
        Assert.Contains("left to right direction", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenOptionsInstanceIsNull_ShouldFallBackToDefaults()
    {
        // Arrange
        var configuration = NewConfiguration();

        // Act
        var defaultDiagram = configuration.ToPlantUmlDiagram();
        var nullDiagram = configuration.ToPlantUmlDiagram((PlantUmlOptions?)null);

        // Assert
        Assert.Equal(defaultDiagram, nullDiagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenConfigureCallbackIsProvided_ShouldApplyMutationsToFreshOptions()
    {
        // Arrange
        var configuration = NewConfiguration();

        // Act
        var diagram = configuration.ToPlantUmlDiagram(options => options.Direction = PlantUmlDirection.LeftToRight);

        // Assert
        Assert.Contains("left to right direction", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenConfigureCallbackIsNull_ShouldFallBackToDefaults()
    {
        // Arrange
        var configuration = NewConfiguration();

        // Act
        var defaultDiagram = configuration.ToPlantUmlDiagram();
        var nullDiagram = configuration.ToPlantUmlDiagram((Action<PlantUmlOptions>?)null);

        // Assert
        Assert.Equal(defaultDiagram, nullDiagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenInstanceAndCallbackOverloadsExpressSameOptions_ShouldProduceIdenticalOutput()
    {
        // Arrange
        var configuration = NewConfiguration();

        // Act
        var fromInstance = configuration.ToPlantUmlDiagram(
            new PlantUmlOptions { Direction = PlantUmlDirection.LeftToRight, Newline = PlantUmlNewline.LineBreak }
        );
        var fromCallback = configuration.ToPlantUmlDiagram(options =>
        {
            options.Direction = PlantUmlDirection.LeftToRight;
            options.Newline = PlantUmlNewline.LineBreak;
        });

        // Assert
        Assert.Equal(fromInstance, fromCallback);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenCalledTwice_ShouldProduceTheSameDiagram()
    {
        // Arrange
        var configuration = NewConfiguration();

        // Act
        var first = configuration.ToPlantUmlDiagram();
        var second = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Equal(first, second);
    }

    private static IStateMachineConfiguration<string, string> NewConfiguration()
    {
        return StateMachine.Configure<string, string>().WithInitialState("Idle").WithState("Idle", state => state);
    }
}
