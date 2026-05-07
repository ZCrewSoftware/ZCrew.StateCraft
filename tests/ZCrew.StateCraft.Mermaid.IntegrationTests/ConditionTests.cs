namespace ZCrew.StateCraft.Mermaid.IntegrationTests;

public class ConditionTests
{
    private static bool IsAuthorized() => true;

    private static bool HasCapacity() => true;

    private static bool QueueIsHealthy() => true;

    [Fact]
    public void ToMermaidDiagram_WhenTransitionHasNoConditions_ShouldNotAppendIfClause()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Go", "Working"))
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Idle --> Working : Go", diagram);
        Assert.DoesNotContain(" <br/> If:", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenTransitionHasOneCondition_ShouldAppendSingleIfClause()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Go", t => t.If(IsAuthorized).To("Working")))
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Idle --> Working : Go <br/> If: IsAuthorized", diagram);
        Assert.DoesNotContain("And:", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenTransitionHasMultipleConditions_ShouldChainWithAndUsingHtmlBreaks()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState(
                "Idle",
                state =>
                    state.WithTransition("Go", t => t.If(IsAuthorized).If(HasCapacity).If(QueueIsHealthy).To("Working"))
            )
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains(
            "    Idle --> Working : Go <br/> If: IsAuthorized <br/> And: HasCapacity <br/> And: QueueIsHealthy",
            diagram
        );
    }

    [Fact]
    public void ToMermaidDiagram_WhenConditionHasCustomDescriptor_ShouldUseProvidedDescriptor()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState(
                "Idle",
                state => state.WithTransition("Go", t => t.If(IsAuthorized, "user is authorized").To("Working"))
            )
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("    Idle --> Working : Go <br/> If: user is authorized", diagram);
        Assert.DoesNotContain("IsAuthorized", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenConditionIsLargerInlineExpression_ShouldRenderTheCapturedExpression()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState(
                "Idle",
                state =>
                    state.WithTransition(
                        "Go",
                        t => t.If(() => IsAuthorized() && HasCapacity() && QueueIsHealthy()).To("Working")
                    )
            )
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("If: () =#gt; IsAuthorized() && HasCapacity() && QueueIsHealthy()", diagram);
    }

    [Fact]
    public void ToMermaidDiagram_WhenConditionIsLargerBlockBody_ShouldRenderTheCapturedBody()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState(
                "Idle",
                state =>
                    state.WithTransition(
                        "Go",
                        t =>
                            t.If(() =>
                                {
                                    var authorized = IsAuthorized();
                                    var capacity = HasCapacity();
                                    return authorized && capacity;
                                })
                                .To("Working")
                    )
            )
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToMermaidDiagram();

        // Assert
        Assert.Contains("If:", diagram);
        Assert.Contains("var authorized = IsAuthorized();", diagram);
        Assert.Contains("var capacity = HasCapacity();", diagram);
        Assert.Contains("return authorized && capacity;", diagram);
    }
}
