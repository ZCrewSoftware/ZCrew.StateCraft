namespace ZCrew.StateCraft.Mermaid.IntegrationTests;

public class TransitionTests
{
    [Fact]
    public void ToMermaidDiagram_WhenTransitionIsParameterless_ShouldRenderArrowWithTriggerAsDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Start", "Working"))
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Idle --> Working : Start", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenTransitionTargetsParameterizedState_ShouldIncludeTypeParameterInDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Start", t => t.WithParameter<int>().To("Working")))
            .WithState("Working", state => state.WithParameter<int>());

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Idle --> Working_System.Int32 : Start#lt;int#gt;", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenTransitionIsMapped_ShouldUseTriggerAloneAsDescriptor()
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
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Idle_System.Int32 --> Working_System.String : Start", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenTransitionIsReentrant_ShouldRenderArrowToSameStateIdentifier()
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
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Working_System.Int32 --> Working_System.Int32 : Start", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenStateMachineHasMultipleTransitions_ShouldEmitAllTransitionsInDeclarationOrder()
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
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        var expected = """
            ---
            title: State Machine
            ---
            stateDiagram-v2
                direction TB

                Idle: Idle
                Working: Working
                Suspended: Suspended
                Finished: Finished

                Idle --> Working : Start
                Idle --> Finished : Stop
                Working --> Suspended : Suspend
                Working --> Finished : Stop
                Suspended --> Working : Resume

            """;
        Assert.Equal(expected.ReplaceLineEndings(), diagram.ReplaceLineEndings());
    }
}
