namespace ZCrew.StateCraft.PlantUml.IntegrationTests;

public class TransitionTests
{
    [Fact]
    public void ToPlantUmlDiagram_WhenTransitionIsParameterless_ShouldRenderArrowWithTriggerAsDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Start", "Working"))
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("Idle --> Working : Start", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenTransitionTargetsParameterizedState_ShouldIncludeTypeParameterInDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Start", t => t.WithParameter<int>().To("Working")))
            .WithState("Working", state => state.WithParameter<int>());

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("Idle --> Working_int : Start<U+003C>int<U+003E>", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenTransitionIsMapped_ShouldUseTriggerAloneAsDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle", 1)
            .WithState(
                "Idle",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "Start",
                            t => t.WithMappedParameter<string>(value => value.ToString()).To("Working")
                        )
            )
            .WithState("Working", state => state.WithParameter<string>());

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("Idle_int --> Working_string : Start", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenTransitionIsReentrant_ShouldRenderArrowToSameAlias()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Working", 1)
            .WithState(
                "Working",
                state => state.WithParameter<int>().WithTransition("Start", t => t.WithSameParameter().To("Working"))
            );

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("Working_int --> Working_int : Start", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenStateMachineHasMultipleTransitions_ShouldEmitAllTransitionsInDeclarationOrder()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Start", "Working").WithTransition("Stop", "Finished"))
            .WithState(
                "Working",
                state => state.WithTransition("Suspend", "Suspended").WithTransition("Stop", "Finished")
            )
            .WithState("Suspended", state => state.WithTransition("Resume", "Working"))
            .WithState("Finished", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        var expected = """
            @startuml
            title State Machine

            top to bottom direction

            state "Idle" as Idle
            state "Working" as Working
            state "Suspended" as Suspended
            state "Finished" as Finished

            [*] --> Idle
            Idle --> Working : Start
            Idle --> Finished : Stop
            Working --> Suspended : Suspend
            Working --> Finished : Stop
            Suspended --> Working : Resume
            @enduml

            """;
        Assert.Equal(expected.ReplaceLineEndings(), diagram.ReplaceLineEndings());
    }
}
