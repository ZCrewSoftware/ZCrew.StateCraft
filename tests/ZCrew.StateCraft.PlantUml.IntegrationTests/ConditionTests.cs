namespace ZCrew.StateCraft.PlantUml.IntegrationTests;

public class ConditionTests
{
    private static bool IsAuthorized() => true;

    private static bool HasCapacity() => true;

    private static bool QueueIsHealthy() => true;

    [Fact]
    public void ToPlantUmlDiagram_WhenTransitionHasNoConditions_ShouldNotAppendIfClause()
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
        Assert.Contains("Idle --> Working : Go", diagram);
        Assert.DoesNotContain("If:", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenTransitionHasOneCondition_ShouldAppendSingleIfClause()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState("Idle", state => state.WithTransition("Go", t => t.If(IsAuthorized).To("Working")))
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains(@"Idle --> Working : Go\nIf: IsAuthorized", diagram);
        Assert.DoesNotContain("And:", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenTransitionHasMultipleConditions_ShouldChainWithAndUsingLineBreaks()
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
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains(@"Idle --> Working : Go\nIf: IsAuthorized\nAnd: HasCapacity\nAnd: QueueIsHealthy", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenConditionHasCustomDescriptor_ShouldUseProvidedDescriptor()
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
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains(@"Idle --> Working : Go\nIf: user is authorized", diagram);
        Assert.DoesNotContain("IsAuthorized", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenConditionIsLargerInlineExpression_ShouldEncodeTheCapturedExpression()
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
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("If: () =<U+003E> IsAuthorized() && HasCapacity() && QueueIsHealthy()", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenConditionHasNoDescriptor_ShouldNotAppendIfClause()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState(
                "Idle",
                state => state.WithTransition("Go", t => t.If(IsAuthorized, descriptor: null).To("Working"))
            )
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains("Idle --> Working : Go", diagram);
        Assert.DoesNotContain("If:", diagram);
    }

    [Fact]
    public void ToPlantUmlDiagram_WhenOneConditionHasNoDescriptor_ShouldRenderOnlyTheDescribedCondition()
    {
        // Arrange
        var configuration = StateMachine
            .Configure<string, string>()
            .WithInitialState("Idle")
            .WithState(
                "Idle",
                state =>
                    state.WithTransition("Go", t => t.If(IsAuthorized, descriptor: null).If(HasCapacity).To("Working"))
            )
            .WithState("Working", state => state);

        // Act
        var diagram = configuration.ToPlantUmlDiagram();

        // Assert
        Assert.Contains(@"Idle --> Working : Go\nIf: HasCapacity", diagram);
        Assert.DoesNotContain("And:", diagram);
        Assert.DoesNotContain("IsAuthorized", diagram);
    }
}
