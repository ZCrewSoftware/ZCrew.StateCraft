namespace ZCrew.StateCraft.PlantUml.IntegrationTests;

public class InitialStateTests
{
    [Fact]
    public void ToPlantUmlDiagram_WhenInitialStateIsStatic_ShouldEmitStartMarkerToThatState()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Go", "Working"))
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("[*] --> Idle", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenInitialStateIsStatic_ShouldEmitStartMarkerBeforeTheTransitions()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Go", "Working"))
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        var markerIndex = diagram.IndexOf("[*] --> Idle", StringComparison.Ordinal);
        var transitionIndex = diagram.IndexOf("Idle --> Working", StringComparison.Ordinal);
        Assert.True(markerIndex < transitionIndex);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenInitialStateIsParameterized_ShouldPointAtTheParameterizedAlias()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Working", 1)
            .WithState("Working", state => state.WithParameter<int>());

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("[*] --> Working_int", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenInitialStateIsDynamic_ShouldNotEmitStartMarker()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState(() => "Idle")
            .WithState("Idle", state => state.WithTransition("Go", "Working"))
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.DoesNotContain("[*]", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenStaticInitialStateIsNotConfigured_ShouldNotEmitStartMarker()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Missing")
            .WithState("Idle", state => state.WithTransition("Go", "Working"))
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.DoesNotContain("[*]", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenStaticInitialStateParametersDoNotMatch_ShouldNotEmitStartMarker()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Working")
            .WithState("Working", state => state.WithParameter<int>());

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.DoesNotContain("[*]", diagram);
    }
}
