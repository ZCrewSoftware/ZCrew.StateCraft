namespace ZCrew.StateCraft.Mermaid.IntegrationTests;

public class FromTransitionTests
{
    [Fact]
    public void ToMermaidDiagram_WhenFromTransitionExcludesNoStates_ShouldRenderEdgeFromEveryState()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("Hub", state => state.WithTransition("Reset", t => t.From().AllStates()));

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    A --> Hub : Reset", diagram);
        Assert.Contains("    B --> Hub : Reset", diagram);
        Assert.Contains("    Hub --> Hub : Reset", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenFromTransitionExcludesSomeStates_ShouldSkipExcludedSources()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("Hub", state => state.WithTransition("Reset", t => t.From().AllOtherStates()));

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    A --> Hub : Reset", diagram);
        Assert.Contains("    B --> Hub : Reset", diagram);
        Assert.DoesNotContain("Hub --> Hub", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenFromTransitionExcludesAllStates_ShouldRenderNoEdges()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState(
                "Hub",
                state => state.WithTransition("Reset", t => t.From().AllStates().Except("A").Except("B").Except("Hub"))
            );

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Hub: Hub", diagram);
        Assert.DoesNotContain("-->", diagram);
    }
}
