namespace ZCrew.StateCraft.Mermaid.IntegrationTests;

public class ToMermaidDiagramApiTests
{
    [Fact]
    public void ToMermaidDiagram_WhenOptionsInstanceIsProvided_ShouldHonorSuppliedDirection()
    {
        // Arrange
        var configuration = NewConfiguration();
        var options = new MermaidOptions { Direction = MermaidDirection.LeftToRight };

        // Act
        var diagram = configuration.ToMermaidDiagram(options);

        // Assert
        Assert.Contains("    direction LR", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenOptionsInstanceIsNull_ShouldFallBackToDefaults()
    {
        // Arrange
        var configuration = NewConfiguration();

        // Act
        var defaultDiagram = configuration.ToMermaidDiagram();
        var nullDiagram = configuration.ToMermaidDiagram((MermaidOptions?)null);

        // Assert
        Assert.Equal(defaultDiagram, nullDiagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenConfigureCallbackIsProvided_ShouldApplyMutationsToFreshOptions()
    {
        // Arrange
        var configuration = NewConfiguration();

        // Act
        var diagram = configuration.ToMermaidDiagram(options => options.Direction = MermaidDirection.LeftToRight);

        // Assert
        Assert.Contains("    direction LR", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenConfigureCallbackIsNull_ShouldFallBackToDefaults()
    {
        // Arrange
        var configuration = NewConfiguration();

        // Act
        var defaultDiagram = configuration.ToMermaidDiagram();
        var nullDiagram = configuration.ToMermaidDiagram((Action<MermaidOptions>?)null);

        // Assert
        Assert.Equal(defaultDiagram, nullDiagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenInstanceAndCallbackOverloadsExpressSameOptions_ShouldProduceIdenticalOutput()
    {
        // Arrange
        var configuration = NewConfiguration();

        // Act
        var fromInstance = configuration.ToMermaidDiagram(
            new MermaidOptions
            {
                Direction = MermaidDirection.LeftToRight,
                Newline = MermaidNewline.HtmlSingleLineBreak,
            }
        );
        var fromCallback = configuration.ToMermaidDiagram(options =>
        {
            options.Direction = MermaidDirection.LeftToRight;
            options.Newline = MermaidNewline.HtmlSingleLineBreak;
        });

        // Assert
        Assert.Equal(fromInstance, fromCallback);
    }

    private static IStateMachineConfiguration<string, string> NewConfiguration()
    {
        return StateMachine.Configure<string, string>().WithInitialState("Idle").WithState("Idle", state => state);
    }
}
