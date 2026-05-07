namespace ZCrew.StateCraft.Mermaid.IntegrationTests;

public class StateTests
{
    [Fact]
    public void ToMermaidDiagram_WhenStateIsParameterless_ShouldRenderIdentifierMatchingDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Idle: Idle", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenStateHasSingleTypeParameter_ShouldSuffixIdentifierAndDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state)
            .WithState("Working", state => state.WithParameter<int>());

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Working_System.Int32: Working#lt;int#gt;", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenStateHasMultipleTypeParameters_ShouldIncludeEachParameterInIdentifierAndDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state)
            .WithState("Storing", state => state.WithParameters<int, string>());

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Storing_System.Int32_System.String: Storing#lt;int, string#gt;", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenMultipleStatesAreRegistered_ShouldEmitEachOnItsOwnLineInDeclarationOrder()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state)
            .WithState("Working", state => state)
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
                Finished: Finished

            """;
        Assert.Equal(expected.ReplaceLineEndings(), diagram.ReplaceLineEndings());
    }
}
