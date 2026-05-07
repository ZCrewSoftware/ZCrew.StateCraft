namespace ZCrew.StateCraft.Mermaid.IntegrationTests;

public class DiagramHeaderTests
{
    [Fact]
    public void ToMermaidDiagram_WhenCalled_ShouldEmitTitleAndStateDiagramHeader()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state);

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

            """;
        Assert.Equal(expected.ReplaceLineEndings(), diagram.ReplaceLineEndings());
    }

    [Fact]
    public void ToMermaidDiagram_WhenDirectionIsTopToBottom_ShouldEmitDirectionTB()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram(options => options.Direction = MermaidDirection.TopToBottom);

        // Assert
        Assert.Contains("    direction TB", diagram);
        Assert.DoesNotContain("    direction LR", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenDirectionIsLeftToRight_ShouldEmitDirectionLR()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram(options => options.Direction = MermaidDirection.LeftToRight);

        // Assert
        Assert.Contains("    direction LR", diagram);
        Assert.DoesNotContain("    direction TB", diagram);
    }
}
