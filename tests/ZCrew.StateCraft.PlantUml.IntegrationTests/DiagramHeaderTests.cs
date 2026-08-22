namespace ZCrew.StateCraft.PlantUml.IntegrationTests;

public class DiagramHeaderTests
{
    [Fact]
    public void ToPlantUmlDiagram_WhenCalled_ShouldEmitTitleBetweenStartAndEndMarkers()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        var expected = """
            @startuml
            title State Machine

            top to bottom direction

            state "Idle" as Idle

            [*] --> Idle
            @enduml

            """;
        Assert.Equal(expected.ReplaceLineEndings(), diagram.ReplaceLineEndings());
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenDirectionIsTopToBottom_ShouldEmitTopToBottomDirection()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram(options => options.Direction = PlantUmlDirection.TopToBottom);

        // Assert
        Assert.Contains("top to bottom direction", diagram);
        Assert.DoesNotContain("left to right direction", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenDirectionIsLeftToRight_ShouldEmitLeftToRightDirection()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram(options => options.Direction = PlantUmlDirection.LeftToRight);

        // Assert
        Assert.Contains("left to right direction", diagram);
        Assert.DoesNotContain("top to bottom direction", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenDirectionIsInvalid_ShouldThrowArgumentOutOfRange()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state);

        // Act
        var render = () => configuration.ToPlantUmlDiagram(options => options.Direction = (PlantUmlDirection)999);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(render);
    }
}
